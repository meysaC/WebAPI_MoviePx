using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class FollowActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7f6e7df9-6edc-45b3-a89b-d058821cc320");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cc4c12d7-4879-40b3-808d-4df7a60d2719");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UserFollows",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UnFollowedWhen",
                table: "UserFollows",
                type: "datetime2",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4a873be4-74fa-4a05-923e-c1ad61065006", null, "User", "USER" },
                    { "5e8d6b16-4593-4c33-8fb6-7b8592a5903e", null, "Admin", "ADMİN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4a873be4-74fa-4a05-923e-c1ad61065006");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5e8d6b16-4593-4c33-8fb6-7b8592a5903e");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UserFollows");

            migrationBuilder.DropColumn(
                name: "UnFollowedWhen",
                table: "UserFollows");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "7f6e7df9-6edc-45b3-a89b-d058821cc320", null, "User", "USER" },
                    { "cc4c12d7-4879-40b3-808d-4df7a60d2719", null, "Admin", "ADMİN" }
                });
        }
    }
}
