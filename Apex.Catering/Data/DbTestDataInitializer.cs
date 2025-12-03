//using Apex.Catering.Data;
//using Microsoft.EntityFrameworkCore;

//namespace Apex.Catering.Data
//{
//    public class DbTestDataInitializer
//    {
//        private readonly CateringDbContext _context;

//        public DbTestDataInitializer(CateringDbContext context)
//        {
//            _context = context;
//        }

//        public void SeedTestData()
//        {
//            // Apply any pending migrations (creates DB if missing)
//            _context.Database.Migrate();

//            // Seed FoodItems if table is empty
//            if (!_context.FoodItems.Any())
//            {
//                var foodItems = new List<FoodItem>
//                {
//                    new FoodItem { FoodItemId = 1, Description = "Chicken Curry", UnitPrice = 8.50m },
//                    new FoodItem { FoodItemId = 2, Description = "Beef Stroganoff", UnitPrice = 9.00m },
//                    new FoodItem { FoodItemId = 3, Description = "Vegetable Stir Fry", UnitPrice = 7.00m },
//                    new FoodItem { FoodItemId = 4, Description = "Caesar Salad", UnitPrice = 6.50m },
//                    new FoodItem { FoodItemId = 5, Description = "Grilled Salmon", UnitPrice = 12.00m },
//                    new FoodItem { FoodItemId = 6, Description = "Pasta Primavera", UnitPrice = 8.00m },
//                    new FoodItem { FoodItemId = 7, Description = "Roast Lamb", UnitPrice = 11.00m },
//                    new FoodItem { FoodItemId = 8, Description = "Vegetable Lasagna", UnitPrice = 9.50m }
//                };
//                _context.FoodItems.AddRange(foodItems);
//            }

//            // Seed Menus
//            if (!_context.Menus.Any())
//            {
//                var menus = new List<Menu>
//                {
//                    new Menu { MenuId = 1, MenuName = "Standard Buffet" },
//                    new Menu { MenuId = 2, MenuName = "Vegetarian Delight" },
//                    new Menu { MenuId = 3, MenuName = "Seafood Special" }
//                };
//                _context.Menus.AddRange(menus);
//            }

//            // Seed FoodBookings
//            if (!_context.FoodBookings.Any())
//            {
//                var bookings = new List<FoodBooking>
//                {
//                    new FoodBooking { FoodBookingId = 1, ClintReferenceId = 1001, NumberOfGuests = 50, MenuId = 1 },
//                    new FoodBooking { FoodBookingId = 2, ClintReferenceId = 1002, NumberOfGuests = 30, MenuId = 2 },
//                    new FoodBooking { FoodBookingId = 3, ClintReferenceId = 1003, NumberOfGuests = 20, MenuId = 3 },
//                    new FoodBooking { FoodBookingId = 4, ClintReferenceId = 1004, NumberOfGuests = 40, MenuId = 1 }
//                };
//                _context.FoodBookings.AddRange(bookings);
//            }

//            // Seed MenuFoodItems
//            if (!_context.MenuFoodItems.Any())
//            {
//                var menuFoodItems = new List<MenuFoodItem>
//                {
//                    new MenuFoodItem { MenuId = 1, FoodItemId = 1 },
//                    new MenuFoodItem { MenuId = 1, FoodItemId = 2 },
//                    new MenuFoodItem { MenuId = 1, FoodItemId = 4 },
//                    new MenuFoodItem { MenuId = 1, FoodItemId = 5 },
//                    new MenuFoodItem { MenuId = 2, FoodItemId = 3 },
//                    new MenuFoodItem { MenuId = 2, FoodItemId = 4 },
//                    new MenuFoodItem { MenuId = 2, FoodItemId = 6 },
//                    new MenuFoodItem { MenuId = 2, FoodItemId = 8 },
//                    new MenuFoodItem { MenuId = 3, FoodItemId = 4 },
//                    new MenuFoodItem { MenuId = 3, FoodItemId = 5 },
//                    new MenuFoodItem { MenuId = 3, FoodItemId = 7 }
//                };
//                _context.MenuFoodItems.AddRange(menuFoodItems);
//            }

//            // Save changes
//            _context.SaveChanges();
//        }
//    }
//}

//Readme(DbTestDataInitializer)
