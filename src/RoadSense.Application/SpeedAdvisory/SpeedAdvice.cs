namespace RoadSense.Application.SpeedAdvisory;

public enum RiskLevel { Low, Moderate, High, Severe }

public sealed record SpeedAdvice(
    int BaseLimitKmh,
    int RecommendedKmh,
    RiskLevel Risk,
    IReadOnlyList<WeatherCondition> Conditions,
    IReadOnlyList<string> Messages);
