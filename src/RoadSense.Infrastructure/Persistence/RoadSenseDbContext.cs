using Microsoft.EntityFrameworkCore;
using RoadSense.Domain.Entities;

namespace RoadSense.Infrastructure.Persistence;

public class RoadSenseDbContext(DbContextOptions<RoadSenseDbContext> options) : DbContext(options)
{
    public DbSet<Oblast> Oblasts => Set<Oblast>();
    public DbSet<OblastPeriodStat> OblastPeriodStats => Set<OblastPeriodStat>();
    public DbSet<OblastMonthlyPtp> OblastMonthlyPtps => Set<OblastMonthlyPtp>();
    public DbSet<HourWeekdayStat> HourWeekdayStats => Set<HourWeekdayStat>();
    public DbSet<WeatherDaily> WeatherDays => Set<WeatherDaily>();
    public DbSet<Bridge> Bridges => Set<Bridge>();
    public DbSet<Tunnel> Tunnels => Set<Tunnel>();
    public DbSet<DataSource> DataSources => Set<DataSource>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Oblast>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(60);
            e.HasIndex(x => x.Name).IsUnique();
        });

        // Една област има най-много един ред на период / месец / ден - защитава от двоен импорт.
        b.Entity<OblastPeriodStat>(e =>
        {
            e.Property(x => x.Period).HasMaxLength(20);
            e.HasIndex(x => new { x.OblastId, x.Period }).IsUnique();
        });
        b.Entity<OblastMonthlyPtp>().HasIndex(x => new { x.OblastId, x.Year, x.Month }).IsUnique();
        b.Entity<HourWeekdayStat>().HasIndex(x => new { x.Weekday, x.Hour }).IsUnique();
        b.Entity<WeatherDaily>().HasIndex(x => new { x.OblastId, x.Date }).IsUnique();

        b.Entity<Bridge>().Property(x => x.RoadRef).HasMaxLength(50);
        b.Entity<Tunnel>().Property(x => x.RoadRef).HasMaxLength(50);

        b.Entity<DataSource>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(200);
            e.Property(x => x.Period).HasMaxLength(100);
            e.Property(x => x.Url).HasMaxLength(300);
        });
    }
}
