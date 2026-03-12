using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bake.Migrations
{
    public partial class AddProductIngredients : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Product_Ingredients",
                schema: "Sales",
                columns: table => new
                {
                    product_id = table.Column<int>(nullable: false),
                    shelf_life_note = table.Column<string>(maxLength: 200, nullable: true),
                    ingredients = table.Column<string>(nullable: true),
                    net_weight = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product_Ingredient", x => x.product_id);
                    table.ForeignKey(
                        name: "FK_Ingredient_Product",
                        column: x => x.product_id,
                        principalSchema: "Sales",
                        principalTable: "Products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Product_Ingredients",
                schema: "Sales");
        }
    }
}