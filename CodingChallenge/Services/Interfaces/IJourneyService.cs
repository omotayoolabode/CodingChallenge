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

    /// <summary>
    /// Finds all valid journeys from origin to destination that depart at or after the given time, ordered by lowest total price.
    /// </summary>
    /// <param name="origin">The starting location.</param>
    /// <param name="destination">The target location.</param>
    /// <param name="departure">Flights that take off on or after the selected departure time.</param>
    /// <param name="ascending">Order ascending if true, descending if false.</param>
    /// <param name="maxExchanges">Maximum number of exchanges allowed in a journey (optional).</param>
    /// <param name="maxResults">Maximum number of journeys to return (optional, default 100).</param>
    /// <returns>List of journeys.</returns>
    Task<List<Journey>> FindJourneysByMinPriceAsync(string origin, string destination, DateTime departure, bool ascending = true, int? maxExchanges = null, int maxResults = 100);

    /// <summary>
    /// Finds all valid journeys from origin to destination that depart at or after the given time, ordered by shortest total duration.
    /// </summary>
    /// <param name="origin">The starting location.</param>
    /// <param name="destination">The target location.</param>
    /// <param name="departure">Flights that take off on or after the selected departure time.</param>
    /// <param name="ascending">Order ascending if true, descending if false.</param>
    /// <param name="maxExchanges">Maximum number of exchanges allowed in a journey (optional).</param>
    /// <param name="maxResults">Maximum number of journeys to return (optional, default 100).</param>
    /// <returns>List of journeys.</returns>
    Task<List<Journey>> FindJourneysByMinDurationAsync(string origin, string destination, DateTime departure, bool ascending = true, int? maxExchanges = null, int maxResults = 100);

    /// <summary>
    /// Gets all possible journeys calculated on-the-fly with pagination support.
    /// </summary>
    /// <param name="page">The current page number (default: 1).</param>
    /// <param name="size">The max number of journeys per page (default: 100).</param>
    /// <param name="orderBy">Attribute to order by: NumberOfExchanges, TotalPrice, TotalDuration (default: NumberOfExchanges).</param>
    /// <param name="ascending">Order ascending if true, descending if false.</param>
    /// <param name="maxExchanges">Maximum number of exchanges allowed in a journey (optional).</param>
    /// <returns>Paginated list of journeys with metadata.</returns>
    Task<(List<Journey> Journeys, int TotalCount, int CurrentPage, int PageSize)> GetAllJourneysAsync(int page = 1, int size = 100, string orderBy = "NumberOfExchanges", bool ascending = true, int? maxExchanges = null);
} 