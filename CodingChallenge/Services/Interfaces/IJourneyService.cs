using CodingChallenge.Models.Entities;

namespace CodingChallenge.Services.Interfaces;

public interface IJourneyService
{
    /// <summary>
    /// Finds all valid journeys from origin to destination, ordered by fewest exchanges.
    /// </summary>
    /// <param name="origin">The starting location.</param>
    /// <param name="destination">The target location.</param>
    /// <param name="ascending">Order ascending if true, descending if false.</param>
    /// <param name="maxExchanges">Maximum number of exchanges allowed in a journey (optional).</param>
    /// <param name="maxResults">Maximum number of journeys to return (optional, default 100).</param>
    /// <returns>List of journeys.</returns>
    Task<List<Journey>> FindJourneysByMinExchangesAsync(string origin, string destination, bool ascending = true, int? maxExchanges = null, int maxResults = 100);
} 