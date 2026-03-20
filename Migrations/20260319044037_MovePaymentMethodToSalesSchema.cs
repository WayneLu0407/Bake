using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bake.Migrations
{
    /// <inheritdoc />
    public partial class MovePaymentMethodToSalesSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "PaymentMethod",
                newName: "PaymentMethod",
                newSchema: "Sales");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "PaymentMethod",
                schema: "Sales",
                newName: "PaymentMethod");
        }
    }
}
