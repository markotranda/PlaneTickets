using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using PlaneTickets.Contracts.User;
using PlaneTickets.Models;

namespace PlaneTickets.Services.Users;

public interface IUserService
{
    ErrorOr<Created> CreateUser(UserDb user);
    ErrorOr<UserDb> LoginUser(UserDto user);
    ErrorOr<UserDb> GetUser(string username);
    string GetUsername();
    string GetRole();
}