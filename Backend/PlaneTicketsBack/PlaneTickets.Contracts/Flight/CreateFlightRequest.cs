namespace PlaneTickets.Contracts.Flight;

public record CreateFlightRequest(
    Guid DeparturePlaceId,
    Guid ArrivalPlaceId,
    DateTime DepartureDateTime,
    DateTime ArrivalDateTime,
    int Transfers,
    int PassengerNumber
);