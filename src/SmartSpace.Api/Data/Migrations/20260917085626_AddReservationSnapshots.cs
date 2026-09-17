using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartSpace.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationSnapshots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CapacityAtBooking",
                table: "Reservations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LocationNameAtBooking",
                table: "Reservations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResourceNameAtBooking",
                table: "Reservations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CapacityAtBooking",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "LocationNameAtBooking",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ResourceNameAtBooking",
                table: "Reservations");
        }
    }
}
