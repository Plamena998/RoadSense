using RoadSense.Application.SpeedAdvisory;

namespace RoadSense.Application.Abstractions;

/// <summary>Източник на моментно време за точка. Реализира се в Infrastructure (Open-Meteo).</summary>
public interface IWeatherProvider
{
    Task<WeatherObservation> GetCurrentAsync(double latitude, double longitude, CancellationToken cancellationToken = default);
}
