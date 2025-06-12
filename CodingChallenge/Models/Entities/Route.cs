using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodingChallenge.Models.Entities;

public class Route
{
    [Key]
    [Required]
    [MaxLength(50)]
    public string RouteId { get; set; } = string.Empty;

    [Required]
    [MaxLength(1)]
    public string From { get; set; } = string.Empty;

    [Required]
    [MaxLength(1)]
    public string To { get; set; } = string.Empty;

    [Required]
    public int Duration { get; set; }

    
    // Navigation property
    public ICollection<Flight> Flights { get; set; } = new List<Flight>();
}
