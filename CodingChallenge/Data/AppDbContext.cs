using CodingChallenge.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CodingChallenge.Data;

public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    

    public DbSet<Flight> Flights { get; set; }
    public DbSet<Journey> Journeys { get; set; }
    public DbSet<JourneyFlight> JourneyFlights { get; set; }
    public DbSet<CodingChallenge.Models.Entities.Route> Routes { get; set; }
}
