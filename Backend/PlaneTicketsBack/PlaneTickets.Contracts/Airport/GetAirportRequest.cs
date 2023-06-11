namespace PlaneTickets.Contracts.Airport;

public record GetAirportsRequest(
    Guid? Id,
    string? Name
);