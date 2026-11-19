namespace RoadSense.Domain.Entities;

/// <summary>Мост от пътната мрежа на АПИ (точка).</summary>
public class Bridge
{
    public int Id { get; set; }
    public string RoadRef { get; set; } = "";
    public double Km { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
