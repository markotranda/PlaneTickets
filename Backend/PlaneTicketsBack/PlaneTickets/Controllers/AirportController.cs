using System.Net;
using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PlaneTickets.Contracts.Airport;
using PlaneTickets.Hubs;
using PlaneTickets.Models;
using PlaneTickets.ServiceErrors;
using PlaneTickets.Services.Airports;
using PlaneTickets.Services.Users;

namespace PlaneTickets.Controllers;

public class AirportController : ApiController
{
    private readonly IAirportService _airportService;
    private readonly IUserService _userService;
    public AirportController(IAirportService airportService, IUserService userService)
    {
        _airportService = airportService;
        _userService = userService;
    }

    [HttpPost("airport/"), Authorize(Roles = "Admin,Agent")]
    public IActionResult CreateAirport(CreateAirportRequest request)
    {
        ErrorOr<Airport> requestToAirportResult = Airport.From(request);

        if (requestToAirportResult.IsError)
        {
            return Problem(requestToAirportResult.Errors);
        }

        var airport = requestToAirportResult.Value;

        ErrorOr<Created> createAirportResult = _airportService.CreateAirport(airport);


        return createAirportResult.Match(
            created => CreatedAtGetAirport(airport),
            errors => Problem(errors)
        );
    }

    [HttpGet("airport/{id:guid}"), Authorize(Roles = "Admin,Agent,Visitor")]
    public IActionResult GetAirport(Guid id)
    {
        ErrorOr<Airport> getAirportResult = _airportService.GetAirport(id);

        return getAirportResult.Match(
            airport => Ok(MapAirportResponse(airport)),
            errors => Problem(errors));
    }

    [HttpGet("airport/"), Authorize(Roles = "Admin,Agent,Visitor")]
    public IActionResult GetAirports([FromQuery] GetAirportsRequest request)
    {
        ErrorOr<List<Airport>> getAirportResult = _airportService.GetAirports(request, _userService.GetRole());

        List<AirportResponse> airportResponses = new();
        if (!getAirportResult.IsError)
        {
            getAirportResult.Value.ForEach(f => airportResponses.Add(MapAirportResponse(f)));
        }

        return getAirportResult.Match(
            airports => Ok(airportResponses),
            errors => Problem(errors));
    }

    [HttpPut("airport/{id:guid}"), Authorize(Roles = "Admin")]
    public IActionResult UpsertAirport(Guid id, UpsertAirportRequest request)
    {
        ErrorOr<Airport> requestToAirportResult = Airport.From(id, request);

        if (requestToAirportResult.IsError)
        {
            return Problem(requestToAirportResult.Errors);
        }

        var airport = requestToAirportResult.Value;

        ErrorOr<UpsertedAirport> upsertAirportResult = _airportService.UpsertAirport(airport);

        return upsertAirportResult.Match(
            upserted => upserted.IsNewlyCreated ? CreatedAtGetAirport(airport) : NoContent(),
            errors => Problem(errors));
    }

    [HttpDelete("airport/{id:guid}"), Authorize(Roles = "Admin")]
    public IActionResult DeleteAirport(Guid id)
    {
        ErrorOr<Deleted> deleteAirportResult = _airportService.DeleteAirport(id);

        return deleteAirportResult.Match(
            deleted => NoContent(),
            errors => Problem(errors));
    }

    private static AirportResponse MapAirportResponse(Airport airport)
    {
        return new AirportResponse(
                    airport.Id,
                    airport.Name);
    }

    private CreatedAtActionResult CreatedAtGetAirport(Airport airport)
    {
        return CreatedAtAction(
            actionName: nameof(GetAirport),
            routeValues: new { id = airport.Id },
            value: MapAirportResponse(airport));
    }
}