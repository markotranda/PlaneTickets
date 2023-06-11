namespace PlaneTickets.Contracts.Reservation;

public record CreateReservationRequest(
    Guid FlightId,
    int Tickets
);