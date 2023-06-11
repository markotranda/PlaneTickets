using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using PlaneTickets.Contracts.Airport;
using PlaneTickets.Models;

namespace PlaneTickets.Services.Airports;

public interface IAirportService
{
    ErrorOr<Created> CreateAirport(Airport airport);
    ErrorOr<Deleted> DeleteAirport(Guid id);
    ErrorOr<Airport> GetAirport(Guid id);
    ErrorOr<List<Airport>> GetAirports(GetAirportsRequest request, string role);
    ErrorOr<UpsertedAirport> UpsertAirport(Airport airport);
}