using Apex.Events.Data;
using Apex.Events.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net.Security;

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
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "events.db");
    options.UseSqlite($"Data Source={dbPath}");
});

// Db Initializer
builder.Services.AddScoped<DbTestDataInitializer>();

// EventTypeService
builder.Services.AddHttpClient<EventTypeService>(client =>
{
    var baseUrl = builder.Configuration["VenuesApi:BaseUrl"];
    client.BaseAddress = new Uri(baseUrl!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// VenueReservationService – FIXED DI issue
builder.Services.AddHttpClient<IVenueReservationService, VenueReservationService>((sp, client) =>
{
    var baseUrl = builder.Configuration["VenuesApi:BaseUrl"];
    client.BaseAddress = new Uri(baseUrl!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (message, _, _, errors) =>
        message?.RequestUri?.Host is "localhost" or "127.0.0.1"
            ? true
            : errors == SslPolicyErrors.None
});





var app = builder.Build();

// Seed DB
// Apply migrations (recommended instead of EnsureCreated)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EventsDbContext>();
    dbContext.Database.Migrate();
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


app.Run();
