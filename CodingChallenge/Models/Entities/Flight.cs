using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodingChallenge.Models.Entities;

public class Flight
{
    [Key]
    public int FlightId { get; set; }

    [Required]
    [ForeignKey("Route")]
    public string RouteId { get; set; } = string.Empty;
    
    // Navigation property
    public Route Route { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Provider { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Required]
    public DateTime Departure { get; set; }
    
    // Navigation property for journeys
    public ICollection<JourneyFlight> JourneyFlights { get; set; } = new List<JourneyFlight>();
}
