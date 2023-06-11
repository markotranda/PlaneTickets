namespace PlaneTickets.Contracts.User;

public record UserLoginResponse(
    string Username,
    string Token,
    string Role
);