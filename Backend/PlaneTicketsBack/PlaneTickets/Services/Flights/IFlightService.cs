using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using PlaneTickets.Contracts.Flight;
using PlaneTickets.Models;

namespace PlaneTickets.Services.Flights;

public interface IFlightService
{
    ErrorOr<Success> CancelFlight(Guid id);
    ErrorOr<Created> CreateFlight(Flight flight);
    ErrorOr<Deleted> DeleteFlight(Guid id);
    ErrorOr<int> GetAvailableSeats(Guid id);
    ErrorOr<Flight> GetFlight(Guid id);
    ErrorOr<List<Flight>> GetFlights(GetFlightsRequest request, string role);
    ErrorOr<UpsertedFlight> UpsertFlight(Flight flight);
}