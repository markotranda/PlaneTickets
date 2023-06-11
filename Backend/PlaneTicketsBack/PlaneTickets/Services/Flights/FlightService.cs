using ErrorOr;
using PlaneTickets.Contracts.Flight;
using PlaneTickets.Contracts.Reservation;
using PlaneTickets.Models;
using PlaneTickets.Persistence;
using PlaneTickets.ServiceErrors;

namespace PlaneTickets.Services.Flights;

public class FlightService : IFlightService
{
    private readonly PlaneTicketsDbContext _dbContext;

    public FlightService(PlaneTicketsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public ErrorOr<Created> CreateFlight(Flight flight)
    {
        SetAirports(flight);
        _dbContext.Add(flight);
        _dbContext.SaveChanges();

        return Result.Created;
    }

    private void SetAirports(Flight flight)
    {
        var departurePlace = _dbContext.Airports.Find(flight.AirportDeparturePlaceId);
        if (departurePlace is not null)
        {
            flight.SetDeparturePlace(departurePlace);
        }
        var arrivalPlace = _dbContext.Airports.Find(flight.AirportArrivalPlaceId);
        if (arrivalPlace is not null)
        {
            flight.SetArrivalPlace(arrivalPlace);
        }
    }

    public ErrorOr<Deleted> DeleteFlight(Guid id)
    {
        var flight = _dbContext.Flights.Find(id);
        if (flight is null)
        {
            return Errors.Flight.NotFound;
        }

        _dbContext.Remove(flight);
        _dbContext.SaveChanges();

        return Result.Deleted;
    }

    public ErrorOr<Flight> GetFlight(Guid id)
    {
        if (_dbContext.Flights.Find(id) is Flight flight)
        {
            SetAirports(flight);
            return flight;
        }
        else
        {
            return Errors.Flight.NotFound;
        }
    }

    public ErrorOr<List<Flight>> GetFlights(GetFlightsRequest request, string role)
    {
        var query = _dbContext.Flights.AsQueryable();
        if (request.Id != null) { query = query.Where(f => f.Id == request.Id); }
        if (request.DeparturePlaceId != null) { query = query.Where(f => f.AirportDeparturePlaceId == request.DeparturePlaceId); }
        if (request.ArrivalPlaceId != null) { query = query.Where(f => f.AirportArrivalPlaceId == request.ArrivalPlaceId); }
        if (request.DepartureDateTime != null) { query = query.Where(f => f.DepartureDateTime == request.DepartureDateTime); }
        if (request.ArrivalDateTime != null) { query = query.Where(f => f.ArrivalDateTime == request.ArrivalDateTime); }
        if (request.Transfers != null) { query = query.Where(f => f.Transfers == request.Transfers); }
        if (request.PassengerNumber != null) { query = query.Where(f => f.PassengerNumber == request.PassengerNumber); }
        if (request.FlightStatus != null) { query = query.Where(f => f.FlightStatus == request.FlightStatus); }
        if (role == "Visitor") { query = query.Where(f => f.FlightStatus == FlightStatus.new_); }
        List<Flight> flights = query.ToList();
        flights.ForEach(SetAirports);
        if (flights.Count > 0)
        {
            return flights;
        }
        else
        {
            return Errors.Flight.NotFound;
        }
    }

    public ErrorOr<UpsertedFlight> UpsertFlight(Flight flight)
    {
        var isNewlyCreated = !_dbContext.Flights.Any(f => f.Id == flight.Id);

        if (isNewlyCreated)
        {
            _dbContext.Flights.Add(flight);
        }
        else
        {
            _dbContext.Flights.Update(flight);
        }
        _dbContext.SaveChanges();

        return new UpsertedFlight(IsNewlyCreated: isNewlyCreated);
    }

    public ErrorOr<Success> CancelFlight(Guid id)
    {
        var flight = _dbContext.Flights.Find(id);
        if (flight is null)
        {
            return Errors.Flight.NotFound;
        }
        flight.CancelFlight();
        _dbContext.SaveChanges();
        return Result.Success;
    }

    public ErrorOr<int> GetAvailableSeats(Guid FlightId)
    {
        if (_dbContext.Flights.Find(FlightId) is Flight flight)
        {
            return flight.PassengerNumber - _dbContext.Reservations.Where(f => f.FlightId == FlightId).Where(f => f.ReservationStatus == ReservationStatus.approved).Sum(f => f.Tickets);
        }
        else
        {
            return Errors.Flight.NotFound;
        }
    }
}
