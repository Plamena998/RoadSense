using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using RoadSense.Application.Abstractions;
using RoadSense.Application.SpeedAdvisory;

namespace RoadSense.Infrastructure.Weather;

/// <summary>
/// Взима времето за текущия час от Open-Meteo. Ползваме почасовите стойности (а не "current"),
/// за да имат същия смисъл като данните в dataSource/weather: валежът е сумата за последния час,
/// затова праговете в <see cref="WeatherClassifier"/> важат без преобразуване.
/// </summary>
public sealed class OpenMeteoWeatherProvider(HttpClient http) : IWeatherProvider
{
    private const string Variables =
        "temperature_2m,relative_humidity_2m,precipitation,snowfall,snow_depth,weather_code,wind_speed_10m,wind_gusts_10m,is_day";

    public async Task<WeatherObservation> GetCurrentAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
    {
        var lat = latitude.ToString(CultureInfo.InvariantCulture);
        var lon = longitude.ToString(CultureInfo.InvariantCulture);
        var url = $"v1/forecast?latitude={lat}&longitude={lon}&hourly={Variables}&forecast_days=1&timezone=auto";

        var response = await http.GetFromJsonAsync<ForecastResponse>(url, cancellationToken)
                       ?? throw new InvalidOperationException("Open-Meteo върна празен отговор.");
        var h = response.Hourly ?? throw new InvalidOperationException("Open-Meteo не върна почасови данни.");

        // Часовете са в местно време за координатите; избираме текущия час по изместването от UTC.
        var localNow = DateTime.UtcNow.AddSeconds(response.UtcOffsetSeconds);
        var wanted = new DateTime(localNow.Year, localNow.Month, localNow.Day, localNow.Hour, 0, 0)
            .ToString("yyyy-MM-dd'T'HH:mm", CultureInfo.InvariantCulture);
        var i = Array.IndexOf(h.Time, wanted);
        if (i < 0) throw new InvalidOperationException($"Open-Meteo не върна данни за {wanted}.");

        return new WeatherObservation(
            TemperatureC: h.Temperature[i] ?? 0,
            HumidityPercent: h.Humidity[i] ?? 0,
            PrecipitationMmPerHour: h.Precipitation[i] ?? 0,
            SnowfallCmPerHour: h.Snowfall[i] ?? 0,
            SnowDepthM: h.SnowDepth[i] ?? 0,
            WeatherCode: (int)(h.WeatherCode[i] ?? 0),
            WindKmh: h.WindSpeed[i] ?? 0,
            GustKmh: h.WindGusts[i] ?? 0,
            IsDay: (h.IsDay[i] ?? 1) >= 1);
    }

    private sealed class ForecastResponse
    {
        [JsonPropertyName("utc_offset_seconds")] public int UtcOffsetSeconds { get; set; }
        [JsonPropertyName("hourly")] public HourlyData? Hourly { get; set; }
    }

    private sealed class HourlyData
    {
        [JsonPropertyName("time")] public string[] Time { get; set; } = [];
        [JsonPropertyName("temperature_2m")] public double?[] Temperature { get; set; } = [];
        [JsonPropertyName("relative_humidity_2m")] public double?[] Humidity { get; set; } = [];
        [JsonPropertyName("precipitation")] public double?[] Precipitation { get; set; } = [];
        [JsonPropertyName("snowfall")] public double?[] Snowfall { get; set; } = [];
        [JsonPropertyName("snow_depth")] public double?[] SnowDepth { get; set; } = [];
        [JsonPropertyName("weather_code")] public double?[] WeatherCode { get; set; } = [];
        [JsonPropertyName("wind_speed_10m")] public double?[] WindSpeed { get; set; } = [];
        [JsonPropertyName("wind_gusts_10m")] public double?[] WindGusts { get; set; } = [];
        [JsonPropertyName("is_day")] public double?[] IsDay { get; set; } = [];
    }
}
