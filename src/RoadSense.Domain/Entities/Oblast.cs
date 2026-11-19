namespace RoadSense.Domain.Entities;

/// <summary>Една от 28-те области. Координатите са на областния център (за времето и картата).</summary>
public class Oblast
{
    public int Id { get; set; }

    /// <summary>Име във вида, в който е във файловете на МВР (главни букви), напр. "СТАРА ЗАГОРА".</summary>
    public string Name { get; set; } = "";

    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public List<OblastPeriodStat> PeriodStats { get; set; } = [];
    public List<OblastMonthlyPtp> MonthlyPtps { get; set; } = [];
    public List<WeatherDaily> WeatherDays { get; set; } = [];
}
