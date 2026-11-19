using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoadSense.Application.Abstractions;
using RoadSense.Infrastructure.Import;
using RoadSense.Infrastructure.Persistence;

namespace RoadSense.Infrastructure;

public static class DependencyInjection
{
    /// <summary>Регистрира базата, импортера и реализациите на интерфейсите от Application.</summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, InfrastructureOptions options)
    {
        services.AddSingleton(options);
        services.AddDbContext<RoadSenseDbContext>(o => o.UseSqlServer(options.ConnectionString));
        services.AddScoped<DataImporter>();
        services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();
        services.AddScoped<IRoadSenseData, RoadSenseData>();
        return services;
    }
}
