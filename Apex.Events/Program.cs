using Apex.Catering.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add Razor Pages
builder.Services.AddRazorPages();

// Add Controllers (API support)
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext with SQLite
builder.Services.AddDbContext<CateringDbContext>(options =>
    options.UseSqlite($"Data Source={System.IO.Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "catering.db")}")
);

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CateringDbContext>();
    db.Database.EnsureCreated(); // Creates the SQLite file if missing
}

// Developer Exception Page
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
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

// Map Razor Pages
app.MapRazorPages();

// Map API controllers
app.MapControllers();

// Swagger middleware
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Apex Events API V1");
    c.RoutePrefix = "swagger"; // Access via /swagger
});

app.Run();
