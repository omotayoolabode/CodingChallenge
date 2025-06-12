using CodingChallenge.Data;
using CodingChallenge.Models.Entities;
using CodingChallenge.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CodingChallenge.Services;

public class JourneyService : IJourneyService
{
    private readonly AppDbContext _context;
    private readonly IMemoryCache _cache;
    
    private const string ROUTES_CACHE_KEY = "routes";
    private const string FLIGHTS_CACHE_KEY = "flights";

    public JourneyService(AppDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<List<Journey>> FindJourneysByMinExchangesAsync(string origin, string destination, bool ascending = true, int? maxExchanges = null, int maxResults = 100)
    {
        // Get routes from cache or load from database
        var routes = await _cache.GetOrCreateAsync(ROUTES_CACHE_KEY, async entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(30);
            return await _context.Routes.Include(r => r.Flights).ToListAsync();
        });

        // Get flights from cache or load from database
        var flights = await _cache.GetOrCreateAsync(FLIGHTS_CACHE_KEY, async entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(30);
            return await _context.Flights.Include(f => f.Route).ToListAsync();
        });

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

    public async Task<List<Journey>> FindJourneysByMinPriceAsync(string origin, string destination, DateTime departure, bool ascending = true, int? maxExchanges = null, int maxResults = 100)
    {
        // Get routes from cache or load from database
        var routes = await _cache.GetOrCreateAsync(ROUTES_CACHE_KEY, async entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(30);
            return await _context.Routes.Include(r => r.Flights).ToListAsync();
        });

        // Get flights from cache or load from database
        var flights = await _cache.GetOrCreateAsync(FLIGHTS_CACHE_KEY, async entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(30);
            return await _context.Flights.Include(f => f.Route).ToListAsync();
        });

        // Filter flights by departure time
        var availableFlights = flights.Where(f => f.Departure >= departure).ToList();

        // Build a map from location to available flights
        var flightsFrom = availableFlights.GroupBy(f => f.Route.From)
            .ToDictionary(g => g.Key, g => g.ToList());

        var results = new List<Journey>();
        var queue = new Queue<(string current, List<Flight> path, HashSet<int> usedFlightIds, HashSet<string> visitedLocations, DateTime currentTime)>();
        queue.Enqueue((origin, new List<Flight>(), new HashSet<int>(), new HashSet<string> { origin }, departure));

        while (queue.Count > 0 && results.Count < maxResults)
        {
            var (current, path, usedFlightIds, visitedLocations, currentTime) = queue.Dequeue();
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
                if (flight.Departure < currentTime)
                    continue; // Flight departs before current time
                var newPath = new List<Flight>(path) { flight };
                if (maxExchanges.HasValue && newPath.Count - 1 > maxExchanges.Value)
                    continue; // Exceeds max exchanges
                var newUsed = new HashSet<int>(usedFlightIds) { flight.FlightId };
                var newVisited = new HashSet<string>(visitedLocations) { flight.Route.To };
                // Update current time to flight departure time for next iteration
                queue.Enqueue((flight.Route.To, newPath, newUsed, newVisited, flight.Departure));
            }
        }
        // Order by total price
        results = ascending
            ? results.OrderBy(j => j.TotalPrice).Take(maxResults).ToList()
            : results.OrderByDescending(j => j.TotalPrice).Take(maxResults).ToList();
        return results;
    }
} 