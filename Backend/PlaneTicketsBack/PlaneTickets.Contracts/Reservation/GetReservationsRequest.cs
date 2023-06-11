using System.Data.Common;
namespace PlaneTickets.Contracts.Reservation;

public record GetReservationsRequest(
    Guid? Id,
    Guid? FlightId,
    int? Tickets,
    ReservationStatus? ReservationStatus
);