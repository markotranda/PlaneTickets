using ErrorOr;
using PlaneTickets.Contracts.Reservation;
using PlaneTickets.ServiceErrors;

namespace PlaneTickets.Models;

public class Reservation
{
    public Guid Id { get; private set; }
    public Guid FlightId { get; private set; }
    public virtual Flight _Flight { get; private set; }
    public string _UserUsername { get; private set; }
    public virtual UserDb _User { get; private set; }
    public int Tickets { get; private set; }
    public ReservationStatus ReservationStatus { get; private set; }

    private Reservation() { }

    private Reservation(
        Guid id,
        Guid flightId,
        string username,
        int tickets,
        ReservationStatus reservationStatus)
    {
        Id = id;
        FlightId = flightId;
        _UserUsername = username;
        Tickets = tickets;
        ReservationStatus = reservationStatus;
    }

    public static ErrorOr<Reservation> Create(
        Guid flightId,
        Flight flight,
        string username,
        int tickets,
        int availableSeats,
        Guid? id = null)
    {
        List<Error> errors = new();

        if (DateTime.Now.AddDays(3) >= flight.DepartureDateTime)
        {
            errors.Add(Errors.Reservation.LateReservation);
        }
        if (tickets <= 0)
        {
            errors.Add(Errors.Reservation.InvalidTickets);
        }
        if (tickets > availableSeats)
        {
            errors.Add(Errors.Reservation.LowAvailableTickets);
        }

        if (errors.Count > 0)
        {
            return errors;
        }
        return new Reservation(
            id ?? Guid.NewGuid(),
            flightId,
            username,
            tickets,
            ReservationStatus.pending);
    }

    public void CancelReservation()
    {
        ReservationStatus = ReservationStatus.canceled;
    }

    public bool SetFlight(Flight flight)
    {
        _Flight = flight;
        return true;
    }

    public ErrorOr<bool> ConfirmReservation(int availableSeats)
    {
        List<Error> errors = new();
        ReservationStatus = ReservationStatus.approved;
        if (Tickets > availableSeats)
        {
            errors.Add(Errors.Reservation.LowAvailableTickets);
        }

        if (errors.Count > 0)
        {
            return errors;
        }

        return true;
    }

    public static ErrorOr<Reservation> From(CreateReservationRequest request, Flight flight, string username, int availableSeats)
    {
        return Create(
            request.FlightId,
            flight,
            username,
            request.Tickets,
            availableSeats);
    }

    public static ErrorOr<Reservation> From(Guid id, UpsertReservationRequest request, Flight flight, string username, int availableSeats)
    {
        return Create(
            request.FlightId,
            flight,
            username,
            request.Tickets,
            availableSeats,
            id);
    }
}