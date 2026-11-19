using Microsoft.EntityFrameworkCore;
using RoadSense.Application.Abstractions;
using RoadSense.Infrastructure.Import;

namespace RoadSense.Infrastructure.Persistence;

/// <summary>Прилага миграциите и зарежда данните от CSV файловете, ако базата е празна.</summary>
internal class DatabaseInitializer(
    RoadSenseDbContext db,
    DataImporter importer,
    InfrastructureOptions options) : IDatabaseInitializer
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await db.Database.MigrateAsync(cancellationToken);
        await importer.ImportAsync(options.ProcessedDataPath);
    }
}
