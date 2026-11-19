namespace RoadSense.Domain.Entities;

/// <summary>Тежки ПТП за цялата страна по ден от седмицата и час (в данните на МВР няма разбивка по област).</summary>
public class HourWeekdayStat
{
    public int Id { get; set; }

    /// <summary>1 = понеделник ... 7 = неделя.</summary>
    public int Weekday { get; set; }

    /// <summary>Час 0–23 (началото на часовия интервал).</summary>
    public int Hour { get; set; }

    public int Ptp { get; set; }
    public int Killed { get; set; }
    public int Injured { get; set; }
}
