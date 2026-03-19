using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Bake.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentMethodTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentMethod",
                columns: table => new
                {
                    PaymentMethodId = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethod", x => x.PaymentMethodId);
                });

            migrationBuilder.InsertData(
                table: "PaymentMethod",
                columns: new[] { "PaymentMethodId", "Name" },
                values: new object[,]
                {
                    { (byte)0, "信用卡" },
                    { (byte)1, "轉帳" },
                    { (byte)2, "貨到付款" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_payment_method",
                schema: "Sales",
                table: "Orders",
                column: "payment_method");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_PaymentMethod_payment_method",
                schema: "Sales",
                table: "Orders",
                column: "payment_method",
                principalTable: "PaymentMethod",
                principalColumn: "PaymentMethodId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_PaymentMethod_payment_method",
                schema: "Sales",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "PaymentMethod");

            migrationBuilder.DropIndex(
                name: "IX_Orders_payment_method",
                schema: "Sales",
                table: "Orders");
        }
    }
}
