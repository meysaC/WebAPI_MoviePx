using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserFollow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95e6b4f1-9773-497f-be0e-8429befa7c9e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a3d0c734-b337-49ff-9d04-9295e64fd7e0");

            migrationBuilder.RenameColumn(
                name: "FallowingId",
                table: "UserFollows",
                newName: "FollowingId");

            migrationBuilder.AddColumn<DateTime>(
                name: "FollowedWhen",
                table: "UserFollows",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4b3da6e8-f592-497f-b985-9e83e9596196", null, "Admin", "ADMİN" },
                    { "56d94023-4d74-4072-ab85-699ff2bffde8", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "FollowedWhen",
                table: "UserFollows");

            migrationBuilder.RenameColumn(
                name: "FollowingId",
                table: "UserFollows",
                newName: "FallowingId");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "95e6b4f1-9773-497f-be0e-8429befa7c9e", null, "Admin", "ADMİN" },
                    { "a3d0c734-b337-49ff-9d04-9295e64fd7e0", null, "User", "USER" }
                });
        }
    }
}
