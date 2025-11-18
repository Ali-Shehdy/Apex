namespace Apex.Catering.Data
{
    public class DbTestDataInitializer
    {

        private readonly CateringDbContext _context;
        public DbTestDataInitializer(CateringDbContext context)
        {
            _context = context;
        }

        public void SeedTestData()
        {
            // Ensure database is created
            _context.Database.EnsureCreated();
            // Check if data already exists
            if (_context.FoodItems.Any())
            {
                return; // Data already seeded
            }
            // Seed FoodItems
            var foodItems = new List<FoodItem>
            {
                new FoodItem { FoodItemId = 4, Description = "Spaghetti Bolognese", UnitPrice = 11.99m },
                new FoodItem { FoodItemId = 5, Description = "Vegetable Stir Fry", UnitPrice = 7.00m },
                new FoodItem { FoodItemId = 6, Description = "Beef Tacos", UnitPrice = 9.25m },
                new FoodItem { FoodItemId = 7, Description = "Mushroom Risotto", UnitPrice = 10.00m },
                new FoodItem { FoodItemId = 8, Description = "Caesar Salad", UnitPrice = 6.50m },

            };

            _context.FoodItems.AddRange(foodItems);
            _context.SaveChanges();

            var foodBookings = new List<FoodBooking>
            {
                new FoodBooking { FoodBookingId = 2, ClintReferenceId = 1007, NumberOfGuests = 30, MenuId = 2 },
                new FoodBooking { FoodBookingId = 3, ClintReferenceId = 1008, NumberOfGuests = 20, MenuId = 3 },
                new FoodBooking { FoodBookingId = 4, ClintReferenceId = 1009, NumberOfGuests = 40, MenuId = 1 },
                new FoodBooking { FoodBookingId = 5, ClintReferenceId = 1010, NumberOfGuests = 25, MenuId = 2 },
                };

            _context.FoodBookings.AddRange(foodBookings);
            _context.SaveChanges(); _context.SaveChanges();

            var menu = new List<Menu>
            {

                new Menu { MenuId = 4, MenuName = "Italian Feast" },
                new Menu { MenuId = 5, MenuName = "Barbecue Bonanza" },
                new Menu { MenuId = 6, MenuName = "Healthy Options" },
            };
            _context.Menus.AddRange(menu);
            _context.SaveChanges();

            var menuFoodItem = new List<MenuFoodItem>
            {
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
            };

            _context.MenuFoodItems.AddRange(menuFoodItem);
            _context.SaveChanges();



        }
}
}
