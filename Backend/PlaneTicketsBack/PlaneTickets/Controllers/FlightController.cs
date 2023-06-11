using System.Net;
using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PlaneTickets.Contracts.Flight;
using PlaneTickets.Hubs;
using PlaneTickets.Models;
using PlaneTickets.ServiceErrors;
using PlaneTickets.Services.Airports;
using PlaneTickets.Services.Flights;
using PlaneTickets.Services.Users;

namespace PlaneTickets.Controllers;

public class FlightController : ApiController
{
    private readonly IFlightService _flightService;
    private readonly IUserService _userService;
    private readonly IHubContext<FlightHub> _flightHubContext;
    private readonly IAirportService _airportService;

    public FlightController(IFlightService flightService, IUserService userService, IHubContext<FlightHub> flightHubContext, IAirportService airportService)
    {
        _flightService = flightService;
        _userService = userService;
        _flightHubContext = flightHubContext;
        _airportService = airportService;
    }

    [HttpPost("flight/"), Authorize(Roles = "Admin,Agent")]
    public IActionResult CreateFlight(CreateFlightRequest request)
    {
        ErrorOr<Flight> requestToFlightResult = Flight.From(request);

        if (requestToFlightResult.IsError)
        {
            return Problem(requestToFlightResult.Errors);
        }

        var flight = requestToFlightResult.Value;

        ErrorOr<Created> createFlightResult = _flightService.CreateFlight(flight);


        return createFlightResult.Match(
            created =>
            {
                _flightHubContext.Clients.All.SendAsync("Refresh");
                return CreatedAtGetFlight(flight);
            },
            errors => Problem(errors)
        );
    }

    [HttpGet("flight/availableSeats/{id:guid}"), Authorize(Roles = "Admin,Agent,Visitor")]
    public IActionResult GetAvailableSeats(Guid id)
    {
        ErrorOr<int> getFlightResult = _flightService.GetAvailableSeats(id);

        return getFlightResult.Match(
            num => Ok(num),
            errors => Problem(errors));
    }

    [HttpGet("flight/{id:guid}"), Authorize(Roles = "Admin,Agent,Visitor")]
    public IActionResult GetFlight(Guid id)
    {
        ErrorOr<Flight> getFlightResult = _flightService.GetFlight(id);

        return getFlightResult.Match(
            flight => Ok(MapFlightResponse(flight)),
            errors => Problem(errors));
    }

    [HttpGet("flight/"), Authorize(Roles = "Admin,Agent,Visitor")]
    public IActionResult GetFlights([FromQuery] GetFlightsRequest request)
    {
        ErrorOr<List<Flight>> getFlightResult = _flightService.GetFlights(request, _userService.GetRole());

        List<FlightResponse> flightResponses = new();
        if (!getFlightResult.IsError)
        {
            getFlightResult.Value.ForEach(f => flightResponses.Add(MapFlightResponse(f)));
        }

        return getFlightResult.Match(
            flights => Ok(flightResponses),
            errors => Problem(errors));
    }

    [HttpPut("flight/{id:guid}"), Authorize(Roles = "Admin")]
    public IActionResult UpsertFlight(Guid id, UpsertFlightRequest request)
    {
        ErrorOr<Flight> requestToFlightResult = Flight.From(id, request);

        if (requestToFlightResult.IsError)
        {
            return Problem(requestToFlightResult.Errors);
        }

        var flight = requestToFlightResult.Value;

        ErrorOr<UpsertedFlight> upsertFlightResult = _flightService.UpsertFlight(flight);

        return upsertFlightResult.Match(
            upserted => upserted.IsNewlyCreated ? CreatedAtGetFlight(flight) : NoContent(),
            errors => Problem(errors));
    }

    [HttpDelete("flight/{id:guid}"), Authorize(Roles = "Admin")]
    public IActionResult DeleteFlight(Guid id)
    {
        ErrorOr<Deleted> deleteFlightResult = _flightService.DeleteFlight(id);

        return deleteFlightResult.Match(
            deleted => NoContent(),
            errors => Problem(errors));
    }

    [HttpPost("flight/cancel/{id:guid}"), Authorize(Roles = "Admin")]
    public IActionResult CancelFlight(Guid id)
    {
        ErrorOr<Success> cancelledFlightResult = _flightService.CancelFlight(id);

        return cancelledFlightResult.Match(
            success => NoContent(),
            errors => Problem(errors));
    }

    private static FlightResponse MapFlightResponse(Flight flight)
    {
        return new FlightResponse(
                    flight.Id,
                    flight.AirportDeparturePlaceId,
                    flight.AirportDeparturePlace.Name,
                    flight.AirportArrivalPlaceId,
                    flight.AirportArrivalPlace.Name,
                    flight.DepartureDateTime,
                    flight.ArrivalDateTime,
                    flight.Transfers,
                    flight.PassengerNumber,
                    flight.FlightStatus);
    }

    private CreatedAtActionResult CreatedAtGetFlight(Flight flight)
    {
        return CreatedAtAction(
            actionName: nameof(GetFlight),
            routeValues: new { id = flight.Id },
            value: MapFlightResponse(flight));
    }
}