using Microsoft.EntityFrameworkCore;
using PlaneTickets.Models;

namespace PlaneTickets.Persistence;

public class PlaneTicketsDbContext : DbContext
{
    public PlaneTicketsDbContext(DbContextOptions<PlaneTicketsDbContext> options) : base(options)
    {
    }

    public DbSet<Airport> Airports { get; set; } = null!;
    public DbSet<Flight> Flights { get; set; } = null!;
    public DbSet<UserDb> Users { get; set; } = null!;
    public DbSet<Reservation> Reservations { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PlaneTicketsDbContext).Assembly);
    }
}