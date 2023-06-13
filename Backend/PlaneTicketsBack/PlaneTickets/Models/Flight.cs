using ErrorOr;
using PlaneTickets.Contracts.Flight;
using PlaneTickets.ServiceErrors;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlaneTickets.Models;

public class Flight
{
    public const int MinDeparturePlaceLength = 3;
    public const int MaxDeparturePlaceLength = 150;
    public Guid Id { get; private set; }
    public Guid AirportDeparturePlaceId { get; private set; }
    public virtual Airport AirportDeparturePlace { get; private set; }
    public Guid AirportArrivalPlaceId { get; private set; }
    public virtual Airport AirportArrivalPlace { get; private set; }
    public DateTime DepartureDateTime { get; private set; }
    public DateTime ArrivalDateTime { get; private set; }
    public int Transfers { get; private set; }
    public int PassengerNumber { get; private set; }
    public FlightStatus FlightStatus { get; private set; }

    private Flight() { }

    private Flight(
        Guid id,
        Guid departurePlaceId,
        Guid arrivalPlaceId,
        DateTime departureDateTime,
        DateTime arrivalDateTime,
        int transfers,
        int passengerNumber,
        FlightStatus status)
    {
        Id = id;
        AirportDeparturePlaceId = departurePlaceId;
        AirportArrivalPlaceId = arrivalPlaceId;
        DepartureDateTime = departureDateTime;
        ArrivalDateTime = arrivalDateTime;
        Transfers = transfers;
        PassengerNumber = passengerNumber;
        FlightStatus = status;
    }

    public static ErrorOr<Flight> Create(
        Guid departurePlaceId,
        Guid arrivalPlaceId,
        DateTime departureDateTime,
        DateTime arrivalDateTime,
        int transfers,
        int passengerNumber,
        Guid? id = null)
    {
        List<Error> errors = new();
        if (arrivalPlaceId == departurePlaceId)
        {
            errors.Add(Errors.Flight.InvalidDeparturePlace);
            errors.Add(Errors.Flight.InvalidArrivalPlace);
        }
        if (transfers < 0)
        {
            errors.Add(Errors.Flight.NegativeTransfers);
        }
        if (passengerNumber < 0)
        {
            errors.Add(Errors.Flight.NegativePassengerNumber);
        }
        if (arrivalDateTime <= departureDateTime)
        {
            errors.Add(Errors.Flight.InvalidArrivalDateTime);
        }

        if (errors.Count > 0)
        {
            return errors;
        }
        return new Flight(
            id ?? Guid.NewGuid(),
            departurePlaceId,
            arrivalPlaceId,
            departureDateTime,
            arrivalDateTime,
            transfers,
            passengerNumber,
            FlightStatus.new_);
    }

    public void CancelFlight()
    {
        FlightStatus = FlightStatus.canceled;
    }

    public static ErrorOr<Flight> From(CreateFlightRequest request)
    {
        return Create(
            request.DeparturePlaceId,
            request.ArrivalPlaceId,
            request.DepartureDateTime,
            request.ArrivalDateTime,
            request.Transfers,
            request.PassengerNumber);
    }

    public static ErrorOr<Flight> From(Guid id, UpsertFlightRequest request)
    {
        return Create(
            request.DeparturePlaceId,
            request.ArrivalPlaceId,
            request.DepartureDateTime,
            request.ArrivalDateTime,
            request.Transfers,
            request.PassengerNumber,
            id);
    }
}