using Microsoft.EntityFrameworkCore;

namespace Apex.Catering.Data
{
    public class CateringDbContext : DbContext
    {
       

        public DbSet<FoodItem> FoodItems { get; set; } = null!;
        public DbSet<Menu> Menus { get; set; } = null!;
        public DbSet<MenuFoodItem> MenuFoodItems { get; set; } = null!;
        public DbSet<FoodBooking> FoodBookings { get; set; } = null!;

        public CateringDbContext(DbContextOptions<CateringDbContext> options)
          : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite key for MenuFoodItem
            modelBuilder.Entity<MenuFoodItem>()
                .HasKey(mf => new { mf.MenuId, mf.FoodItemId });

            // Relationships
            modelBuilder.Entity<MenuFoodItem>()
                .HasOne(mf => mf.FoodItem)
                .WithMany(fi => fi.MenuFoodItem)
                .HasForeignKey(mf => mf.FoodItemId);

            modelBuilder.Entity<MenuFoodItem>()
                .HasOne(mf => mf.Menu)
                .WithMany(m => m.MenuFoodItems)
                .HasForeignKey(mf => mf.MenuId);

            modelBuilder.Entity<FoodBooking>()
                .HasOne(fb => fb.Menu)
                .WithMany(m => m.FoodBookings)
                .HasForeignKey(fb => fb.MenuId);

            // Seed Data via HasData (optional)
            modelBuilder.Entity<FoodItem>().HasData(
                new FoodItem { FoodItemId = 1, Description = "Chicken Curry", UnitPrice = 8.50M },
                new FoodItem { FoodItemId = 2, Description = "Beef Stroganoff", UnitPrice = 9.00M },
                new FoodItem { FoodItemId = 3, Description = "Vegetable Stir Fry", UnitPrice = 7.00M },
                new FoodItem { FoodItemId = 4, Description = "Caesar Salad", UnitPrice = 6.50M },
                new FoodItem { FoodItemId = 5, Description = "Grilled Salmon", UnitPrice = 12.00M },
                new FoodItem { FoodItemId = 6, Description = "Pasta Primavera", UnitPrice = 8.00M },
                new FoodItem { FoodItemId = 7, Description = "Roast Lamb", UnitPrice = 11.00M },
                new FoodItem { FoodItemId = 8, Description = "Vegetable Lasagna", UnitPrice = 9.50M }
            );

            modelBuilder.Entity<Menu>().HasData(
                new Menu { MenuId = 1, MenuName = "Standard Buffet" },
                new Menu { MenuId = 2, MenuName = "Vegetarian Delight" },
                new Menu { MenuId = 3, MenuName = "Seafood Special" }
            );

            modelBuilder.Entity<FoodBooking>().HasData(
                new FoodBooking { FoodBookingId = 1, ClintReferenceId = 1001, NumberOfGuests = 50, MenuId = 1 },
                new FoodBooking { FoodBookingId = 2, ClintReferenceId = 1002, NumberOfGuests = 30, MenuId = 2 },
                new FoodBooking { FoodBookingId = 3, ClintReferenceId = 1003, NumberOfGuests = 20, MenuId = 3 },
                new FoodBooking { FoodBookingId = 4, ClintReferenceId = 1004, NumberOfGuests = 40, MenuId = 1 }
            );

            modelBuilder.Entity<MenuFoodItem>().HasData(
                new MenuFoodItem { MenuId = 1, FoodItemId = 1 },
                new MenuFoodItem { MenuId = 1, FoodItemId = 2 },
                new MenuFoodItem { MenuId = 1, FoodItemId = 4 },
                new MenuFoodItem { MenuId = 1, FoodItemId = 5 },
                new MenuFoodItem { MenuId = 2, FoodItemId = 3 },
                new MenuFoodItem { MenuId = 2, FoodItemId = 4 },
                new MenuFoodItem { MenuId = 2, FoodItemId = 6 },
                new MenuFoodItem { MenuId = 2, FoodItemId = 8 },
                new MenuFoodItem { MenuId = 3, FoodItemId = 4 },
                new MenuFoodItem { MenuId = 3, FoodItemId = 5 },
                new MenuFoodItem { MenuId = 3, FoodItemId = 7 }
            );
        }
    }
}
