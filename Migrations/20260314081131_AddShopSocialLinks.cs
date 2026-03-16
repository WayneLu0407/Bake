using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bake.Migrations
{
    /// <inheritdoc />
    public partial class AddShopSocialLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "facebook_url",
                schema: "Sales",
                table: "Shop",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "instagram_url",
                schema: "Sales",
                table: "Shop",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "pinterest_url",
                schema: "Sales",
                table: "Shop",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "youtube_url",
                schema: "Sales",
                table: "Shop",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "facebook_url",
                schema: "Sales",
                table: "Shop");

            migrationBuilder.DropColumn(
                name: "instagram_url",
                schema: "Sales",
                table: "Shop");

            migrationBuilder.DropColumn(
                name: "pinterest_url",
                schema: "Sales",
                table: "Shop");

            migrationBuilder.DropColumn(
                name: "youtube_url",
                schema: "Sales",
                table: "Shop");
        }
    }
}
