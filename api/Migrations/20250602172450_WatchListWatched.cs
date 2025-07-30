using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class WatchListWatched : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "46d85c0c-e5dc-4fae-8d0f-6063e8c5709b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "47e7de83-aa93-4b3e-accb-2c516bec860a");

            migrationBuilder.AddColumn<bool>(
                name: "IsWatchList",
                table: "UserPreferances",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsWatched",
                table: "UserPreferances",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "13602737-ae1e-4c7c-bd07-60ca7ba1a855", null, "User", "USER" },
                    { "cdbb0896-618b-4320-9499-5b9d01f5477d", null, "Admin", "ADMİN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "13602737-ae1e-4c7c-bd07-60ca7ba1a855");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cdbb0896-618b-4320-9499-5b9d01f5477d");

            migrationBuilder.DropColumn(
                name: "IsWatchList",
                table: "UserPreferances");

            migrationBuilder.DropColumn(
                name: "IsWatched",
                table: "UserPreferances");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "46d85c0c-e5dc-4fae-8d0f-6063e8c5709b", null, "User", "USER" },
                    { "47e7de83-aa93-4b3e-accb-2c516bec860a", null, "Admin", "ADMİN" }
                });
        }
    }
}
