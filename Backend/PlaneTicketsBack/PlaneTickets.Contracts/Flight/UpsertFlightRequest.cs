namespace PlaneTickets.Contracts.Flight;

public record UpsertFlightRequest(
    Guid DeparturePlaceId,
    Guid ArrivalPlaceId,
    DateTime DepartureDateTime,
    DateTime ArrivalDateTime,
    int Transfers,
    int PassengerNumber
);