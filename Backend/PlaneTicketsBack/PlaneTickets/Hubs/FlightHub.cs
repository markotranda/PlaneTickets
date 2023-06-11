using Microsoft.AspNetCore.SignalR;

namespace PlaneTickets.Hubs;

public class FlightHub : Hub
{
    public async Task RefreshAllClients()
    {
        await Clients.All.SendAsync("Refresh");
    }
}