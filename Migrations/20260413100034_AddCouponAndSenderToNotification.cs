using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bake.Migrations
{
    /// <inheritdoc />
    public partial class AddCouponAndSenderToNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "order_id",
                schema: "Service",
                table: "Notifications",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "coupon_id",
                schema: "Service",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "sender_id",
                schema: "Service",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_coupon_id",
                schema: "Service",
                table: "Notifications",
                column: "coupon_id");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_sender_id",
                schema: "Service",
                table: "Notifications",
                column: "sender_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_AccountAuth",
                schema: "Service",
                table: "Notifications",
                column: "sender_id",
                principalSchema: "User",
                principalTable: "Account_Auth",
                principalColumn: "user_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Coupons",
                schema: "Service",
                table: "Notifications",
                column: "coupon_id",
                principalTable: "Coupons",
                principalColumn: "CouponId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_AccountAuth",
                schema: "Service",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Coupons",
                schema: "Service",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_coupon_id",
                schema: "Service",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_sender_id",
                schema: "Service",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "coupon_id",
                schema: "Service",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "sender_id",
                schema: "Service",
                table: "Notifications");

            migrationBuilder.AlterColumn<int>(
                name: "order_id",
                schema: "Service",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
