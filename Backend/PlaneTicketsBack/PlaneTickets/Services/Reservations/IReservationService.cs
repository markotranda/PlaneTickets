using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using PlaneTickets.Contracts.Reservation;
using PlaneTickets.Models;

namespace PlaneTickets.Services.Reservations;

public interface IReservationService
{
    ErrorOr<Success> CancelReservation(Guid id);
    ErrorOr<Success> ConfirmReservation(Guid id);
    ErrorOr<Created> CreateReservation(Reservation reservation);
    ErrorOr<Deleted> DeleteReservation(Guid id);
    ErrorOr<Reservation> GetReservation(Guid id);
    ErrorOr<List<Reservation>> GetReservations(GetReservationsRequest request, string? username, string? role);
    ErrorOr<UpsertedReservation> UpsertReservation(Reservation reservation);
}