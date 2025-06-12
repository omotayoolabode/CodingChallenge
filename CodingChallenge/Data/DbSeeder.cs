using CodingChallenge.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Route = CodingChallenge.Models.Entities.Route;

namespace CodingChallenge.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context, IServiceProvider serviceProvider)
    {
        // Seed Routes
        if (!context.Routes.Any())
        {
            var routesJson = await File.ReadAllTextAsync(Path.Combine("Sample Data", "routes.json"));
            var routes = JsonConvert.DeserializeObject<List<RouteSeedModel>>(routesJson);
            if (routes != null)
            {
                var routeEntities = routes.Select(r => new Route
                {
                    RouteId = r.route_id,
                    From = r.from,
                    To = r.to,
                    Duration = r.duration
                }).ToList();
                context.Routes.AddRange(routeEntities);
                await context.SaveChangesAsync();
            }
        }

        // Seed Flights
        if (!context.Flights.Any())
        {
            var flightsJson = await File.ReadAllTextAsync(Path.Combine("Sample Data", "flights.json"));
            var flights = JsonConvert.DeserializeObject<List<FlightSeedModel>>(flightsJson);
            if (flights != null)
            {
                var flightEntities = flights.Select(f => new Flight
                {
                    RouteId = f.route,
                    Provider = f.provider,
                    Price = f.price,
                    Departure = ParseDeparture(f.departure)
                }).ToList();
                context.Flights.AddRange(flightEntities);
                await context.SaveChangesAsync();
            }
        }
    }

    private static DateTime ParseDeparture(string departure)
    {
        // Example: "+6 day 12 hour"
        var now = DateTime.UtcNow;
        var parts = departure.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        int days = 0, hours = 0;
        for (int i = 0; i < parts.Length - 1; i += 2)
        {
            if (parts[i + 1].StartsWith("day"))
                days = int.Parse(parts[i].Replace("+", ""));
            else if (parts[i + 1].StartsWith("hour"))
                hours = int.Parse(parts[i]);
        }
        return now.AddDays(days).AddHours(hours);
    }

    private class RouteSeedModel
    {
        public string route_id { get; set; } = string.Empty;
        public string from { get; set; } = string.Empty;
        public string to { get; set; } = string.Empty;
        public int duration { get; set; }
    }

    private class FlightSeedModel
    {
        public int flight_id { get; set; }
        public string route { get; set; } = string.Empty;
        public string provider { get; set; } = string.Empty;
        public decimal price { get; set; }
        public string departure { get; set; } = string.Empty;
    }
} 