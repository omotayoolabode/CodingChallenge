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
            departure = j.DepartureTime,
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
} 