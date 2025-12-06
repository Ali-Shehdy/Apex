using Apex.Events.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add Razor Pages
builder.Services.AddRazorPages();

// Add Controllers
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext with SQLite
builder.Services.AddDbContext<EventsDbContext>(options =>
{
    var dbPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "events.db");

    options.UseSqlite($"Data Source={dbPath}");
});

// Add Db Initializer
builder.Services.AddScoped<DbTestDataInitializer>();

// 🔥 REGISTER VENUE SERVICE HERE (before Build)
builder.Services.AddHttpClient<VenueService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7135/api/"); // Apex.Venues host
});

// Build App
var app = builder.Build();

// Ensure database exists
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EventsDbContext>();
    db.Database.EnsureCreated();
}

// Developer Exception page / HSTS
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

// Razor pages + API routes
app.MapRazorPages();
app.MapControllers();

app.Run();

// --- Helper ---
void AddTestData(IHost app)
{
    using var scope = app.Services.CreateScope();
    var initializer = scope.ServiceProvider.GetRequiredService<DbTestDataInitializer>();
    initializer.Initialize();
}
