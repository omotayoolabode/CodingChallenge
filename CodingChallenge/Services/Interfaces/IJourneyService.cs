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
    /// <returns>List of journeys.</returns>
    Task<List<Journey>> FindJourneysByMinExchangesAsync(string origin, string destination, bool ascending = true);
} 