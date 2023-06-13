using System.Security.Claims;
using System.Net;
using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlaneTickets.Contracts.Reservation;
using PlaneTickets.Models;
using PlaneTickets.ServiceErrors;
using PlaneTickets.Services.Reservations;
using PlaneTickets.Services.Users;
using PlaneTickets.Services.Flights;

namespace PlaneTickets.Controllers;

public class ReservationController : ApiController
{
    private readonly IReservationService _reservationService;
    private readonly IUserService _userService;
    private readonly IFlightService _flightService;
    public ReservationController(IReservationService reservationService, IUserService userService, IFlightService flightService)
    {
        _reservationService = reservationService;
        _userService = userService;
        _flightService = flightService;
    }


    [HttpPost("reservation/"), Authorize(Roles = "Admin,Visitor")]
    public IActionResult CreateReservation(CreateReservationRequest request)
    {
        ErrorOr<Flight> getFlightResponse = _flightService.GetFlight(request.FlightId);
        if (getFlightResponse.IsError)
        {
            return Problem(getFlightResponse.Errors);
        }
        ErrorOr<Reservation> requestToReservationResult = Reservation.From(request, getFlightResponse.Value, _userService.GetUsername(), _flightService.GetAvailableSeats(request.FlightId).Value);

        if (requestToReservationResult.IsError)
        {
            return Problem(requestToReservationResult.Errors);
        }

        var reservation = requestToReservationResult.Value;

        ErrorOr<Created> createReservationResult = _reservationService.CreateReservation(reservation);

        return createReservationResult.Match(
            created => CreatedAtGetReservation(reservation),
            errors => Problem(errors)
        );
    }

    [HttpGet("reservation/{id:guid}"), Authorize(Roles = "Admin,Agent,Visitor")]
    public IActionResult GetReservation(Guid id)
    {
        ErrorOr<Reservation> getReservationResult = _reservationService.GetReservation(id);

        return getReservationResult.Match(
            reservation => Ok(MapReservationResponseDetailed(reservation)),
            errors => Problem(errors));
    }

    [HttpGet("reservation/"), Authorize(Roles = "Admin,Agent,Visitor")]
    public IActionResult GetReservations([FromQuery] GetReservationsRequest request)
    {
        ErrorOr<List<Reservation>> getReservationResult = _reservationService.GetReservations(request, _userService.GetUsername(), _userService.GetRole());

        List<ReservationResponse> reservationResponses = new();
        if (!getReservationResult.IsError)
        {
            getReservationResult.Value.ForEach(f => reservationResponses.Add(MapReservationResponse(f)));
        }

        return getReservationResult.Match(
            reservations => Ok(reservationResponses),
            errors => Problem(errors));
    }

    [HttpGet("reservation/detail"), Authorize(Roles = "Admin,Agent,Visitor")]
    public IActionResult GetReservationsDetailed([FromQuery] GetReservationsRequest request)
    {
        ErrorOr<List<Reservation>> getReservationResult = _reservationService.GetReservations(request, _userService.GetUsername(), _userService.GetRole());

        List<ReservationResponseDetailed> reservationResponses = new();
        if (!getReservationResult.IsError)
        {
            getReservationResult.Value.ForEach(f => reservationResponses.Add(MapReservationResponseDetailed(f)));
        }

        return getReservationResult.Match(
            reservations => Ok(reservationResponses),
            errors => Problem(errors));
    }

    [HttpPut("reservation/{id:guid}"), Authorize(Roles = "Admin")]
    public IActionResult UpsertReservation(Guid id, UpsertReservationRequest request)
    {
        ErrorOr<Flight> getFlightResponse = _flightService.GetFlight(request.FlightId);
        if (getFlightResponse.IsError)
        {
            return Problem(getFlightResponse.Errors);
        }
        ErrorOr<Reservation> requestToReservationResult = Reservation.From(id, request, getFlightResponse.Value, _userService.GetUsername(), _flightService.GetAvailableSeats(request.FlightId).Value);

        if (requestToReservationResult.IsError)
        {
            return Problem(requestToReservationResult.Errors);
        }

        var reservation = requestToReservationResult.Value;

        ErrorOr<UpsertedReservation> upsertReservationResult = _reservationService.UpsertReservation(reservation);

        return upsertReservationResult.Match(
            upserted => upserted.IsNewlyCreated ? CreatedAtGetReservation(reservation) : NoContent(),
            errors => Problem(errors));
    }

    [HttpDelete("reservation/{id:guid}"), Authorize(Roles = "Admin")]
    public IActionResult DeleteReservation(Guid id)
    {
        ErrorOr<Deleted> deleteReservationResult = _reservationService.DeleteReservation(id);

        return deleteReservationResult.Match(
            deleted => NoContent(),
            errors => Problem(errors));
    }

    [HttpPost("reservation/cancel/{id:guid}"), Authorize(Roles = "Admin,Agent")]
    public IActionResult CancelReservation(Guid id)
    {
        ErrorOr<Success> cancelledReservationResult = _reservationService.CancelReservation(id);

        return cancelledReservationResult.Match(
            success => NoContent(),
            errors => Problem(errors));
    }

    [HttpPost("reservation/confirm/{id:guid}"), Authorize(Roles = "Admin,Agent")]
    public IActionResult ConfirmReservation(Guid id)
    {
        ErrorOr<Success> confirmedReservationResult = _reservationService.ConfirmReservation(id);

        return confirmedReservationResult.Match(
            success => NoContent(),
            errors => Problem(errors));
    }

    private static ReservationResponse MapReservationResponse(Reservation reservation)
    {
        return new ReservationResponse(
                    reservation.Id,
                    reservation.FlightId,
                    reservation._UserUsername,
                    reservation.Tickets,
                    reservation.ReservationStatus
                    );
    }

    private static ReservationResponseDetailed MapReservationResponseDetailed(Reservation reservation)
    {
        return new ReservationResponseDetailed(
                    reservation.Id,
                    reservation.FlightId,
                    reservation._UserUsername,
                    reservation.Tickets,
                    reservation.ReservationStatus,
                    reservation._Flight.AirportDeparturePlace.Name,
                    reservation._Flight.AirportArrivalPlace.Name,
                    reservation._Flight.DepartureDateTime,
                    reservation._Flight.ArrivalDateTime,
                    reservation._Flight.Transfers,
                    reservation._Flight.PassengerNumber,
                    reservation._Flight.FlightStatus
                    );
    }

    private CreatedAtActionResult CreatedAtGetReservation(Reservation reservation)
    {
        return CreatedAtAction(
            actionName: nameof(GetReservation),
            routeValues: new { id = reservation.Id },
            value: MapReservationResponse(reservation));
    }
}