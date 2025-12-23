// Apex.Events/Program.cs
using Apex.Events.Data;
using Apex.Events.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Add services
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

// Register EventTypeService with extended timeout
builder.Services.AddHttpClient<EventTypeService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7030/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
    };
    return handler;
});

// Register VenueReservationService with extended timeout
builder.Services.AddScoped<IVenueReservationService, VenueReservationService>();
builder.Services.AddHttpClient("VenueReservationService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7030/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
    };
    return handler;
});

var app = builder.Build();

// Create and seed database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EventsDbContext>();
    dbContext.Database.EnsureCreated();

    var initializer = scope.ServiceProvider.GetRequiredService<DbTestDataInitializer>();
    initializer.Initialize();
}

// Configure pipeline
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

app.MapRazorPages();

// Add specific route for test page
app.MapGet("/testvenues", () => Results.Redirect("/TestVenues"));
app.MapGet("/test", () => Results.Redirect("/TestVenues"));

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }));

// Test Apex.Venues connection on startup
app.Lifetime.ApplicationStarted.Register(() =>
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var config = scope.ServiceProvider.GetService<IConfiguration>();

    logger.LogInformation("Application started");
    logger.LogInformation("Testing connection to Apex.Venues API...");

    try
    {
        var clientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        var client = clientFactory.CreateClient();
        client.BaseAddress = new Uri("https://localhost:7030/");
        client.Timeout = TimeSpan.FromSeconds(10);

        var response = client.GetAsync("api/eventtypes").Result;
        if (response.IsSuccessStatusCode)
        {
            logger.LogInformation("✅ Successfully connected to Apex.Venues API");

            // Test venue availability API
            var testDate = DateTime.Today.AddMonths(2).ToString("yyyy-MM-dd");
            var venueResponse = client.GetAsync($"api/availability?eventType=MET&beginDate={testDate}").Result;
            logger.LogInformation($"Venue API test: {venueResponse.StatusCode}");
        }
        else
        {
            logger.LogWarning($"⚠️ Apex.Venues API returned status: {response.StatusCode}");
            logger.LogWarning("Make sure Apex.Venues is running on https://localhost:7030");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Failed to connect to Apex.Venues API");
        logger.LogError("Please start Apex.Venues project first on port 7030");
    }

    logger.LogInformation("=== Apex.Venues Constraints ===");
    logger.LogInformation("• Only 30% of dates have availability (random)");
    logger.LogInformation("• Dates start ~2 months from today");
    logger.LogInformation("• MET events → TNDMR venue only");
    logger.LogInformation("• CON events → CRKHL, TNDMR venues");
    logger.LogInformation("• WED events → CRKHL, TNDMR, FDLCK venues");
    logger.LogInformation("• PTY events → CRKHL, FDLCK venues");
});

app.Run();