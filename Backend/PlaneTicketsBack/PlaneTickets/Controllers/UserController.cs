using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PlaneTickets.Contracts.User;
using PlaneTickets.Models;
using PlaneTickets.ServiceErrors;
using PlaneTickets.Services.Users;

namespace PlaneTickets.Controllers;

public class UserController : ApiController
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    public UserController(IUserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

    [HttpPost("user/register/"), Authorize(Roles = "Admin")]
    public IActionResult RegisterUser(UserDto request)
    {
        ErrorOr<UserDb> requestToUserResult = UserDb.From(request);

        if (requestToUserResult.IsError)
        {
            return Problem(requestToUserResult.Errors);
        }

        var user = requestToUserResult.Value;

        ErrorOr<Created> createUserResult = _userService.CreateUser(user);

        return createUserResult.Match(
            created => CreatedAtGetUser(user),
            errors => Problem(errors)
        );
    }

    [HttpPost("user/createBase/"), AllowAnonymous]
    public IActionResult CreateBaseUsers()
    {
        var adminUser = UserDb.Create("Admin", "Admin", "Admin").Value;
        var agentUser = UserDb.Create("Agent", "Agent", "Agent").Value;
        var visitorUser = UserDb.Create("Visitor", "Visitor", "Visitor").Value;

        _userService.CreateUser(adminUser);
        _userService.CreateUser(agentUser);
        _userService.CreateUser(visitorUser);

        return Ok();
    }

    [HttpPost("user/login"), AllowAnonymous]
    public IActionResult LoginUser(UserDto request)
    {
        ErrorOr<UserDb> loginSuccess = _userService.LoginUser(request);

        return loginSuccess.Match(
            success => Ok(MapLoginUserResponse(loginSuccess.Value)),
            errors => Problem(errors)
        );
    }

    [HttpGet("user/{username}")]
    public IActionResult GetUser(string username)
    {
        ErrorOr<UserDb> getUserResult = _userService.GetUser(username);

        return getUserResult.Match(
            user => Ok(MapUserResponse(user)),
            errors => Problem(errors));
    }

    private static UserResponse MapUserResponse(UserDb user)
    {
        return new UserResponse(
                    user.Username,
                    user.Role);
    }

    private UserLoginResponse MapLoginUserResponse(UserDb user)
    {
        return new UserLoginResponse(
                    user.Username,
                    CreateToken(user),
                    user.Role);
    }

    private CreatedAtActionResult CreatedAtGetUser(UserDb user)
    {
        return CreatedAtAction(
            actionName: nameof(GetUser),
            routeValues: new { username = user.Username },
            value: MapUserResponse(user));
    }

    private string CreateToken(UserDb user)
    {
        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: cred);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}