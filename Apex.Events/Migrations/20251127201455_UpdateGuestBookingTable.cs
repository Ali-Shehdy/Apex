using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Apex.Events.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGuestBookingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "GuestBookings",
                columns: new[] { "GuestBookingId", "BookingDate", "EventId", "GuestId" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1 },
                    { 2, new DateTime(2024, 5, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GuestBookings",
                keyColumn: "GuestBookingId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "GuestBookings",
                keyColumn: "GuestBookingId",
                keyValue: 2);
        }
    }
}
