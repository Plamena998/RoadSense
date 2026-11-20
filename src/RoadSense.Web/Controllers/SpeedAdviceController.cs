using Microsoft.AspNetCore.Mvc;
using RoadSense.Application.Abstractions;
using RoadSense.Application.SpeedAdvisory;

namespace RoadSense.Web.Controllers;

/// <summary>
/// GET /api/speed-advice?lat=42.70&amp;lon=23.32&amp;roadClass=FirstClass
/// или с точно ограничение: ...&amp;limit=90
/// </summary>
[ApiController]
[Route("api/speed-advice")]
public class SpeedAdviceController(IWeatherProvider weather, SpeedAdvisor advisor, ILogger<SpeedAdviceController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] double lat, [FromQuery] double lon,
        [FromQuery] RoadClass roadClass = RoadClass.FirstClass,
        [FromQuery] int? limit = null,
        CancellationToken ct = default)
    {
        if (lat is < 41.0 or > 44.5 || lon is < 22.0 or > 29.0)
            return BadRequest("Координатите са извън България.");
        if (limit is < 10 or > 140)
            return BadRequest("Ограничението трябва да е между 10 и 140 км/ч.");

        WeatherObservation observation;
        try
        {
            observation = await weather.GetCurrentAsync(lat, lon, ct);
        }
        catch (Exception ex) when (ex is HttpRequestException or InvalidOperationException or TaskCanceledException && !ct.IsCancellationRequested)
        {
            logger.LogError(ex, "Open-Meteo е недостъпен");
            return StatusCode(StatusCodes.Status502BadGateway, "Времето не може да бъде получено в момента.");
        }

        var advice = limit is { } l ? advisor.Advise(observation, l) : advisor.Advise(observation, roadClass);
        return Ok(new { weather = observation, advice });
    }
}
