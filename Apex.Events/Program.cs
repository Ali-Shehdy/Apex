using Apex.Events.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add Razor Pages
builder.Services.AddRazorPages();

// Add Controllers (API support)
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<EventsDbContext>(options =>
{
    var dbPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "events.db");

    options.UseSqlite($"Data Source={dbPath}");
});
builder.Services.AddRazorPages();
// Add DbContext with SQLite

builder.Services.AddScoped<DbTestDataInitializer>();

var app = builder.Build();

builder.Services.AddHttpClient<VenueService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7135/api/"); // Apex.Venues host
});
app.Run();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EventsDbContext>();
    db.Database.EnsureCreated(); // Creates the SQLite file if missing
}

// Developer Exception Page
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    AddTestData(app);
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Map Razor Pages
app.MapRazorPages();

// Map API controllers
app.MapControllers();
app.Run();

void AddTestData(IHost app)
{
    using (var scope = app.Services.CreateScope())
    {
       var services = scope.ServiceProvider;
        var initializer = services.GetRequiredService<DbTestDataInitializer>();
        initializer.Initialize();
    }
}