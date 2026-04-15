using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bake.Migrations
{
    /// <inheritdoc />
    public partial class AllowNullShop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "shop_time",
                schema: "Sales",
                table: "Shop",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "(sysdatetime())",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "(sysdatetime())");

            migrationBuilder.AlterColumn<decimal>(
                name: "shop_rating",
                schema: "Sales",
                table: "Shop",
                type: "decimal(2,1)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(2,1)");

            migrationBuilder.AlterColumn<string>(
                name: "shop_img",
                schema: "Sales",
                table: "Shop",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "shop_time",
                schema: "Sales",
                table: "Shop",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "(sysdatetime())",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValueSql: "(sysdatetime())");

            migrationBuilder.AlterColumn<decimal>(
                name: "shop_rating",
                schema: "Sales",
                table: "Shop",
                type: "decimal(2,1)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(2,1)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "shop_img",
                schema: "Sales",
                table: "Shop",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048,
                oldNullable: true);
        }
    }
}
