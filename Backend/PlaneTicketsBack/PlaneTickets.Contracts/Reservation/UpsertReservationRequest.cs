namespace PlaneTickets.Contracts.Reservation;

public record UpsertReservationRequest(
    Guid FlightId,
    int Tickets,
    ReservationStatus ReservationStatus
);