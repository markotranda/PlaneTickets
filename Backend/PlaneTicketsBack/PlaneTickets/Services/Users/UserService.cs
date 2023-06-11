using System.Text;
using System.Security.Claims;
using ErrorOr;
using Microsoft.IdentityModel.Tokens;
using PlaneTickets.Contracts.User;
using PlaneTickets.Models;
using PlaneTickets.Persistence;
using PlaneTickets.ServiceErrors;

namespace PlaneTickets.Services.Users;

public class UserService : IUserService
{
    private readonly PlaneTicketsDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(PlaneTicketsDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public ErrorOr<Created> CreateUser(UserDb user)
    {
        var u = _dbContext.Users.Find(user.Username);
        if (u is null)
        {
            _dbContext.Add(user);
            _dbContext.SaveChanges();
            return Result.Created;
        }
        else
        {
            return Errors.User.AlreadyExists;
        }

    }

    public ErrorOr<UserDb> GetUser(string username)
    {
        if (_dbContext.Users.Find(username) is UserDb user)
        {
            return user;
        }
        else
        {
            return Errors.User.NotFound;
        }
    }

    public ErrorOr<UserDb> LoginUser(UserDto request)
    {
        if (_dbContext.Users.Find(request.Username) is UserDb user)
        {
            if (user.VerifyPasswordHash(request.Password) == Result.Success)
            {
                return user;
            }
            else
            {
                return Errors.User.InvalidPassword;
            }
        }
        else
        {
            return Errors.User.InvalidUsername;
        }
    }

    public string GetUsername()
    {
        var result = string.Empty;
        if (_httpContextAccessor.HttpContext != null)
        {
            result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        }
        return result;
    }

    public string GetRole()
    {
        var result = string.Empty;
        if (_httpContextAccessor.HttpContext != null)
        {
            result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
        }
        return result;
    }
}
