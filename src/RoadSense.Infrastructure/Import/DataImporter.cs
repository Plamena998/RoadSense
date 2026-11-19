using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoadSense.Infrastructure.Persistence;
using RoadSense.Domain.Entities;

namespace RoadSense.Infrastructure.Import;

/// <summary>
/// Зарежда чистите CSV файлове от dataSource/processed в базата.
/// Пуска се при старта на приложението; ако базата вече има данни, не прави нищо.
/// Файловете се подготвят със скрипта tools/prepare_data.py.
/// </summary>
public class DataImporter(RoadSenseDbContext db, ILogger<DataImporter> log)
{
    public async Task ImportAsync(string dir)
    {
        if (await db.Oblasts.AnyAsync())
        {
            log.LogInformation("Базата вече има данни - импортът се пропуска.");
            return;
        }

        if (!Directory.Exists(dir))
            throw new DirectoryNotFoundException(
                $"Не е намерена папката с данни: {dir}. Пуснете tools/prepare_data.py.");

        // Областите първи - всичко останало сочи към тях.
        var oblasts = CsvFile.Read(Path.Combine(dir, "oblasts.csv"), r => new Oblast
        {
            Name = r.Str("name"),
            Latitude = r.Dbl("latitude"),
            Longitude = r.Dbl("longitude"),
        });
        db.Oblasts.AddRange(oblasts);
        await db.SaveChangesAsync();
        var ids = oblasts.ToDictionary(o => o.Name, o => o.Id);

        db.OblastPeriodStats.AddRange(CsvFile.Read(Path.Combine(dir, "oblast_period_stats.csv"), r => new OblastPeriodStat
        {
            OblastId = ids[r.Str("oblast")],
            Period = r.Str("period"),
            Ptp = r.Int("ptp"),
            Killed = r.Int("killed"),
            Injured = r.Int("injured"),
        }));

        db.OblastMonthlyPtps.AddRange(CsvFile.Read(Path.Combine(dir, "oblast_monthly_ptp.csv"), r => new OblastMonthlyPtp
        {
            OblastId = ids[r.Str("oblast")],
            Year = r.Int("year"),
            Month = r.Int("month"),
            Ptp = r.Int("ptp"),
        }));

        db.HourWeekdayStats.AddRange(CsvFile.Read(Path.Combine(dir, "hour_weekday_stats.csv"), r => new HourWeekdayStat
        {
            Weekday = r.Int("weekday"),
            Hour = r.Int("hour"),
            Ptp = r.Int("ptp"),
            Killed = r.Int("killed"),
            Injured = r.Int("injured"),
        }));

        db.WeatherDays.AddRange(CsvFile.Read(Path.Combine(dir, "weather_daily.csv"), r => new WeatherDaily
        {
            OblastId = ids[r.Str("oblast")],
            Date = DateOnly.ParseExact(r.Str("date"), "yyyy-MM-dd", CultureInfo.InvariantCulture),
            TempMin = r.Dbl("temp_min"),
            TempMax = r.Dbl("temp_max"),
            TempMean = r.Dbl("temp_mean"),
            PrecipitationMm = r.Dbl("precipitation_mm"),
            SnowfallCm = r.Dbl("snowfall_cm"),
            HumidityMean = r.Dbl("humidity_mean"),
            CloudMean = r.Dbl("cloud_mean"),
            WindMaxKmh = r.Dbl("wind_max_kmh"),
            GustMaxKmh = r.Dbl("gust_max_kmh"),
            WeatherCodeMax = r.Int("weather_code_max"),
        }));

        db.Bridges.AddRange(CsvFile.Read(Path.Combine(dir, "bridges.csv"), r => new Bridge
        {
            RoadRef = r.Str("road_ref"),
            Km = r.Dbl("km"),
            Latitude = r.Dbl("latitude"),
            Longitude = r.Dbl("longitude"),
        }));

        db.Tunnels.AddRange(CsvFile.Read(Path.Combine(dir, "tunnels.csv"), r => new Tunnel
        {
            RoadRef = r.Str("road_ref"),
            Km = r.Dbl("km"),
            Latitude = r.Dbl("latitude"),
            Longitude = r.Dbl("longitude"),
        }));

        db.DataSources.AddRange(CsvFile.Read(Path.Combine(dir, "data_sources.csv"), r => new DataSource
        {
            Name = r.Str("name"),
            Period = r.Str("period"),
            Url = r.Str("url"),
        }));

        await db.SaveChangesAsync();
        log.LogInformation("Импортът приключи: {Count} записа.", await db.WeatherDays.CountAsync());
    }

}
