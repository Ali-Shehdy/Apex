using Apex.Events.Data;
using Apex.Events.Services;
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

// 🔥 REGISTER VENUE SERVICE (EventTypeService) WITH CONFIGURED URL
// Add HttpClient for EventTypeService
builder.Services.AddHttpClient<EventTypeService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["VenuesApiUrl"]);
    client.DefaultRequestVersion = System.Net.HttpVersion.Version20;
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.ConfigurePrimaryHttpMessageHandler(() =>
    new HttpClientHandler
    {
        // Accept self-signed HTTPS certificates in development
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
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
