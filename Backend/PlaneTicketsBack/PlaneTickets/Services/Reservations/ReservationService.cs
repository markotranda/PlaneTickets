using ErrorOr;
using PlaneTickets.Contracts.Reservation;
using PlaneTickets.Models;
using PlaneTickets.Persistence;
using PlaneTickets.ServiceErrors;

namespace PlaneTickets.Services.Reservations;

public class ReservationService : IReservationService
{
    private readonly PlaneTicketsDbContext _dbContext;

    public ReservationService(PlaneTicketsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public ErrorOr<Created> CreateReservation(Reservation reservation)
    {
        _dbContext.Add(reservation);
        _dbContext.SaveChanges();

        return Result.Created;
    }

    public ErrorOr<Deleted> DeleteReservation(Guid id)
    {
        var reservation = _dbContext.Reservations.Find(id);
        if (reservation is null)
        {
            return Errors.Reservation.NotFound;
        }

        _dbContext.Remove(reservation);
        _dbContext.SaveChanges();

        return Result.Deleted;
    }

    public ErrorOr<Reservation> GetReservation(Guid id)
    {
        if (_dbContext.Reservations.Find(id) is Reservation reservation)
        {
            return reservation;
        }
        else
        {
            return Errors.Reservation.NotFound;
        }
    }

    public ErrorOr<List<Reservation>> GetReservations(GetReservationsRequest request, string? username, string? role)
    {
        var query = _dbContext.Reservations.AsQueryable();
        if (request.Id != null) { query = query.Where(f => f.Id == request.Id); }
        if (request.FlightId != null) { query = query.Where(f => f.FlightId == request.FlightId); }
        if (username != null && role == "Visitor") { query = query.Where(f => f._UserUsername == username); }
        if (request.Tickets != null) { query = query.Where(f => f.Tickets == request.Tickets); }
        if (request.ReservationStatus != null) { query = query.Where(f => f.ReservationStatus == request.ReservationStatus); }
        List<Reservation> reservations = query.ToList();
        reservations.ForEach(SetReservation);
        if (reservations.Count > 0)
        {
            return reservations;
        }
        else
        {
            return Errors.Reservation.NotFound;
        }
    }

    private void SetReservation(Reservation r)
    {
        var flight = _dbContext.Flights.Find(r.FlightId);
        if (flight is not null)
        {
            r.SetFlight(flight);
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
    }

    public ErrorOr<UpsertedReservation> UpsertReservation(Reservation reservation)
    {
        var isNewlyCreated = !_dbContext.Reservations.Any(f => f.Id == reservation.Id);

        if (isNewlyCreated)
        {
            _dbContext.Reservations.Add(reservation);
        }
        else
        {
            _dbContext.Reservations.Update(reservation);
        }
        _dbContext.SaveChanges();

        return new UpsertedReservation(IsNewlyCreated: isNewlyCreated);
    }

    public ErrorOr<Success> CancelReservation(Guid id)
    {
        var reservation = _dbContext.Reservations.Find(id);
        if (reservation is null)
        {
            return Errors.Reservation.NotFound;
        }
        reservation.CancelReservation();
        _dbContext.SaveChanges();
        return Result.Success;
    }

    public ErrorOr<Success> ConfirmReservation(Guid id)
    {

        var reservation = _dbContext.Reservations.Find(id);
        if (reservation is null)
        {
            return Errors.Reservation.NotFound;
        }

        if (_dbContext.Flights.Find(reservation.FlightId) is Flight flight)
        {
            int availableSeats = flight.PassengerNumber - _dbContext.Reservations.Where(f => f.FlightId == reservation.FlightId).Where(f => f.ReservationStatus == ReservationStatus.approved).Sum(f => f.Tickets);
            ErrorOr<bool> confirmResult = reservation.ConfirmReservation(availableSeats);
            if (confirmResult.IsError)
            {
                return confirmResult.Errors;
            }
            _dbContext.SaveChanges();
            return Result.Success;
        }
        else
        {
            return Errors.Flight.NotFound;
        }
    }

}
