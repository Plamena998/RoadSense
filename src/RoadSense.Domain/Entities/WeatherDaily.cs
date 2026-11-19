namespace RoadSense.Domain.Entities;

/// <summary>Дневно време за областния център (агрегирано от почасовите данни на Open-Meteo).</summary>
public class WeatherDaily
{
    public int Id { get; set; }
    public int OblastId { get; set; }
    public Oblast Oblast { get; set; } = null!;

    public DateOnly Date { get; set; }
    public double TempMin { get; set; }
    public double TempMax { get; set; }
    public double TempMean { get; set; }
    public double PrecipitationMm { get; set; }
    public double SnowfallCm { get; set; }
    public double HumidityMean { get; set; }
    public double CloudMean { get; set; }
    public double WindMaxKmh { get; set; }
    public double GustMaxKmh { get; set; }

    /// <summary>Най-високият WMO код за деня (по-висок = по-лошо време).</summary>
    public int WeatherCodeMax { get; set; }
}
