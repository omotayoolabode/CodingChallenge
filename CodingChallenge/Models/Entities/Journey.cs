using System.ComponentModel.DataAnnotations;

namespace CodingChallenge.Models.Entities;

public class Journey
{
    [Key]
    public int JourneyId { get; set; }
    
    [Required]
    [MaxLength(1)]
    public string Origin { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(1)]
    public string Destination { get; set; } = string.Empty;
    
    public DateTime? DepartureTime { get; set; }
    
    // Navigation property for the many-to-many relationship with Flight
    public ICollection<JourneyFlight> JourneyFlights { get; set; } = new List<JourneyFlight>();
    
    // Computed properties (not mapped to database)
    public int NumberOfExchanges => JourneyFlights.Count - 1;
    
    public decimal TotalPrice => JourneyFlights.Sum(jf => jf.Flight.Price);
    
    public int TotalDuration => JourneyFlights.Sum(jf => jf.Flight.Route.Duration);
}
