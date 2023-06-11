namespace PlaneTickets.Contracts.Reservation;

public record ReservationResponse(
    Guid Id,
    Guid FlightId,
    string Username,
    int Tickets,
    ReservationStatus ReservationStatus
);