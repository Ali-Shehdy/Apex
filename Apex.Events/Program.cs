using Apex.Events.Data;
using Apex.Events.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net.Security;
using System.Security.Claims;

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
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManagerOnly", policy => policy.RequireRole("Manager"));
    options.AddPolicy("Leadership", policy => policy.RequireRole("Manager", "TeamLeader"));
});

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
    DbSchemaInitializer.EnsureEventCancellationColumn(dbContext);
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
if (app.Environment.IsDevelopment())
{
    var enableDevLogin = builder.Configuration.GetValue<bool>("Auth:EnableDevelopmentLogin");
    if (enableDevLogin)
    {
        app.Use(async (context, next) =>
        {
            if (context.User?.Identity?.IsAuthenticated != true)
            {
                var roles = builder.Configuration.GetSection("Auth:DefaultRoles").Get<string[]>() ?? Array.Empty<string>();
                if (roles.Length > 0)
                {
                    var claims = new List<Claim> { new Claim(ClaimTypes.Name, "dev-user") };
                    claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
                    var identity = new ClaimsIdentity(claims, "Development");
                    context.User = new ClaimsPrincipal(identity);
                }
            }
            await next();

        });
    }

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapRazorPages();
    app.MapControllers();

    // Test endpoints
    app.MapGet("/testvenues", () => Results.Redirect("/TestVenues"));
    app.MapGet("/test", () => Results.Redirect("/TestVenues"));
    app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }));


    app.Run();

    static void EnsureEventCancellationColumn(EventsDbContext dbContext)
    {
        var connection = dbContext.Database.GetDbConnection();
        var shouldClose = connection.State == System.Data.ConnectionState.Closed;

        if (shouldClose)
        {
            connection.Open();
        }

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "PRAGMA table_info(Events);";
            using var reader = command.ExecuteReader();

            var hasColumn = false;
            while (reader.Read())
            {
                var columnName = reader.GetString(1);
                if (string.Equals(columnName, "IsCancelled", StringComparison.OrdinalIgnoreCase))
                {
                    hasColumn = true;
                    break;
                }
            }

            if (!hasColumn)
            {
                using var alterCommand = connection.CreateCommand();
                alterCommand.CommandText = "ALTER TABLE Events ADD COLUMN IsCancelled INTEGER NOT NULL DEFAULT 0;";
                alterCommand.ExecuteNonQuery();
            }
        }
        finally
        {
            if (shouldClose)
            {
                connection.Close();
            }
        }
    }
}
