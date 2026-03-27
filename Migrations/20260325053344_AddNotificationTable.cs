using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bake.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notifications",
                schema: "Service",
                columns: table => new
                {
                    notification_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    order_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    url = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    is_read = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    create_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.notification_id);
                    table.ForeignKey(
                        name: "FK_Notification_Orders",
                        column: x => x.order_id,
                        principalSchema: "Sales",
                        principalTable: "Orders",
                        principalColumn: "order_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_Items_product_id",
                schema: "Sales",
                table: "Order_Items",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_order_id",
                schema: "Service",
                table: "Notifications",
                column: "order_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Items_Products_product_id",
                schema: "Sales",
                table: "Order_Items",
                column: "product_id",
                principalSchema: "Sales",
                principalTable: "Products",
                principalColumn: "product_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Items_Products_product_id",
                schema: "Sales",
                table: "Order_Items");

            migrationBuilder.DropTable(
                name: "Notifications",
                schema: "Service");

            migrationBuilder.DropIndex(
                name: "IX_Order_Items_product_id",
                schema: "Sales",
                table: "Order_Items");
        }
    }
}
