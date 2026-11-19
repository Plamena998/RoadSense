namespace RoadSense.Domain.Entities;

/// <summary>Тежки ПТП, загинали и ранени за област и период (напр. "2026-H1" = 01.01–30.06.2026).</summary>
public class OblastPeriodStat
{
    public int Id { get; set; }
    public int OblastId { get; set; }
    public Oblast Oblast { get; set; } = null!;

    public string Period { get; set; } = "";
    public int Ptp { get; set; }
    public int Killed { get; set; }
    public int Injured { get; set; }
}
