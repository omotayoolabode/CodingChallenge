using CodingChallenge.Data;
using CodingChallenge.Models.Entities;
using CodingChallenge.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CodingChallenge.Services;

public class JourneyService : IJourneyService
{
    private readonly AppDbContext _context;

    public JourneyService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Journey>> FindJourneysByMinExchangesAsync(string origin, string destination, bool ascending = true)
    {
        // Load all routes and flights into memory for graph traversal
        var routes = await _context.Routes.Include(r => r.Flights).ToListAsync();
        var flights = await _context.Flights.Include(f => f.Route).ToListAsync();

        // Build a map from location to available flights
        var flightsFrom = flights.GroupBy(f => f.Route.From)
            .ToDictionary(g => g.Key, g => g.ToList());

        var results = new List<Journey>();
        var queue = new Queue<(string current, List<Flight> path, HashSet<int> usedFlightIds)>();
        queue.Enqueue((origin, new List<Flight>(), new HashSet<int>()));

        while (queue.Count > 0)
        {
            var (current, path, usedFlightIds) = queue.Dequeue();
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
                    continue; // Prevent cycles
                var newPath = new List<Flight>(path) { flight };
                var newUsed = new HashSet<int>(usedFlightIds) { flight.FlightId };
                queue.Enqueue((flight.Route.To, newPath, newUsed));
            }
        }
        // Order by fewest exchanges
        results = ascending
            ? results.OrderBy(j => j.NumberOfExchanges).ToList()
            : results.OrderByDescending(j => j.NumberOfExchanges).ToList();
        return results;
    }
} 