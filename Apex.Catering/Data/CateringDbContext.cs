using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Runtime.InteropServices.Marshalling;

namespace Apex.Catering.Data
{
        

    public class CateringDbContext : DbContext
    {
        // Notes
        // - DbSet defines the database table.
        // - the class name is defined as part of the data model
        // - the instance/object name is normally plural
        // - by default, the instance name will become the table name
        public DbSet<FoodItem> FoodItems { get; set; } = null!;
        public DbSet<Menu> Menus { get; set; } = null!;
        public DbSet<MenuFoodItem> MenuFoodItems { get; set; } = null!;
        public DbSet<FoodBooking> FoodBookings { get; set; } = null!;


        private string DbPath { get; set; } = string.Empty;

        // Constructor to set-up the database path and name
        public CateringDbContext(DbContextOptions<CateringDbContext> options)
            : base(options)
        {
        }

        // Optional: keep parameterless for local dev tools if you want
        public CateringDbContext()
        {
            var folder = Environment.SpecialFolder.MyDocuments;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "catering.db");
        }

        // OnConfiguring to specify that the SQLite database engine will be used
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Composite primary key for MenuFoodItem
            modelBuilder.Entity<MenuFoodItem>()
                .HasKey(mf => new { mf.MenuId, mf.FoodItemId });

            // Relationship between MenuFoodItem and FoodItem
            modelBuilder.Entity<MenuFoodItem>()
                .HasOne(mf => mf.FoodItem)
                .WithMany(fi => fi.MenuFoodItem)
                .HasForeignKey(mf => mf.FoodItemId);

            // Relationship between MenuFoodItem and Menu
            modelBuilder.Entity<MenuFoodItem>()
                .HasOne(mf => mf.Menu)
                .WithMany(m => m.MenuFoodItems)
                .HasForeignKey(mf => mf.MenuId);

            // Relationship between FoodBooking and Menu
            modelBuilder.Entity<FoodBooking>()
                .HasOne(fb => fb.Menu)
                .WithMany(m => m.FoodBookings)
                .HasForeignKey(fb => fb.MenuId);

            // Insert Seed/Test Data
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

            modelBuilder.Entity<FoodBooking>().HasData(
                
                new FoodBooking { FoodBookingId = 1, ClintReferenceId = 1001, NumberOfGuests = 50, MenuId = 1 },
                new FoodBooking { FoodBookingId = 2, ClintReferenceId = 1002, NumberOfGuests = 30, MenuId = 2 },
                new FoodBooking { FoodBookingId = 3, ClintReferenceId = 1003, NumberOfGuests = 20, MenuId = 3 },
                new FoodBooking { FoodBookingId = 4, ClintReferenceId = 1004, NumberOfGuests = 40, MenuId = 1 }
            );

            modelBuilder.Entity<Menu>().HasData(
                new Menu { MenuId = 1, MenuName = "Standard Buffet" },
                new Menu { MenuId = 2, MenuName = "Vegetarian Delight" },
                new Menu { MenuId = 3, MenuName = "Seafood Special" }
            );

            modelBuilder.Entity<MenuFoodItem>().HasData(
                // Standard Buffet Menu
                new MenuFoodItem { MenuId = 1, FoodItemId = 1 },
                new MenuFoodItem { MenuId = 1, FoodItemId = 2 },
                new MenuFoodItem { MenuId = 1, FoodItemId = 4 },
                new MenuFoodItem { MenuId = 1, FoodItemId = 5 },
                // Vegetarian Delight Menu
                new MenuFoodItem { MenuId = 2, FoodItemId = 3 },
                new MenuFoodItem { MenuId = 2, FoodItemId = 4 },
                new MenuFoodItem { MenuId = 2, FoodItemId = 6 },
                new MenuFoodItem { MenuId = 2, FoodItemId = 8 },
                // Seafood Special Menu
                new MenuFoodItem { MenuId = 3, FoodItemId = 5 },
                new MenuFoodItem { MenuId = 3, FoodItemId = 7 },
                new MenuFoodItem { MenuId = 3, FoodItemId = 4 }
            );

        }
    }
    }



