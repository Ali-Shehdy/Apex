// Apex.Events/Program.cs
using Apex.Events.Data;
using Apex.Events.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure DbContext
builder.Services.AddDbContext<EventsDbContext>(options =>
{
    var dbPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "events.db");
    options.UseSqlite($"Data Source={dbPath}");
});

// Add Db Initializer
builder.Services.AddScoped<DbTestDataInitializer>();

// ✅ Register EventTypeService
builder.Services.AddHttpClient<EventTypeService>((serviceProvider, client) =>
{
    // Configure base address for Apex.Venues API
    // This should be the same as your Venues project URL
    client.BaseAddress = new Uri("https://localhost:7030/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");

    // For development - handle self-signed certificates
    if (builder.Environment.IsDevelopment())
    {
        client.DefaultRequestVersion = System.Net.HttpVersion.Version20;
    }
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();

    // For development only: accept any certificate
    if (builder.Environment.IsDevelopment())
    {
        handler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    }

    return handler;
});

// ✅ Register your VenueReservationService (if you created it)
builder.Services.AddScoped<IVenueReservationService, VenueReservationService>();

// ✅ Register HttpClient for VenueReservationService
builder.Services.AddHttpClient<VenueReservationService>((serviceProvider, client) =>
{
    // Configure base address for Apex.Venues API
    client.BaseAddress = new Uri("https://localhost:7030/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();

    if (builder.Environment.IsDevelopment())
    {
        handler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    }

    return handler;
});

var app = builder.Build();

// 🔥 CRITICAL: CREATE DATABASE BEFORE SEEDING
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EventsDbContext>();

    // Ensure database is created (creates tables if they don't exist)
    dbContext.Database.EnsureCreated();

    // Now seed data
    var initializer = scope.ServiceProvider.GetRequiredService<DbTestDataInitializer>();
    initializer.Initialize();
}

// Configure pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();