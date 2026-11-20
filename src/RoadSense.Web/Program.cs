using RoadSense.Application.Abstractions;
using RoadSense.Application.SpeedAdvisory;
using RoadSense.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddInfrastructure(new InfrastructureOptions(
    ConnectionString: builder.Configuration.GetConnectionString("RoadSense")!,
    ProcessedDataPath: Path.GetFullPath(Path.Combine(
        builder.Environment.ContentRootPath,
        builder.Configuration["DataImport:ProcessedPath"]!))));

builder.Services.AddSingleton<SpeedAdvisor>();

var app = builder.Build();

// При старт: създава/обновява базата и зарежда данните, ако е празна.
using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>().InitializeAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
