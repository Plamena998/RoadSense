namespace RoadSense.Application.SpeedAdvisory;

/// <summary>Съветва безопасна скорост по моментното време и ограничението на пътя.</summary>
public sealed class SpeedAdvisor
{
    public SpeedAdvice Advise(WeatherObservation weather, int baseLimitKmh)
    {
        var conditions = WeatherClassifier.Classify(weather);
        if (conditions.Count == 0)
            return new SpeedAdvice(baseLimitKmh, baseLimitKmh, RiskLevel.Low, conditions,
                ["Условията са нормални – спазвайте ограничението."]);

        var rules = conditions.Select(SpeedReductionTable.For).ToList();

        var reduction = rules.Max(r => r.ReductionPercent)
                        + (rules.Count - 1) * SpeedReductionTable.ExtraPerAdditionalCondition
                        + (weather.IsDay ? 0 : SpeedReductionTable.NightExtra);
        reduction = Math.Min(reduction, SpeedReductionTable.MaxTotalReductionPercent);

        var speed = baseLimitKmh * (100 - reduction) / 100.0;
        foreach (var rule in rules.Where(r => r.MaxSpeedKmh is not null))
            speed = Math.Min(speed, rule.MaxSpeedKmh!.Value);

        // Закръгляме надолу до 10 км/ч, но не над ограничението и не под минимума.
        var recommended = (int)(Math.Floor(speed / 10) * 10);
        recommended = Math.Min(baseLimitKmh, Math.Max(SpeedReductionTable.MinSpeedKmh, recommended));

        var cut = 100.0 * (baseLimitKmh - recommended) / baseLimitKmh;
        var risk = cut switch
        {
            < 10 => RiskLevel.Low,
            < 30 => RiskLevel.Moderate,
            < 50 => RiskLevel.High,
            _ => RiskLevel.Severe
        };

        return new SpeedAdvice(baseLimitKmh, recommended, risk, conditions,
            rules.OrderByDescending(r => r.ReductionPercent).Select(r => r.Advice).ToList());
    }

    public SpeedAdvice Advise(WeatherObservation weather, RoadClass roadClass)
        => Advise(weather, roadClass.DefaultLimitKmh());
}
