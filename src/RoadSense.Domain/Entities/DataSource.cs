namespace RoadSense.Domain.Entities;

/// <summary>Откъде са данните - показва се като етикет "Източник" в сайта.</summary>
public class DataSource
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Period { get; set; } = "";
    public string Url { get; set; } = "";
}
