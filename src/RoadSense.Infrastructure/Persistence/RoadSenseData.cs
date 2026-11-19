using Microsoft.EntityFrameworkCore;
using RoadSense.Application.Abstractions;
using RoadSense.Domain.Entities;

namespace RoadSense.Infrastructure.Persistence;

/// <summary>Четене на данните от SQL Server през EF Core (само за четене, без проследяване на промени).</summary>
internal class RoadSenseData(RoadSenseDbContext db) : IRoadSenseData
{
    public async Task<IReadOnlyList<Oblast>> GetOblastsAsync(CancellationToken cancellationToken = default) =>
        await db.Oblasts.AsNoTracking().OrderBy(o => o.Name).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<DataSource>> GetDataSourcesAsync(CancellationToken cancellationToken = default) =>
        await db.DataSources.AsNoTracking().OrderBy(s => s.Id).ToListAsync(cancellationToken);
}
