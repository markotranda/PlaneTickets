using System.Security.Cryptography;
using System.Text;
using ErrorOr;
using PlaneTickets.Contracts.User;
using PlaneTickets.ServiceErrors;

namespace PlaneTickets.Models;

public class UserDb
{
    public string Username {get; private set;}
    public byte[] PasswordHash {get; private set;}
    public byte[] PasswordSalt {get; private set;}
    public string Role { get; private set; }

    private UserDb(){}

    private UserDb(
        string username,
        byte[] passwordHash,
        byte[] passwordSalt,
        string role)
    {
        Username = username;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
        Role = role;
    }

    public static ErrorOr<UserDb> Create(
        string username,
        string password,
        string role)
    {
        List<Error> errors = new();

        CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

        if (errors.Count > 0)
        {
            return errors;
        }
        return new UserDb(
            username,
            passwordHash,
            passwordSalt,
            role);
    }

    public static ErrorOr<UserDb> From(UserDto request)
    {
        return Create(
            request.Username,
            request.Password,
            request.Role);
    }

    private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }

    public ErrorOr<Success> VerifyPasswordHash(string password)
    {
        using (var hmac = new HMACSHA512(PasswordSalt))
        {
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            if (computedHash.SequenceEqual(PasswordHash))
            {
                return Result.Success;
            }
            else
            {
                return Errors.User.InvalidPassword;
            }
        }
    }
}