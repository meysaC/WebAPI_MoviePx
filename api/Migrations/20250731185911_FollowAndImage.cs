using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class FollowAndImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFollows_AspNetUsers_AppUserId",
                table: "UserFollows");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2acb2a89-44fd-4327-b8bc-e66273d47f55");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "88e7bd28-571c-4409-8b5e-ba3f1e090308");

            migrationBuilder.RenameColumn(
                name: "AppUserId",
                table: "UserFollows",
                newName: "FollowerId");

            migrationBuilder.RenameIndex(
                name: "IX_UserFollows_AppUserId",
                table: "UserFollows",
                newName: "IX_UserFollows_FollowerId");

            migrationBuilder.AlterColumn<string>(
                name: "FollowingId",
                table: "UserFollows",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "BannerImageUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileImageUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "7f6e7df9-6edc-45b3-a89b-d058821cc320", null, "User", "USER" },
                    { "cc4c12d7-4879-40b3-808d-4df7a60d2719", null, "Admin", "ADMİN" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFollows_FollowingId",
                table: "UserFollows",
                column: "FollowingId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollows_AspNetUsers_FollowerId",
                table: "UserFollows",
                column: "FollowerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollows_AspNetUsers_FollowingId",
                table: "UserFollows",
                column: "FollowingId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFollows_AspNetUsers_FollowerId",
                table: "UserFollows");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFollows_AspNetUsers_FollowingId",
                table: "UserFollows");

            migrationBuilder.DropIndex(
                name: "IX_UserFollows_FollowingId",
                table: "UserFollows");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7f6e7df9-6edc-45b3-a89b-d058821cc320");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cc4c12d7-4879-40b3-808d-4df7a60d2719");

            migrationBuilder.DropColumn(
                name: "BannerImageUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfileImageUrl",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "FollowerId",
                table: "UserFollows",
                newName: "AppUserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserFollows_FollowerId",
                table: "UserFollows",
                newName: "IX_UserFollows_AppUserId");

            migrationBuilder.AlterColumn<string>(
                name: "FollowingId",
                table: "UserFollows",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2acb2a89-44fd-4327-b8bc-e66273d47f55", null, "Admin", "ADMİN" },
                    { "88e7bd28-571c-4409-8b5e-ba3f1e090308", null, "User", "USER" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollows_AspNetUsers_AppUserId",
                table: "UserFollows",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
