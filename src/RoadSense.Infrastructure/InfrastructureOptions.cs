namespace RoadSense.Infrastructure;

/// <summary>Настройки на Infrastructure слоя.</summary>
/// <param name="ConnectionString">Връзка към SQL Server.</param>
/// <param name="ProcessedDataPath">Папка с чистите CSV файлове (dataSource/processed).</param>
public record InfrastructureOptions(string ConnectionString, string ProcessedDataPath);
