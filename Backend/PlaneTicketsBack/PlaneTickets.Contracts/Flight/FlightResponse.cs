namespace PlaneTickets.Contracts.Flight;

public record FlightResponse(
    Guid Id,
    Guid DeparturePlaceId,
    string DeparturePlace,
    Guid ArrivalPlaceId,
    string ArrivalPlace,
    DateTime DepartureDateTime,
    DateTime ArrivalDateTime,
    int Transfers,
    int PassengerNumber,
    FlightStatus Status
);