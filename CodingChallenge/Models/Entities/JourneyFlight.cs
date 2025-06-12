using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodingChallenge.Models.Entities;

public class JourneyFlight
{
    [Key]
    public int JourneyFlightId { get; set; }
    
    [Required]
    public int JourneyId { get; set; }
    
    [Required]
    public int FlightId { get; set; }
    
    public int Sequence { get; set; }  // To maintain the order of flights in a journey
    
    // Navigation properties
    public Journey Journey { get; set; } = null!;
    public Flight Flight { get; set; } = null!;
}
