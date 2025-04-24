using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class ApiChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4b3da6e8-f592-497f-b985-9e83e9596196");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "56d94023-4d74-4072-ab85-699ff2bffde8");

            migrationBuilder.DropColumn(
                name: "ImdbID",
                table: "UserRatings");

            migrationBuilder.DropColumn(
                name: "ImdbID",
                table: "UserPreferances");

            migrationBuilder.DropColumn(
                name: "ImdbID",
                table: "Comments");

            migrationBuilder.AddColumn<int>(
                name: "MovieId",
                table: "UserRatings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MovieId",
                table: "UserPreferances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MovieId",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "46d85c0c-e5dc-4fae-8d0f-6063e8c5709b", null, "User", "USER" },
                    { "47e7de83-aa93-4b3e-accb-2c516bec860a", null, "Admin", "ADMİN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "46d85c0c-e5dc-4fae-8d0f-6063e8c5709b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "47e7de83-aa93-4b3e-accb-2c516bec860a");

            migrationBuilder.DropColumn(
                name: "MovieId",
                table: "UserRatings");

            migrationBuilder.DropColumn(
                name: "MovieId",
                table: "UserPreferances");

            migrationBuilder.DropColumn(
                name: "MovieId",
                table: "Comments");

            migrationBuilder.AddColumn<string>(
                name: "ImdbID",
                table: "UserRatings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImdbID",
                table: "UserPreferances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImdbID",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4b3da6e8-f592-497f-b985-9e83e9596196", null, "Admin", "ADMİN" },
                    { "56d94023-4d74-4072-ab85-699ff2bffde8", null, "User", "USER" }
                });
        }
    }
}
