using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bake.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "User");

            migrationBuilder.EnsureSchema(
                name: "Sales");

            migrationBuilder.EnsureSchema(
                name: "Service");

            migrationBuilder.EnsureSchema(
                name: "Social");

            migrationBuilder.EnsureSchema(
                name: "Platform");

            migrationBuilder.CreateTable(
                name: "Account_Status_Definitions",
                schema: "User",
                columns: table => new
                {
                    status_id = table.Column<byte>(type: "tinyint", nullable: false),
                    status_name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Account___3683B531C403EB5D", x => x.status_id);
                });

            migrationBuilder.CreateTable(
                name: "Cart_Status",
                schema: "Sales",
                columns: table => new
                {
                    status_id = table.Column<byte>(type: "tinyint", nullable: false),
                    status_name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Cart_Sta__3683B531F40F6FF0", x => x.status_id);
                });

            migrationBuilder.CreateTable(
                name: "Chat_Room",
                schema: "Service",
                columns: table => new
                {
                    room_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Chat_Roo__19675A8A5EAF03AF", x => x.room_id);
                });

            migrationBuilder.CreateTable(
                name: "Event_Status_Lookup",
                schema: "Social",
                columns: table => new
                {
                    status_id = table.Column<byte>(type: "tinyint", nullable: false),
                    status_name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Event_St__3683B531FAE7C579", x => x.status_id);
                });

            migrationBuilder.CreateTable(
                name: "Event_Type_Lookup",
                schema: "Social",
                columns: table => new
                {
                    event_type_id = table.Column<byte>(type: "tinyint", nullable: false),
                    event_type_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Event_Ty__BB84C6F37D26A3D8", x => x.event_type_id);
                });

            migrationBuilder.CreateTable(
                name: "Notify_Type",
                schema: "Service",
                columns: table => new
                {
                    status_id = table.Column<byte>(type: "tinyint", nullable: false),
                    status_name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Notify_T__3683B5318DF87776", x => x.status_id);
                });

            migrationBuilder.CreateTable(
                name: "Order_Status",
                schema: "Sales",
                columns: table => new
                {
                    status_id = table.Column<byte>(type: "tinyint", nullable: false),
                    status_name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Order_St__3683B531F0F39FA7", x => x.status_id);
                });

            migrationBuilder.CreateTable(
                name: "Payment_Status_Definitions",
                schema: "Platform",
                columns: table => new
                {
                    status_id = table.Column<byte>(type: "tinyint", nullable: false),
                    status_name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Payment___3683B531856E8AD6", x => x.status_id);
                });

            migrationBuilder.CreateTable(
                name: "Post_Type_Lookup",
                schema: "Social",
                columns: table => new
                {
                    type_id = table.Column<byte>(type: "tinyint", nullable: false),
                    type_name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Post_Typ__2C0005987B9A8CC3", x => x.type_id);
                });

            migrationBuilder.CreateTable(
                name: "Product_Category",
                schema: "Sales",
                columns: table => new
                {
                    category_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    category_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    category_description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    display_order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prodeuct_Category", x => x.category_id);
                });

            migrationBuilder.CreateTable(
                name: "Refund_Status_Definition",
                schema: "Sales",
                columns: table => new
                {
                    status_id = table.Column<byte>(type: "tinyint", nullable: false),
                    status_name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Refund_S__3683B531B6C2B1EB", x => x.status_id);
                });

            migrationBuilder.CreateTable(
                name: "Regist_Status_Lookup",
                schema: "Social",
                columns: table => new
                {
                    reg_status_id = table.Column<byte>(type: "tinyint", nullable: false),
                    reg_status_name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Regist_S__E15399458474C6B6", x => x.reg_status_id);
                });

            migrationBuilder.CreateTable(
                name: "Role_Status_Definitions",
                schema: "User",
                columns: table => new
                {
                    status_id = table.Column<byte>(type: "tinyint", nullable: false),
                    status_name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Role_Sta__3683B5311F0212F8", x => x.status_id);
                });

            migrationBuilder.CreateTable(
                name: "Seller_Wallet_Status_Definitions",
                schema: "Platform",
                columns: table => new
                {
                    status_id = table.Column<byte>(type: "tinyint", nullable: false),
                    status_name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Seller_W__3683B531B5D0DB1F", x => x.status_id);
                });

            migrationBuilder.CreateTable(
                name: "Shop_Status",
                schema: "Sales",
                columns: table => new
                {
                    status_id = table.Column<byte>(type: "tinyint", nullable: false),
                    status_name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Shop_Sta__3683B53184C3F953", x => x.status_id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                schema: "Social",
                columns: table => new
                {
                    tag_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tag_name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Tags__4296A2B63A112AAE", x => x.tag_id);
                });

            migrationBuilder.CreateTable(
                name: "Transaction_Status_Definitions",
                schema: "Platform",
                columns: table => new
                {
                    status_id = table.Column<byte>(type: "tinyint", nullable: false),
                    status_name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Transact__3683B531D5616479", x => x.status_id);
                });

            migrationBuilder.CreateTable(
                name: "User_Gender_Status_Definitions",
                schema: "User",
                columns: table => new
                {
                    status_id = table.Column<byte>(type: "tinyint", nullable: false),
                    status_name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User_Gen__3683B531490A5DAD", x => x.status_id);
                });

            migrationBuilder.CreateTable(
                name: "Account_Auth",
                schema: "User",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    user_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    role = table.Column<byte>(type: "tinyint", nullable: false),
                    account_status = table.Column<byte>(type: "tinyint", nullable: false),
                    is_seller = table.Column<bool>(type: "bit", nullable: false),
                    is_email_confirmed = table.Column<bool>(type: "bit", nullable: false),
                    confirmation_token = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    email_verified_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Account___B9BE370F373FAEB4", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_Account_Auth_Account_Status",
                        column: x => x.account_status,
                        principalSchema: "User",
                        principalTable: "Account_Status_Definitions",
                        principalColumn: "status_id");
                    table.ForeignKey(
                        name: "FK_Account_Auth_Role_Status",
                        column: x => x.role,
                        principalSchema: "User",
                        principalTable: "Role_Status_Definitions",
                        principalColumn: "status_id");
                });

            migrationBuilder.CreateTable(
                name: "Products",
                schema: "Sales",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    product_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    product_image = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    product_method = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    product_description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    product_rating = table.Column<decimal>(type: "decimal(2,1)", nullable: true),
                    product_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    category_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Products__47027DF5901740A0", x => x.product_id);
                    table.ForeignKey(
                        name: "FK_Products_Auth",
                        column: x => x.user_id,
                        principalSchema: "User",
                        principalTable: "Account_Auth",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK_Products_Prodeuct_Category",
                        column: x => x.category_id,
                        principalSchema: "Sales",
                        principalTable: "Product_Category",
                        principalColumn: "category_id");
                });

            migrationBuilder.CreateTable(
                name: "Shop",
                schema: "Sales",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    shop_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    shop_description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    shop_rating = table.Column<decimal>(type: "decimal(2,1)", nullable: false),
                    shop_img = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    shop_time = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())"),
                    seller_approved_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status_id = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Shop__B9BE370F36C4789E", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_Shop_Auth",
                        column: x => x.user_id,
                        principalSchema: "User",
                        principalTable: "Account_Auth",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK_Shop_Status",
                        column: x => x.status_id,
                        principalSchema: "Sales",
                        principalTable: "Shop_Status",
                        principalColumn: "status_id");
                });

            migrationBuilder.CreateTable(
                name: "System_Metadata",
                schema: "User",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    last_login_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    last_login_ip = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: false),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    register_ip = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__System_M__B9BE370F6E92A76A", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_System_Metadata_Auth",
                        column: x => x.user_id,
                        principalSchema: "User",
                        principalTable: "Account_Auth",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "User_Payment_Secrets",
                schema: "Platform",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    encrypted_bank_acc = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User_Pay__B9BE370F7A61D912", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_Secret_Auth",
                        column: x => x.user_id,
                        principalSchema: "User",
                        principalTable: "Account_Auth",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "User_Profile",
                schema: "User",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    persona = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    avatar_url = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    bio = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    user_phone = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    user_gender = table.Column<byte>(type: "tinyint", nullable: false),
                    user_birthdate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User_Pro__B9BE370FA505BCCE", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_User_Profile_Auth",
                        column: x => x.user_id,
                        principalSchema: "User",
                        principalTable: "Account_Auth",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK_User_Profile_Gender",
                        column: x => x.user_gender,
                        principalSchema: "User",
                        principalTable: "User_Gender_Status_Definitions",
                        principalColumn: "status_id");
                });

            migrationBuilder.CreateTable(
                name: "Product_Details",
                schema: "Sales",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "int", nullable: false),
                    product_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    product_discount = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    product_quantity = table.Column<int>(type: "int", nullable: false),
                    expire_date = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Product___47027DF5C980F170", x => x.product_id);
                    table.ForeignKey(
                        name: "FK_Product_Details_Products_product_id",
                        column: x => x.product_id,
                        principalSchema: "Sales",
                        principalTable: "Products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Product_Ingredients",
                schema: "Sales",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "int", nullable: false),
                    shelf_life_note = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ingredients = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    net_weight = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
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

            migrationBuilder.CreateTable(
                name: "Seller_Wallet",
                schema: "Platform",
                columns: table => new
                {
                    payout_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    seller_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    fee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    payout_status = table.Column<byte>(type: "tinyint", nullable: false),
                    bank_ref_id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Seller_W__3B0771EC01AAF1C9", x => x.payout_id);
                    table.ForeignKey(
                        name: "FK_Wallet_Shop",
                        column: x => x.user_id,
                        principalSchema: "Sales",
                        principalTable: "Shop",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK_Wallet_Status",
                        column: x => x.payout_status,
                        principalSchema: "Platform",
                        principalTable: "Seller_Wallet_Status_Definitions",
                        principalColumn: "status_id");
                });

            migrationBuilder.CreateTable(
                name: "Cart",
                schema: "Sales",
                columns: table => new
                {
                    cart_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<byte>(type: "tinyint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Cart__2EF52A278338B2D9", x => x.cart_id);
                    table.ForeignKey(
                        name: "FK_cart_Profile",
                        column: x => x.user_id,
                        principalSchema: "User",
                        principalTable: "User_Profile",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK_cart_Status",
                        column: x => x.status,
                        principalSchema: "Sales",
                        principalTable: "Cart_Status",
                        principalColumn: "status_id");
                });

            migrationBuilder.CreateTable(
                name: "Chat_Message",
                schema: "Service",
                columns: table => new
                {
                    message_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    create_date = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())"),
                    is_read = table.Column<bool>(type: "bit", nullable: false),
                    room_id = table.Column<int>(type: "int", nullable: false),
                    sender_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Chat_Mes__0BBF6EE670CE81C2", x => x.message_id);
                    table.ForeignKey(
                        name: "FK_Message_Profile",
                        column: x => x.sender_id,
                        principalSchema: "User",
                        principalTable: "User_Profile",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Chat_Room_Member",
                schema: "Service",
                columns: table => new
                {
                    room_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    joined_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Chat_Roo__F2FCB9FA646E7D20", x => new { x.room_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_Member_Profile",
                        column: x => x.user_id,
                        principalSchema: "User",
                        principalTable: "User_Profile",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK_Member_Room",
                        column: x => x.room_id,
                        principalSchema: "Service",
                        principalTable: "Chat_Room",
                        principalColumn: "room_id");
                });

            migrationBuilder.CreateTable(
                name: "Follows",
                schema: "Social",
                columns: table => new
                {
                    follower_id = table.Column<int>(type: "int", nullable: false),
                    befollowed_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Follows__ED9DDEC41E3954E9", x => new { x.follower_id, x.befollowed_id });
                    table.ForeignKey(
                        name: "FK_Befollowed_Profile",
                        column: x => x.befollowed_id,
                        principalSchema: "User",
                        principalTable: "User_Profile",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK_Follower_Profile",
                        column: x => x.follower_id,
                        principalSchema: "User",
                        principalTable: "User_Profile",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                schema: "Sales",
                columns: table => new
                {
                    order_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    shipping_address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    total_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    payment_method = table.Column<byte>(type: "tinyint", nullable: false),
                    status_id = table.Column<byte>(type: "tinyint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Orders__4659622948DE6D11", x => x.order_id);
                    table.ForeignKey(
                        name: "FK_Orders_Profile",
                        column: x => x.user_id,
                        principalSchema: "User",
                        principalTable: "User_Profile",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK_Orders_Status",
                        column: x => x.status_id,
                        principalSchema: "Sales",
                        principalTable: "Order_Status",
                        principalColumn: "status_id");
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                schema: "Social",
                columns: table => new
                {
                    post_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    author_id = table.Column<int>(type: "int", nullable: false),
                    type_id = table.Column<byte>(type: "tinyint", nullable: false),
                    title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    view_count = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    likes_count = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    favorite_count = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    is_published = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Posts__3ED78766908ED1D3", x => x.post_id);
                    table.ForeignKey(
                        name: "FK_Posts_Profile",
                        column: x => x.author_id,
                        principalSchema: "User",
                        principalTable: "User_Profile",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK_Posts_Type",
                        column: x => x.type_id,
                        principalSchema: "Social",
                        principalTable: "Post_Type_Lookup",
                        principalColumn: "type_id");
                });

            migrationBuilder.CreateTable(
                name: "Product_Review",
                schema: "Service",
                columns: table => new
                {
                    review_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    order_id = table.Column<int>(type: "int", nullable: false),
                    user_rating = table.Column<byte>(type: "tinyint", nullable: false),
                    comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Product___60883D90B2F109EB", x => x.review_id);
                    table.ForeignKey(
                        name: "FK_Review_Profile",
                        column: x => x.user_id,
                        principalSchema: "User",
                        principalTable: "User_Profile",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "System_Notify",
                schema: "Service",
                columns: table => new
                {
                    notify_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    create_date = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())"),
                    content_text = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    sender_id = table.Column<int>(type: "int", nullable: true),
                    recipient_id = table.Column<int>(type: "int", nullable: false),
                    is_read = table.Column<bool>(type: "bit", nullable: false),
                    notify_type = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__System_N__DD351C96AB33B043", x => x.notify_id);
                    table.ForeignKey(
                        name: "FK_Notify_Recipient",
                        column: x => x.recipient_id,
                        principalSchema: "User",
                        principalTable: "User_Profile",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK_Notify_Sender",
                        column: x => x.sender_id,
                        principalSchema: "User",
                        principalTable: "User_Profile",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK_Notify_Type",
                        column: x => x.notify_type,
                        principalSchema: "Service",
                        principalTable: "Notify_Type",
                        principalColumn: "status_id");
                });

            migrationBuilder.CreateTable(
                name: "CartItem",
                schema: "Sales",
                columns: table => new
                {
                    cart_item_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cart_id = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    product_quantity = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CartItem__5D9A6C6E165A4511", x => x.cart_item_id);
                    table.ForeignKey(
                        name: "FK_cartItem_cart",
                        column: x => x.cart_id,
                        principalSchema: "Sales",
                        principalTable: "Cart",
                        principalColumn: "cart_id");
                });

            migrationBuilder.CreateTable(
                name: "Order_Items",
                schema: "Sales",
                columns: table => new
                {
                    item_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    order_id = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    item_quantity = table.Column<int>(type: "int", nullable: false),
                    unit_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Order_It__52020FDD1BE8135E", x => x.item_id);
                    table.ForeignKey(
                        name: "FK_Order_Items_Orders",
                        column: x => x.order_id,
                        principalSchema: "Sales",
                        principalTable: "Orders",
                        principalColumn: "order_id");
                });

            migrationBuilder.CreateTable(
                name: "Payment_Transactions",
                schema: "Platform",
                columns: table => new
                {
                    transaction_id = table.Column<int>(type: "int", nullable: false),
                    orders_id = table.Column<int>(type: "int", nullable: false),
                    payment_method = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    transaction_status = table.Column<byte>(type: "tinyint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Payment___85C600AF79EB0C0D", x => x.transaction_id);
                    table.ForeignKey(
                        name: "FK_Pay_Orders",
                        column: x => x.orders_id,
                        principalSchema: "Sales",
                        principalTable: "Orders",
                        principalColumn: "order_id");
                    table.ForeignKey(
                        name: "FK_Pay_Status",
                        column: x => x.transaction_status,
                        principalSchema: "Platform",
                        principalTable: "Transaction_Status_Definitions",
                        principalColumn: "status_id");
                });

            migrationBuilder.CreateTable(
                name: "Platform_Escrow_Ledger",
                schema: "Platform",
                columns: table => new
                {
                    ledger_id = table.Column<int>(type: "int", nullable: false),
                    order_id = table.Column<int>(type: "int", nullable: false),
                    held_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    payment_status = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Platform__97EDEDA129A989F6", x => x.ledger_id);
                    table.ForeignKey(
                        name: "FK_Ledger_Orders",
                        column: x => x.order_id,
                        principalSchema: "Sales",
                        principalTable: "Orders",
                        principalColumn: "order_id");
                    table.ForeignKey(
                        name: "FK_Ledger_Status",
                        column: x => x.payment_status,
                        principalSchema: "Platform",
                        principalTable: "Payment_Status_Definitions",
                        principalColumn: "status_id");
                });

            migrationBuilder.CreateTable(
                name: "Refund",
                schema: "Sales",
                columns: table => new
                {
                    refund_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    order_id = table.Column<int>(type: "int", nullable: false),
                    reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    refund_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    refund_status = table.Column<byte>(type: "tinyint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Refund__897E9EA304602809", x => x.refund_id);
                    table.ForeignKey(
                        name: "FK_refund_Orders",
                        column: x => x.order_id,
                        principalSchema: "Sales",
                        principalTable: "Orders",
                        principalColumn: "order_id");
                    table.ForeignKey(
                        name: "FK_refund_Status",
                        column: x => x.refund_status,
                        principalSchema: "Sales",
                        principalTable: "Refund_Status_Definition",
                        principalColumn: "status_id");
                });

            migrationBuilder.CreateTable(
                name: "Event_Details",
                schema: "Social",
                columns: table => new
                {
                    event_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    post_id = table.Column<int>(type: "int", nullable: true),
                    event_type_id = table.Column<byte>(type: "tinyint", nullable: false),
                    manual_status_id = table.Column<byte>(type: "tinyint", nullable: false),
                    price = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    max_participants = table.Column<int>(type: "int", nullable: false),
                    signup_start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    signup_deadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    event_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    event_end_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    location_city = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    location_address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Event_De__2370F7274139FFD8", x => x.event_id);
                    table.ForeignKey(
                        name: "FK_Event_Post",
                        column: x => x.post_id,
                        principalSchema: "Social",
                        principalTable: "Posts",
                        principalColumn: "post_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Event_Status",
                        column: x => x.manual_status_id,
                        principalSchema: "Social",
                        principalTable: "Event_Status_Lookup",
                        principalColumn: "status_id");
                    table.ForeignKey(
                        name: "FK_Event_Type",
                        column: x => x.event_type_id,
                        principalSchema: "Social",
                        principalTable: "Event_Type_Lookup",
                        principalColumn: "event_type_id");
                });

            migrationBuilder.CreateTable(
                name: "Post_Attachments",
                schema: "Social",
                columns: table => new
                {
                    image_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    post_id = table.Column<int>(type: "int", nullable: false),
                    file_url = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    alt_text = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    is_cover = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    sort_order = table.Column<int>(type: "int", nullable: true, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Post_Att__DC9AC955774B700E", x => x.image_id);
                    table.ForeignKey(
                        name: "FK_Attach_Post",
                        column: x => x.post_id,
                        principalSchema: "Social",
                        principalTable: "Posts",
                        principalColumn: "post_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Post_Favorites",
                schema: "Social",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    post_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Post_Fav__CA534F79346BBFD2", x => new { x.user_id, x.post_id });
                    table.ForeignKey(
                        name: "FK_Favorites_Post",
                        column: x => x.post_id,
                        principalSchema: "Social",
                        principalTable: "Posts",
                        principalColumn: "post_id");
                    table.ForeignKey(
                        name: "FK_Favorites_Profile",
                        column: x => x.user_id,
                        principalSchema: "User",
                        principalTable: "User_Profile",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Post_Likes",
                schema: "Social",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    post_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Post_Lik__CA534F79CE46A682", x => new { x.user_id, x.post_id });
                    table.ForeignKey(
                        name: "FK_Likes_Post",
                        column: x => x.post_id,
                        principalSchema: "Social",
                        principalTable: "Posts",
                        principalColumn: "post_id");
                    table.ForeignKey(
                        name: "FK_Likes_Profile",
                        column: x => x.user_id,
                        principalSchema: "User",
                        principalTable: "User_Profile",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Post_Tag_Mapping",
                schema: "Social",
                columns: table => new
                {
                    post_id = table.Column<int>(type: "int", nullable: false),
                    tag_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Post_Tag__4AFEED4D6A1B85AD", x => new { x.post_id, x.tag_id });
                    table.ForeignKey(
                        name: "FK_Mapping_Post",
                        column: x => x.post_id,
                        principalSchema: "Social",
                        principalTable: "Posts",
                        principalColumn: "post_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Mapping_Tag",
                        column: x => x.tag_id,
                        principalSchema: "Social",
                        principalTable: "Tags",
                        principalColumn: "tag_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Event_Registrations",
                schema: "Social",
                columns: table => new
                {
                    registration_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    event_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    num_participants = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    notes = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    regist_status_id = table.Column<byte>(type: "tinyint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Event_Re__22A298F68C7318A2", x => x.registration_id);
                    table.ForeignKey(
                        name: "FK_Reg_Event",
                        column: x => x.event_id,
                        principalSchema: "Social",
                        principalTable: "Event_Details",
                        principalColumn: "event_id");
                    table.ForeignKey(
                        name: "FK_Reg_Profile",
                        column: x => x.user_id,
                        principalSchema: "User",
                        principalTable: "User_Profile",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK_Reg_Status",
                        column: x => x.regist_status_id,
                        principalSchema: "Social",
                        principalTable: "Regist_Status_Lookup",
                        principalColumn: "reg_status_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_Auth_account_status",
                schema: "User",
                table: "Account_Auth",
                column: "account_status");

            migrationBuilder.CreateIndex(
                name: "IX_Account_Auth_role",
                schema: "User",
                table: "Account_Auth",
                column: "role");

            migrationBuilder.CreateIndex(
                name: "UQ__Account___7C9273C4B53441EF",
                schema: "User",
                table: "Account_Auth",
                column: "user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Account___AB6E616446F1839A",
                schema: "User",
                table: "Account_Auth",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cart_status",
                schema: "Sales",
                table: "Cart",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_user_id",
                schema: "Sales",
                table: "Cart",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_cart_id",
                schema: "Sales",
                table: "CartItem",
                column: "cart_id");

            migrationBuilder.CreateIndex(
                name: "IX_Chat_Message_sender_id",
                schema: "Service",
                table: "Chat_Message",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "IX_Chat_Room_Member_user_id",
                schema: "Service",
                table: "Chat_Room_Member",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Event_Details_event_type_id",
                schema: "Social",
                table: "Event_Details",
                column: "event_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_Event_Details_manual_status_id",
                schema: "Social",
                table: "Event_Details",
                column: "manual_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_Event_Details_post_id",
                schema: "Social",
                table: "Event_Details",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "IX_Event_Registrations_event_id",
                schema: "Social",
                table: "Event_Registrations",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_Event_Registrations_regist_status_id",
                schema: "Social",
                table: "Event_Registrations",
                column: "regist_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_Event_Registrations_user_id",
                schema: "Social",
                table: "Event_Registrations",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Follows_befollowed_id",
                schema: "Social",
                table: "Follows",
                column: "befollowed_id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_Items_order_id",
                schema: "Sales",
                table: "Order_Items",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_status_id",
                schema: "Sales",
                table: "Orders",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_user_id",
                schema: "Sales",
                table: "Orders",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_Transactions_orders_id",
                schema: "Platform",
                table: "Payment_Transactions",
                column: "orders_id");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_Transactions_transaction_status",
                schema: "Platform",
                table: "Payment_Transactions",
                column: "transaction_status");

            migrationBuilder.CreateIndex(
                name: "IX_Platform_Escrow_Ledger_order_id",
                schema: "Platform",
                table: "Platform_Escrow_Ledger",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_Platform_Escrow_Ledger_payment_status",
                schema: "Platform",
                table: "Platform_Escrow_Ledger",
                column: "payment_status");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Attachments_post_id",
                schema: "Social",
                table: "Post_Attachments",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Favorites_post_id",
                schema: "Social",
                table: "Post_Favorites",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Likes_post_id",
                schema: "Social",
                table: "Post_Likes",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Tag_Mapping_tag_id",
                schema: "Social",
                table: "Post_Tag_Mapping",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_author_id",
                schema: "Social",
                table: "Posts",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_type_id",
                schema: "Social",
                table: "Posts",
                column: "type_id");

            migrationBuilder.CreateIndex(
                name: "IX_Product_Review_user_id",
                schema: "Service",
                table: "Product_Review",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Products_category_id",
                schema: "Sales",
                table: "Products",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_Products_user_id",
                schema: "Sales",
                table: "Products",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Refund_order_id",
                schema: "Sales",
                table: "Refund",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_Refund_refund_status",
                schema: "Sales",
                table: "Refund",
                column: "refund_status");

            migrationBuilder.CreateIndex(
                name: "IX_Seller_Wallet_payout_status",
                schema: "Platform",
                table: "Seller_Wallet",
                column: "payout_status");

            migrationBuilder.CreateIndex(
                name: "IX_Seller_Wallet_user_id",
                schema: "Platform",
                table: "Seller_Wallet",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Shop_status_id",
                schema: "Sales",
                table: "Shop",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_System_Notify_notify_type",
                schema: "Service",
                table: "System_Notify",
                column: "notify_type");

            migrationBuilder.CreateIndex(
                name: "IX_System_Notify_recipient_id",
                schema: "Service",
                table: "System_Notify",
                column: "recipient_id");

            migrationBuilder.CreateIndex(
                name: "IX_System_Notify_sender_id",
                schema: "Service",
                table: "System_Notify",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Tags__E298655C3498945A",
                schema: "Social",
                table: "Tags",
                column: "tag_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Profile_user_gender",
                schema: "User",
                table: "User_Profile",
                column: "user_gender");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItem",
                schema: "Sales");

            migrationBuilder.DropTable(
                name: "Chat_Message",
                schema: "Service");

            migrationBuilder.DropTable(
                name: "Chat_Room_Member",
                schema: "Service");

            migrationBuilder.DropTable(
                name: "Event_Registrations",
                schema: "Social");

            migrationBuilder.DropTable(
                name: "Follows",
                schema: "Social");

            migrationBuilder.DropTable(
                name: "Order_Items",
                schema: "Sales");

            migrationBuilder.DropTable(
                name: "Payment_Transactions",
                schema: "Platform");

            migrationBuilder.DropTable(
                name: "Platform_Escrow_Ledger",
                schema: "Platform");

            migrationBuilder.DropTable(
                name: "Post_Attachments",
                schema: "Social");

            migrationBuilder.DropTable(
                name: "Post_Favorites",
                schema: "Social");

            migrationBuilder.DropTable(
                name: "Post_Likes",
                schema: "Social");

            migrationBuilder.DropTable(
                name: "Post_Tag_Mapping",
                schema: "Social");

            migrationBuilder.DropTable(
                name: "Product_Details",
                schema: "Sales");

            migrationBuilder.DropTable(
                name: "Product_Ingredients",
                schema: "Sales");

            migrationBuilder.DropTable(
                name: "Product_Review",
                schema: "Service");

            migrationBuilder.DropTable(
                name: "Refund",
                schema: "Sales");

            migrationBuilder.DropTable(
                name: "Seller_Wallet",
                schema: "Platform");

            migrationBuilder.DropTable(
                name: "System_Metadata",
                schema: "User");

            migrationBuilder.DropTable(
                name: "System_Notify",
                schema: "Service");

            migrationBuilder.DropTable(
                name: "User_Payment_Secrets",
                schema: "Platform");

            migrationBuilder.DropTable(
                name: "Cart",
                schema: "Sales");

            migrationBuilder.DropTable(
                name: "Chat_Room",
                schema: "Service");

            migrationBuilder.DropTable(
                name: "Event_Details",
                schema: "Social");

            migrationBuilder.DropTable(
                name: "Regist_Status_Lookup",
                schema: "Social");

            migrationBuilder.DropTable(
                name: "Transaction_Status_Definitions",
                schema: "Platform");

            migrationBuilder.DropTable(
                name: "Payment_Status_Definitions",
                schema: "Platform");

            migrationBuilder.DropTable(
                name: "Tags",
                schema: "Social");

            migrationBuilder.DropTable(
                name: "Products",
                schema: "Sales");

            migrationBuilder.DropTable(
                name: "Orders",
                schema: "Sales");

            migrationBuilder.DropTable(
                name: "Refund_Status_Definition",
                schema: "Sales");

            migrationBuilder.DropTable(
                name: "Shop",
                schema: "Sales");

            migrationBuilder.DropTable(
                name: "Seller_Wallet_Status_Definitions",
                schema: "Platform");

            migrationBuilder.DropTable(
                name: "Notify_Type",
                schema: "Service");

            migrationBuilder.DropTable(
                name: "Cart_Status",
                schema: "Sales");

            migrationBuilder.DropTable(
                name: "Posts",
                schema: "Social");

            migrationBuilder.DropTable(
                name: "Event_Status_Lookup",
                schema: "Social");

            migrationBuilder.DropTable(
                name: "Event_Type_Lookup",
                schema: "Social");

            migrationBuilder.DropTable(
                name: "Product_Category",
                schema: "Sales");

            migrationBuilder.DropTable(
                name: "Order_Status",
                schema: "Sales");

            migrationBuilder.DropTable(
                name: "Shop_Status",
                schema: "Sales");

            migrationBuilder.DropTable(
                name: "User_Profile",
                schema: "User");

            migrationBuilder.DropTable(
                name: "Post_Type_Lookup",
                schema: "Social");

            migrationBuilder.DropTable(
                name: "Account_Auth",
                schema: "User");

            migrationBuilder.DropTable(
                name: "User_Gender_Status_Definitions",
                schema: "User");

            migrationBuilder.DropTable(
                name: "Account_Status_Definitions",
                schema: "User");

            migrationBuilder.DropTable(
                name: "Role_Status_Definitions",
                schema: "User");
        }
    }
}
