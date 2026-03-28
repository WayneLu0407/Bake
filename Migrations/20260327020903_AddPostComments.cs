using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bake.Migrations
{
    /// <inheritdoc />
    public partial class AddPostComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Post_Comments",
                schema: "Social",
                columns: table => new
                {
                    comment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    post_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    content = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    parent_comment_id = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post_Comments", x => x.comment_id);
                    table.ForeignKey(
                        name: "FK_Post_Comments_Post_Comments_parent_comment_id",
                        column: x => x.parent_comment_id,
                        principalSchema: "Social",
                        principalTable: "Post_Comments",
                        principalColumn: "comment_id");
                    table.ForeignKey(
                        name: "FK_Post_Comments_Posts_post_id",
                        column: x => x.post_id,
                        principalSchema: "Social",
                        principalTable: "Posts",
                        principalColumn: "post_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Post_Comments_User_Profile_user_id",
                        column: x => x.user_id,
                        principalSchema: "User",
                        principalTable: "User_Profile",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Post_Comments_parent_comment_id",
                schema: "Social",
                table: "Post_Comments",
                column: "parent_comment_id");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Comments_post_id",
                schema: "Social",
                table: "Post_Comments",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Comments_user_id",
                schema: "Social",
                table: "Post_Comments",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Items_Products",
                schema: "Sales",
                table: "Order_Items",
                column: "product_id",
                principalSchema: "Sales",
                principalTable: "Products",
                principalColumn: "product_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Items_Products",
                schema: "Sales",
                table: "Order_Items");

            migrationBuilder.DropTable(
                name: "Post_Comments",
                schema: "Social");

            migrationBuilder.DropIndex(
                name: "IX_Order_Items_product_id",
                schema: "Sales",
                table: "Order_Items");
        }
    }
}
