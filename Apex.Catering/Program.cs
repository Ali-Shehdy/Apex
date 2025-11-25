using Apex.Catering.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure SQLite database path
var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "catering.db");
builder.Services.AddDbContext<CateringDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

var app = builder.Build();

// Ensure database exists and migrations are applied
//using (var scope = app.Services.CreateScope())
//{
//    var context = scope.ServiceProvider.GetRequiredService<CateringDbContext>();
//    context.Database.Migrate();

//    // Seed data only if tables are empty
//    if (!context.FoodItems.Any())
//    {
//        context.FoodItems.AddRange(
//            new FoodItem { Description = "Chicken Curry", UnitPrice = 8.50M },
//            new FoodItem { Description = "Beef Stroganoff", UnitPrice = 9.00M },
//            new FoodItem { Description = "Vegetable Stir Fry", UnitPrice = 7.00M },
//            new FoodItem { Description = "Caesar Salad", UnitPrice = 6.50M },
//            new FoodItem { Description = "Grilled Salmon", UnitPrice = 12.00M },
//            new FoodItem { Description = "Pasta Primavera", UnitPrice = 8.00M },
//            new FoodItem { Description = "Roast Lamb", UnitPrice = 11.00M },
//            new FoodItem { Description = "Vegetable Lasagna", UnitPrice = 9.50M }
//        );
//        context.SaveChanges();
//    }

//    if (!context.Menus.Any())
//    {
//        context.Menus.AddRange(
//            new Menu { MenuId = 1, MenuName = "Standard Buffet" },
//            new Menu { MenuId = 2, MenuName = "Vegetarian Delight" },
//            new Menu { MenuId = 3, MenuName = "Seafood Special" }
//        );
//        context.SaveChanges();
//    }
//}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); 
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
