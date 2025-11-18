using Apex.Catering.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CateringDbContext>();

// Add the DbInitializer as a scoped service
builder.Services.AddScoped<DbTestDataInitializer>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Seed the database with test data
    AddTestData(app);
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Helper Function to run The Seeding (Synchronous)
void AddTestData(IHost app)
{
    // Create a new scope to retrieve scoped services
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        // Get the DbTestDataInitializer service
        var initializer = services.GetRequiredService<DbTestDataInitializer>();

        // Run the initialization logic (Synchronous)
        initializer.SeedTestData();
    }
}

