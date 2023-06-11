using ErrorOr;
using PlaneTickets.Contracts.Airport;
using PlaneTickets.Contracts.Reservation;
using PlaneTickets.Models;
using PlaneTickets.Persistence;
using PlaneTickets.ServiceErrors;

namespace PlaneTickets.Services.Airports;

public class AirportService : IAirportService
{
    private readonly PlaneTicketsDbContext _dbContext;

    public AirportService(PlaneTicketsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public ErrorOr<Created> CreateAirport(Airport airport)
    {
        _dbContext.Add(airport);
        _dbContext.SaveChanges();

        return Result.Created;
    }

    public ErrorOr<Deleted> DeleteAirport(Guid id)
    {
        var airport = _dbContext.Airports.Find(id);
        if (airport is null)
        {
            return Errors.Airport.NotFound;
        }

        _dbContext.Remove(airport);
        _dbContext.SaveChanges();

        return Result.Deleted;
    }

    public ErrorOr<Airport> GetAirport(Guid id)
    {
        if (_dbContext.Airports.Find(id) is Airport airport)
        {
            return airport;
        }
        else
        {
            return Errors.Airport.NotFound;
        }
    }

    public ErrorOr<List<Airport>> GetAirports(GetAirportsRequest request, string role)
    {
        var query = _dbContext.Airports.AsQueryable();
        if (request.Id != null) { query = query.Where(f => f.Id == request.Id); }
        if (request.Name != null) { query = query.Where(f => f.Name == request.Name); }
        List<Airport> airports = query.ToList();
        if (airports.Count > 0)
        {
            return airports;
        }
        else
        {
            return Errors.Airport.NotFound;
        }
    }

    public ErrorOr<UpsertedAirport> UpsertAirport(Airport airport)
    {
        var isNewlyCreated = !_dbContext.Airports.Any(f => f.Id == airport.Id);

        if (isNewlyCreated)
        {
            _dbContext.Airports.Add(airport);
        }
        else
        {
            _dbContext.Airports.Update(airport);
        }
        _dbContext.SaveChanges();

        return new UpsertedAirport(IsNewlyCreated: isNewlyCreated);
    }
}
