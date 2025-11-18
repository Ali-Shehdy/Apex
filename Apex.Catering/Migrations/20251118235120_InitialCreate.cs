using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Apex.Catering.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FoodItems",
                columns: table => new
                {
                    FoodItemId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodItems", x => x.FoodItemId);
                });

            migrationBuilder.CreateTable(
                name: "Menus",
                columns: table => new
                {
                    MenuId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MenuName = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.MenuId);
                });

            migrationBuilder.CreateTable(
                name: "FoodBookings",
                columns: table => new
                {
                    FoodBookingId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClintReferenceId = table.Column<int>(type: "INTEGER", nullable: true),
                    NumberOfGuests = table.Column<int>(type: "INTEGER", nullable: true),
                    MenuId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodBookings", x => x.FoodBookingId);
                    table.ForeignKey(
                        name: "FK_FoodBookings_Menus_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Menus",
                        principalColumn: "MenuId");
                });

            migrationBuilder.CreateTable(
                name: "MenuFoodItems",
                columns: table => new
                {
                    MenuId = table.Column<int>(type: "INTEGER", nullable: false),
                    FoodItemId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuFoodItems", x => new { x.MenuId, x.FoodItemId });
                    table.ForeignKey(
                        name: "FK_MenuFoodItems_FoodItems_FoodItemId",
                        column: x => x.FoodItemId,
                        principalTable: "FoodItems",
                        principalColumn: "FoodItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuFoodItems_Menus_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Menus",
                        principalColumn: "MenuId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "FoodItems",
                columns: new[] { "FoodItemId", "Description", "UnitPrice" },
                values: new object[,]
                {
                    { 1, "Chicken Curry", 8.50m },
                    { 2, "Beef Stroganoff", 9.00m },
                    { 3, "Vegetable Stir Fry", 7.00m },
                    { 4, "Caesar Salad", 6.50m },
                    { 5, "Grilled Salmon", 12.00m },
                    { 6, "Pasta Primavera", 8.00m },
                    { 7, "Roast Lamb", 11.00m },
                    { 8, "Vegetable Lasagna", 9.50m }
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_FoodBookings_MenuId",
                table: "FoodBookings",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuFoodItems_FoodItemId",
                table: "MenuFoodItems",
                column: "FoodItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodBookings");

            migrationBuilder.DropTable(
                name: "MenuFoodItems");

            migrationBuilder.DropTable(
                name: "FoodItems");

            migrationBuilder.DropTable(
                name: "Menus");
        }
    }
}
