namespace RoadSense.Application.Abstractions;

/// <summary>
/// Подготвя хранилището на данни при старт на приложението: създава/обновява схемата
/// и зарежда началните данни, ако хранилището е празно.
/// Реализацията е в Infrastructure (SQL Server), а Web само вика този интерфейс.
/// </summary>
public interface IDatabaseInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}
