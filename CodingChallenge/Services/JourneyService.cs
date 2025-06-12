using CodingChallenge.Data;
using CodingChallenge.Models.Entities;
using CodingChallenge.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CodingChallenge.Services;

public class JourneyService : IJourneyService
{
    private readonly AppDbContext _context;

    // Simple static cache for demonstration (not thread-safe for production)
    private static List<CodingChallenge.Models.Entities.Route>? cachedRoutes = null;
    private static List<Flight>? cachedFlights = null;

    public JourneyService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Journey>> FindJourneysByMinExchangesAsync(string origin, string destination, bool ascending = true, int? maxExchanges = null, int maxResults = 100)
    {
        if (cachedRoutes == null || cachedFlights == null)
        {
            cachedRoutes = await _context.Routes.Include(r => r.Flights).ToListAsync();
            cachedFlights = await _context.Flights.Include(f => f.Route).ToListAsync();
        }
        var routes = cachedRoutes;
        var flights = cachedFlights;

        // Build a map from location to available flights
        var flightsFrom = flights.GroupBy(f => f.Route.From)
            .ToDictionary(g => g.Key, g => g.ToList());

        var results = new List<Journey>();
        var queue = new Queue<(string current, List<Flight> path, HashSet<int> usedFlightIds, HashSet<string> visitedLocations)>();
        queue.Enqueue((origin, new List<Flight>(), new HashSet<int>(), new HashSet<string> { origin }));

        while (queue.Count > 0 && results.Count < maxResults)
        {
            var (current, path, usedFlightIds, visitedLocations) = queue.Dequeue();
            if (current == destination && path.Count > 0)
            {
                // Build Journey object
                var journey = new Journey
                {
                    Origin = origin,
                    Destination = destination,
                    DepartureTime = path.First().Departure,
                    JourneyFlights = path.Select((f, idx) => new JourneyFlight
                    {
                        Flight = f,
                        FlightId = f.FlightId,
                        Sequence = idx
                    }).ToList()
                };
                results.Add(journey);
                continue;
            }
            if (!flightsFrom.ContainsKey(current))
                continue;
            foreach (var flight in flightsFrom[current])
            {
                if (usedFlightIds.Contains(flight.FlightId))
                    continue; // Prevent cycles by flight
                if (visitedLocations.Contains(flight.Route.To))
                    continue; // Prevent cycles by location
                var newPath = new List<Flight>(path) { flight };
                if (maxExchanges.HasValue && newPath.Count - 1 > maxExchanges.Value)
                    continue; // Exceeds max exchanges
                var newUsed = new HashSet<int>(usedFlightIds) { flight.FlightId };
                var newVisited = new HashSet<string>(visitedLocations) { flight.Route.To };
                queue.Enqueue((flight.Route.To, newPath, newUsed, newVisited));
            }
        }
        // Order by fewest exchanges
        results = ascending
            ? results.OrderBy(j => j.NumberOfExchanges).Take(maxResults).ToList()
            : results.OrderByDescending(j => j.NumberOfExchanges).Take(maxResults).ToList();
        return results;
    }
} 