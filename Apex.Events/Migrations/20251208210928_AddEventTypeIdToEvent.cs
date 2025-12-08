using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Apex.Events.Migrations
{
    /// <inheritdoc />
    public partial class AddEventTypeIdToEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventTypeId",
                table: "Events",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventTypeId",
                table: "Events");
        }
    }
}
