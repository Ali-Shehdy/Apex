using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Apex.Catering.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeeddata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "FoodItems",
                columns: new[] { "FoodItemId", "Description", "UnitPrice" },
                values: new object[] { 8, "Vegetable Lasagna", 9.50m });

            migrationBuilder.InsertData(
                table: "Menus",
                columns: new[] { "MenuId", "MenuName" },
                values: new object[,]
                {
                    { 1, "Standard Buffet" },
                    { 2, "Vegetarian Delight" },
                    { 3, "Seafood Special" }
                });

            migrationBuilder.InsertData(
                table: "FoodBookings",
                columns: new[] { "FoodBookingId", "ClintReferenceId", "MenuId", "NumberOfGuests" },
                values: new object[,]
                {
                    { 1, 1001, 1, 50 },
                    { 2, 1002, 2, 30 },
                    { 3, 1003, 3, 20 },
                    { 4, 1004, 1, 40 }
                });

            migrationBuilder.InsertData(
                table: "MenuFoodItems",
                columns: new[] { "FoodItemId", "MenuId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 4, 1 },
                    { 5, 1 },
                    { 3, 2 },
                    { 4, 2 },
                    { 6, 2 },
                    { 8, 2 },
                    { 4, 3 },
                    { 5, 3 },
                    { 7, 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FoodBookings",
                keyColumn: "FoodBookingId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "FoodBookings",
                keyColumn: "FoodBookingId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "FoodBookings",
                keyColumn: "FoodBookingId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "FoodBookings",
                keyColumn: "FoodBookingId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "MenuFoodItems",
                keyColumns: new[] { "FoodItemId", "MenuId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "MenuFoodItems",
                keyColumns: new[] { "FoodItemId", "MenuId" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "MenuFoodItems",
                keyColumns: new[] { "FoodItemId", "MenuId" },
                keyValues: new object[] { 4, 1 });

            migrationBuilder.DeleteData(
                table: "MenuFoodItems",
                keyColumns: new[] { "FoodItemId", "MenuId" },
                keyValues: new object[] { 5, 1 });

            migrationBuilder.DeleteData(
                table: "MenuFoodItems",
                keyColumns: new[] { "FoodItemId", "MenuId" },
                keyValues: new object[] { 3, 2 });

            migrationBuilder.DeleteData(
                table: "MenuFoodItems",
                keyColumns: new[] { "FoodItemId", "MenuId" },
                keyValues: new object[] { 4, 2 });

            migrationBuilder.DeleteData(
                table: "MenuFoodItems",
                keyColumns: new[] { "FoodItemId", "MenuId" },
                keyValues: new object[] { 6, 2 });

            migrationBuilder.DeleteData(
                table: "MenuFoodItems",
                keyColumns: new[] { "FoodItemId", "MenuId" },
                keyValues: new object[] { 8, 2 });

            migrationBuilder.DeleteData(
                table: "MenuFoodItems",
                keyColumns: new[] { "FoodItemId", "MenuId" },
                keyValues: new object[] { 4, 3 });

            migrationBuilder.DeleteData(
                table: "MenuFoodItems",
                keyColumns: new[] { "FoodItemId", "MenuId" },
                keyValues: new object[] { 5, 3 });

            migrationBuilder.DeleteData(
                table: "MenuFoodItems",
                keyColumns: new[] { "FoodItemId", "MenuId" },
                keyValues: new object[] { 7, 3 });

            migrationBuilder.DeleteData(
                table: "FoodItems",
                keyColumn: "FoodItemId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Menus",
                keyColumn: "MenuId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Menus",
                keyColumn: "MenuId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Menus",
                keyColumn: "MenuId",
                keyValue: 3);
        }
    }
}
