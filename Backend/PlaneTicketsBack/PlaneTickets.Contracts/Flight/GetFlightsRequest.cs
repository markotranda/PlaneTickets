namespace PlaneTickets.Contracts.Flight;

public record GetFlightsRequest(
    Guid? Id,
    Guid? DeparturePlaceId,
    Guid? ArrivalPlaceId,
    DateTime? DepartureDateTime,
    DateTime? ArrivalDateTime,
    int? Transfers,
    int? PassengerNumber,
    FlightStatus? FlightStatus
);