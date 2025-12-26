using Apex.Events.Data;
using Apex.Events.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Add services
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext
builder.Services.AddDbContext<EventsDbContext>(options =>
{
    var dbPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "events.db");
    options.UseSqlite($"Data Source={dbPath}");
});

// Db Initializer
builder.Services.AddScoped<DbTestDataInitializer>();

// EventTypeService
builder.Services.AddHttpClient<EventTypeService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7088/");
});

// VenueReservationService – FIXED DI issue
builder.Services.AddScoped<IVenueReservationService>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient();
    httpClient.BaseAddress = new Uri("https://localhost:7030/");
    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    httpClient.Timeout = TimeSpan.FromSeconds(30);

    var logger = sp.GetRequiredService<ILogger<VenueReservationService>>();

    return new VenueReservationService(httpClient, logger);
});

var app = builder.Build();

// Seed DB
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EventsDbContext>();
    dbContext.Database.EnsureCreated();

    var initializer = scope.ServiceProvider.GetRequiredService<DbTestDataInitializer>();
    initializer.Initialize();
}

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Apex.Events API V1");
        c.RoutePrefix = "swagger";
    });
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

// Test endpoints
app.MapGet("/testvenues", () => Results.Redirect("/TestVenues"));
app.MapGet("/test", () => Results.Redirect("/TestVenues"));
app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }));

// Test Apex.Venues connection on startup
app.Lifetime.ApplicationStarted.Register(() =>
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Application started");

    try
    {
        var clientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        var client = clientFactory.CreateClient();
        client.BaseAddress = new Uri("https://localhost:7030/");
        client.Timeout = TimeSpan.FromSeconds(10);

        var response = client.GetAsync("api/eventtypes").Result;
        logger.LogInformation(response.IsSuccessStatusCode
            ? "✅ Connected to Apex.Venues API"
            : $"⚠️ Apex.Venues API returned status: {response.StatusCode}");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Failed to connect to Apex.Venues API. Start Apex.Venues first on port 7030.");
    }
});

app.Run();
