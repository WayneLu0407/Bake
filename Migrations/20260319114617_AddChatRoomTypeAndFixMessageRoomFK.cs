using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Bake.Migrations
{
    /// <inheritdoc />
    public partial class AddChatRoomTypeAndFixMessageRoomFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "room_type",
                schema: "Service",
                table: "Chat_Room",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateTable(
                name: "Chat_Room_Type",
                schema: "Service",
                columns: table => new
                {
                    type_id = table.Column<byte>(type: "tinyint", nullable: false),
                    type_name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chat_Room_Type", x => x.type_id);
                });

            migrationBuilder.InsertData(
                schema: "Service",
                table: "Chat_Room_Type",
                columns: new[] { "type_id", "type_name" },
                values: new object[,]
                {
                    { (byte)0, "一對一" },
                    { (byte)1, "群組" },
                    { (byte)2, "AI客服" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chat_Room_room_type",
                schema: "Service",
                table: "Chat_Room",
                column: "room_type");

            migrationBuilder.CreateIndex(
                name: "IX_Chat_Message_room_id",
                schema: "Service",
                table: "Chat_Message",
                column: "room_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Room",
                schema: "Service",
                table: "Chat_Message",
                column: "room_id",
                principalSchema: "Service",
                principalTable: "Chat_Room",
                principalColumn: "room_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Room_Type",
                schema: "Service",
                table: "Chat_Room",
                column: "room_type",
                principalSchema: "Service",
                principalTable: "Chat_Room_Type",
                principalColumn: "type_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_Room",
                schema: "Service",
                table: "Chat_Message");

            migrationBuilder.DropForeignKey(
                name: "FK_Room_Type",
                schema: "Service",
                table: "Chat_Room");

            migrationBuilder.DropTable(
                name: "Chat_Room_Type",
                schema: "Service");

            migrationBuilder.DropIndex(
                name: "IX_Chat_Room_room_type",
                schema: "Service",
                table: "Chat_Room");

            migrationBuilder.DropIndex(
                name: "IX_Chat_Message_room_id",
                schema: "Service",
                table: "Chat_Message");

            migrationBuilder.DropColumn(
                name: "room_type",
                schema: "Service",
                table: "Chat_Room");
        }
    }
}
