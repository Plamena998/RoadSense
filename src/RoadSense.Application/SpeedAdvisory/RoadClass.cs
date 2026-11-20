namespace RoadSense.Application.SpeedAdvisory;

/// <summary>Вид път – определя базовото ограничение, когато няма по-точно (напр. maxspeed от OSM).</summary>
public enum RoadClass { Motorway, FirstClass, SecondClass, ThirdClass, Municipal, Urban }

public static class RoadClassExtensions
{
    /// <summary>Законови ограничения за леки автомобили в България.</summary>
    public static int DefaultLimitKmh(this RoadClass c) => c switch
    {
        RoadClass.Motorway => 140,
        RoadClass.Urban => 50,
        _ => 90
    };
}
