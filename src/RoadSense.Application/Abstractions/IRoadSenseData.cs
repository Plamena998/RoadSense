using RoadSense.Domain.Entities;

namespace RoadSense.Application.Abstractions;

/// <summary>
/// Четене на данните, нужни на сайта. Application зависи само от този интерфейс,
/// затова логиката за риска и статистиката може да се тества без база.
/// Методите се добавят тук, когато страниците започнат да ги ползват.
/// </summary>
public interface IRoadSenseData
{
    Task<IReadOnlyList<Oblast>> GetOblastsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DataSource>> GetDataSourcesAsync(CancellationToken cancellationToken = default);
}
