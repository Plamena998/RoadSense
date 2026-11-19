namespace RoadSense.Domain.Entities;

/// <summary>Брой тежки ПТП за област и месец.</summary>
public class OblastMonthlyPtp
{
    public int Id { get; set; }
    public int OblastId { get; set; }
    public Oblast Oblast { get; set; } = null!;

    public int Year { get; set; }
    public int Month { get; set; }
    public int Ptp { get; set; }
}
