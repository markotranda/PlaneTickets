using ErrorOr;
using PlaneTickets.Contracts.Airport;
using PlaneTickets.ServiceErrors;

namespace PlaneTickets.Models;

public class Airport
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    private Airport() { }

    private Airport(
        Guid id,
        string name)
    {
        Id = id;
        Name = name;
    }

    public static ErrorOr<Airport> Create(
        string name,
        Guid? id = null)
    {
        List<Error> errors = new();

        if (errors.Count > 0)
        {
            return errors;
        }
        return new Airport(
            id ?? Guid.NewGuid(),
            name);
    }

    public static ErrorOr<Airport> From(CreateAirportRequest request)
    {
        return Create(
            request.Name);
    }

    public static ErrorOr<Airport> From(Guid id, UpsertAirportRequest request)
    {
        return Create(
            request.Name,
            id);
    }
}