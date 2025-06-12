using CodingChallenge.Models.Entities;
using CodingChallenge.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodingChallenge.Controllers;

[ApiController]
[Route("api/journeys")]
[Authorize]
[Produces("application/json")]
public class JourneyController : ControllerBase
{
    private readonly IJourneyService _journeyService;
    private readonly ILogger<JourneyController> _logger;

    public JourneyController(IJourneyService journeyService, ILogger<JourneyController> logger)
    {
        _journeyService = journeyService;
        _logger = logger;
    }

    /// <summary>
    /// Returns all valid journeys from origin to destination, ordered by fewest exchanges.
    /// </summary>
    /// <param name="origin">The starting location (e.g., 'A').</param>
    /// <param name="destination">The target location (e.g., 'C').</param>
    /// <param name="order">Sort order: 'asc' (default) or 'desc'.</param>
    /// <param name="maxExchanges">Maximum number of exchanges allowed in a journey (optional).</param>
    /// <param name="maxResults">Maximum number of journeys to return (optional, default 100).</param>
    /// <returns>List of journeys.</returns>
    [HttpGet("min-exchanges")]
    public async Task<IActionResult> GetJourneysByMinExchanges([
        FromQuery] string origin,
        [FromQuery] string destination,
        [FromQuery] string order = "asc",
        [FromQuery] int? maxExchanges = null,
        [FromQuery] int maxResults = 100)
    {
        if (string.IsNullOrWhiteSpace(origin) || string.IsNullOrWhiteSpace(destination))
            return BadRequest(new { message = "Origin and destination are required." });
        bool ascending = order.ToLower() != "desc";
        var journeys = await _journeyService.FindJourneysByMinExchangesAsync(origin, destination, ascending, maxExchanges, maxResults);
        if (journeys == null || journeys.Count == 0)
            return NotFound(new { message = "No journey exists between the specified locations." });
        // Optionally, map to a DTO for cleaner output
        return Ok(journeys.Select(j => new {
            origin = j.Origin,
            destination = j.Destination,
            departure = j.DepartureTime.HasValue ? FormatDepartureTime(j.DepartureTime.Value) : "unknown",
            exchanges = j.NumberOfExchanges,
            flights = j.JourneyFlights.Select(jf => new {
                flight_id = jf.FlightId,
                provider = jf.Flight.Provider,
                from = jf.Flight.Route.From,
                to = jf.Flight.Route.To,
                price = jf.Flight.Price,
                departure = jf.Flight.Departure
            })
        }));
    }

    /// <summary>
    /// Returns all valid journeys from origin to destination that depart at or after the given time, ordered by lowest total price.
    /// </summary>
    /// <param name="origin">The starting location (e.g., 'A').</param>
    /// <param name="destination">The target location (e.g., 'C').</param>
    /// <param name="departure">Flights that take off on or after the selected departure time (ISO 8601 format: 2024-01-15T10:30:00Z).</param>
    /// <param name="order">Sort order: 'asc' (default) or 'desc'.</param>
    /// <param name="maxExchanges">Maximum number of exchanges allowed in a journey (optional).</param>
    /// <param name="maxResults">Maximum number of journeys to return (optional, default 100).</param>
    /// <returns>List of journeys ordered by total price.</returns>
    [HttpGet("min-price")]
    public async Task<IActionResult> GetJourneysByMinPrice([
        FromQuery] string origin,
        [FromQuery] string destination,
        [FromQuery] DateTime departure,
        [FromQuery] string order = "asc",
        [FromQuery] int? maxExchanges = null,
        [FromQuery] int maxResults = 100)
    {
        if (string.IsNullOrWhiteSpace(origin) || string.IsNullOrWhiteSpace(destination))
            return BadRequest(new { message = "Origin and destination are required." });

        bool ascending = order.ToLower() != "desc";
        var journeys = await _journeyService.FindJourneysByMinPriceAsync(origin, destination, departure, ascending, maxExchanges, maxResults);
        
        if (journeys == null || journeys.Count == 0)
            return NotFound(new { message = "No journey exists between the specified locations for the given departure time." });

        return Ok(journeys.Select(j => new {
            origin = j.Origin,
            destination = j.Destination,
            departure = j.DepartureTime.HasValue ? FormatDepartureTime(j.DepartureTime.Value) : "unknown",
            totalPrice = j.TotalPrice,
            exchanges = j.NumberOfExchanges,
            flights = j.JourneyFlights.Select(jf => new {
                flight_id = jf.FlightId,
                provider = jf.Flight.Provider,
                from = jf.Flight.Route.From,
                to = jf.Flight.Route.To,
                price = jf.Flight.Price,
                departure = jf.Flight.Departure
            })
        }));
    }

    /// <summary>
    /// Returns all valid journeys from origin to destination that depart at or after the given time, ordered by shortest total duration.
    /// </summary>
    /// <param name="origin">The starting location (e.g., 'A').</param>
    /// <param name="destination">The target location (e.g., 'C').</param>
    /// <param name="departure">Flights that take off on or after the selected departure time (ISO 8601 format: 2024-01-15T10:30:00Z).</param>
    /// <param name="order">Sort order: 'asc' (default) or 'desc'.</param>
    /// <param name="maxExchanges">Maximum number of exchanges allowed in a journey (optional).</param>
    /// <param name="maxResults">Maximum number of journeys to return (optional, default 100).</param>
    /// <returns>List of journeys ordered by total duration.</returns>
    [HttpGet("min-duration")]
    public async Task<IActionResult> GetJourneysByMinDuration([
        FromQuery] string origin,
        [FromQuery] string destination,
        [FromQuery] DateTime departure,
        [FromQuery] string order = "asc",
        [FromQuery] int? maxExchanges = null,
        [FromQuery] int maxResults = 100)
    {
        if (string.IsNullOrWhiteSpace(origin) || string.IsNullOrWhiteSpace(destination))
            return BadRequest(new { message = "Origin and destination are required." });

        bool ascending = order.ToLower() != "desc";
        var journeys = await _journeyService.FindJourneysByMinDurationAsync(origin, destination, departure, ascending, maxExchanges, maxResults);
        
        if (journeys == null || journeys.Count == 0)
            return NotFound(new { message = "No journey exists between the specified locations for the given departure time." });

        return Ok(journeys.Select(j => new {
            origin = j.Origin,
            destination = j.Destination,
            departure = j.DepartureTime.HasValue ? FormatDepartureTime(j.DepartureTime.Value) : "unknown",
            totalDuration = j.TotalDuration,
            exchanges = j.NumberOfExchanges,
            flights = j.JourneyFlights.Select(jf => new {
                flight_id = jf.FlightId,
                provider = jf.Flight.Provider,
                from = jf.Flight.Route.From,
                to = jf.Flight.Route.To,
                duration = jf.Flight.Route.Duration,
                departure = jf.Flight.Departure
            })
        }));
    }

    /// <summary>
    /// Returns all possible journeys calculated on-the-fly with pagination support.
    /// </summary>
    /// <param name="page">The current page number (default: 1).</param>
    /// <param name="size">The max number of journeys per page (default: 100).</param>
    /// <param name="orderBy">Attribute to order by: NumberOfExchanges, TotalPrice, TotalDuration (default: NumberOfExchanges).</param>
    /// <param name="order">Sort order: 'asc' (default) or 'desc'.</param>
    /// <param name="maxExchanges">Maximum number of exchanges allowed in a journey (optional).</param>
    /// <returns>Paginated list of journeys with metadata.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllJourneys([
        FromQuery] int page = 1,
        [FromQuery] int size = 100,
        [FromQuery] string orderBy = "NumberOfExchanges",
        [FromQuery] string order = "asc",
        [FromQuery] int? maxExchanges = null)
    {
        if (page < 1) page = 1;
        if (size < 1 || size > 1000) size = 100;

        bool ascending = order.ToLower() != "desc";
        var (journeys, totalCount, currentPage, pageSize) = await _journeyService.GetAllJourneysAsync(page, size, orderBy, ascending, maxExchanges);

        return Ok(new {
            journeys = journeys.Select(j => new {
                origin = j.Origin,
                destination = j.Destination,
                departure = j.DepartureTime.HasValue ? FormatDepartureTime(j.DepartureTime.Value) : "unknown",
                totalDuration = j.TotalDuration,
                totalPrice = j.TotalPrice,
                exchanges = j.NumberOfExchanges,
                flights = j.JourneyFlights.Select(jf => new {
                    flight_id = jf.FlightId,
                    provider = jf.Flight.Provider,
                    from = jf.Flight.Route.From,
                    to = jf.Flight.Route.To,
                    price = jf.Flight.Price,
                    duration = jf.Flight.Route.Duration,
                    departure = jf.Flight.Departure
                })
            }),
            pagination = new {
                totalJourneys = totalCount,
                currentPage = currentPage,
                pageSize = pageSize,
                totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            }
        });
    }

    private string FormatDepartureTime(DateTime departureTime)
    {
        var now = DateTime.UtcNow;
        var timeDifference = departureTime - now;
        
        if (timeDifference.TotalHours < 1)
        {
            return "now";
        }
        
        var days = (int)timeDifference.TotalDays;
        var hours = timeDifference.Hours;
        
        if (days > 0 && hours > 0)
        {
            return $"+{days} day {hours} hour";
        }
        else if (days > 0)
        {
            return $"+{days} day";
        }
        else
        {
            return $"+{hours} hour";
        }
    }
} 