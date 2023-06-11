using PlaneTickets.Contracts.Flight;

namespace PlaneTickets.Contracts.Reservation;

public record ReservationResponseDetailed(
    Guid Id,
    Guid FlightId,
    string Username,
    int Tickets,
    ReservationStatus ReservationStatus,
    string DeparturePlace,
    string ArrivalPlace,
    DateTime DepartureDateTime,
    DateTime ArrivalDateTime,
    int Transfers,
    int PassengerNumber,
    FlightStatus FlightStatus
);