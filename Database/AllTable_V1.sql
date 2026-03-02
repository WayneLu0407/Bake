/*
================================================================
專案名稱：烘焙平台全站整合腳本.v1
資料庫名稱：Bake
模組總計：5 大模組 (Platform, Sales, Service, Social, User)
資料表總計：44 張
================================================================
*/

USE [Bake];
GO

-- =============================================
-- 1. [User] 模組 (會員與權限)
-- =============================================

CREATE TABLE [User].[Account_Status_Definitions] (
    [status_id] TINYINT PRIMARY KEY,
    [status_name] NVARCHAR(20) NOT NULL
);

CREATE TABLE [User].[Role_Status_Definitions] (
    [status_id] TINYINT PRIMARY KEY,
    [status_name] NVARCHAR(20) NOT NULL
);

CREATE TABLE [User].[Account_Auth] (
    [user_id] INT IDENTITY(1,1) PRIMARY KEY,
    [email] NVARCHAR(255) NOT NULL UNIQUE,
    [password_hash] NVARCHAR(MAX) NOT NULL,
    [user_name] NVARCHAR(50) NOT NULL UNIQUE,
    [role] TINYINT NOT NULL DEFAULT (0),
    [account_status] TINYINT NOT NULL DEFAULT (0),
    [is_seller] BIT NOT NULL DEFAULT (0),
    [is_email_confirmed] BIT NOT NULL DEFAULT (0),
    [confirmation_token] NVARCHAR(100) NOT NULL,
    [email_verified_at] DATETIME2(7) NULL,
    CONSTRAINT [FK_Account_Auth_Account_Status] FOREIGN KEY([account_status]) REFERENCES [User].[Account_Status_Definitions] ([status_id]),
    CONSTRAINT [FK_Account_Auth_Role_Status] FOREIGN KEY([role]) REFERENCES [User].[Role_Status_Definitions] ([status_id])
);

CREATE TABLE [User].[User_Gender_Status_Definitions] (
    [status_id] TINYINT PRIMARY KEY,
    [status_name] NVARCHAR(20) NOT NULL
);

CREATE TABLE [User].[User_Profile] (
    [user_id] INT PRIMARY KEY,
    [full_name] NVARCHAR(100) NOT NULL,
    [persona] NVARCHAR(50) NOT NULL,
    [avatar_url] NVARCHAR(2048) NOT NULL,
    [bio] NVARCHAR(500) NULL,
    [user_phone] VARCHAR(20) NOT NULL,
    [user_gender] TINYINT NOT NULL DEFAULT (0),
    [user_birthdate] DATETIME2(7) NOT NULL,
    CONSTRAINT [FK_User_Profile_Auth] FOREIGN KEY([user_id]) REFERENCES [User].[Account_Auth] ([user_id]),
    CONSTRAINT [FK_User_Profile_Gender] FOREIGN KEY([user_gender]) REFERENCES [User].[User_Gender_Status_Definitions] ([status_id])
);

CREATE TABLE [User].[System_Metadata] (
    [user_id] INT PRIMARY KEY,
    [created_at] DATETIME2(7) NOT NULL DEFAULT (SYSDATETIME()),
    [updated_at] DATETIME2(7) NOT NULL,
    [last_login_at] DATETIME2(7) NOT NULL,
    [last_login_ip] VARCHAR(45) NOT NULL,
    [deleted_at] DATETIME2(7) NOT NULL,
    [register_ip] VARCHAR(45) NOT NULL,
    CONSTRAINT [FK_System_Metadata_Auth] FOREIGN KEY([user_id]) REFERENCES [User].[Account_Auth] ([user_id])
);

-- =============================================
-- 2. Sales 模組 (商店與電子商務)
-- =============================================

CREATE TABLE [Sales].[Shop_Status] (
    [status_id] TINYINT PRIMARY KEY,
    [status_name] NVARCHAR(20) NOT NULL
);

CREATE TABLE [Sales].[Shop] (
    [user_id] INT IDENTITY(1,1) PRIMARY KEY,
    [shop_name] NVARCHAR(100) NOT NULL,
    [shop_description] NVARCHAR(MAX) NULL,
    [shop_rating] DECIMAL(2, 1) NOT NULL,
    [shop_img] NVARCHAR(2048) NOT NULL,
    [shop_time] DATETIME2(7) NOT NULL DEFAULT (SYSDATETIME()),
    [seller_approved_at] DATETIME2(7) NOT NULL,
    [status_id] TINYINT NOT NULL,
    CONSTRAINT [FK_Shop_Auth] FOREIGN KEY([user_id]) REFERENCES [User].[Account_Auth] ([user_id]),
    CONSTRAINT [FK_Shop_Status] FOREIGN KEY([status_id]) REFERENCES [Sales].[Shop_status] ([status_id])
);

CREATE TABLE [Sales].[Products] (
    [product_id] INT IDENTITY(1,1) PRIMARY KEY,
    [user_id] INT NOT NULL,
    [product_name] NVARCHAR(100) NOT NULL,
    [product_image] NVARCHAR(2048) NULL,
    [product_method] NVARCHAR(50) NOT NULL,
    [product_description] NVARCHAR(MAX) NULL,
    [product_rating] DECIMAL(2, 1) NULL,
    [product_date] DATETIME NOT NULL,
    CONSTRAINT [FK_Products_Auth] FOREIGN KEY([user_id]) REFERENCES [User].[Account_Auth] ([user_id])
);

CREATE TABLE [Sales].[Product_Details] (
    [product_id] INT PRIMARY KEY,
    [product_price] DECIMAL(18, 2) NOT NULL,
    [product_discount] DECIMAL(3, 2) NULL,
    [product_quantity] INT NOT NULL,
    [expire_date] DATETIME NOT NULL,
    CONSTRAINT [FK_Product_Details_Products] FOREIGN KEY([product_id]) REFERENCES [Sales].[Products] ([product_id])
);

CREATE TABLE [Sales].[Cart_Status] (
    [status_id] TINYINT PRIMARY KEY,
    [status_name] NVARCHAR(20) NOT NULL
);

CREATE TABLE [Sales].[Cart] (
    [cart_id] INT IDENTITY(1,1) PRIMARY KEY,
    [user_id] INT NOT NULL,
    [status] TINYINT NOT NULL DEFAULT (0),
    [created_at] DATETIME2(7) NOT NULL DEFAULT (SYSDATETIME()),
    [updated_at] DATETIME2(7) NOT NULL DEFAULT (SYSDATETIME()),
    CONSTRAINT [FK_cart_Profile] FOREIGN KEY([user_id]) REFERENCES [User].[User_Profile] ([user_id]),
    CONSTRAINT [FK_cart_Status] FOREIGN KEY([status]) REFERENCES [Sales].[cart_status] ([status_id])
);

CREATE TABLE [Sales].[CartItem] (
    [cart_item_id] INT IDENTITY(1,1) PRIMARY KEY,
    [cart_id] INT NOT NULL,
    [product_id] INT NOT NULL,
    [product_quantity] INT NOT NULL,
    [created_at] DATETIME2(7) NOT NULL DEFAULT (SYSDATETIME()),
    [updated_at] DATETIME2(7) NOT NULL DEFAULT (SYSDATETIME()),
    CONSTRAINT [FK_cartItem_cart] FOREIGN KEY([cart_id]) REFERENCES [Sales].[cart] ([cart_id]),
    CONSTRAINT [FK_cartItem_Products] FOREIGN KEY([product_id]) REFERENCES [Sales].[Products] ([product_id]),
    CONSTRAINT [CK_product_quantity_Positive] CHECK ([product_quantity] > (0))
);

CREATE TABLE [Sales].[Order_Status] (
    [status_id] TINYINT PRIMARY KEY,
    [status_name] NVARCHAR(20) NOT NULL
);

CREATE TABLE [Sales].[Orders] (
    [order_id] INT IDENTITY(1,1) PRIMARY KEY,
    [user_id] INT NOT NULL,
    [shipping_address] NVARCHAR(500) NOT NULL,
    [total_amount] DECIMAL(18, 2) NOT NULL,
    [payment_method] TINYINT NOT NULL,
    [status_id] TINYINT NOT NULL DEFAULT (0),
    [created_at] DATETIME2(7) NOT NULL,
    [updated_at] DATETIME2(7) NOT NULL,
    CONSTRAINT [FK_Orders_Profile] FOREIGN KEY([user_id]) REFERENCES [User].[User_Profile] ([user_id]),
    CONSTRAINT [FK_Orders_Status] FOREIGN KEY([status_id]) REFERENCES [Sales].[Order_Status] ([status_id])
);

CREATE TABLE [Sales].[Order_Items] (
    [item_id] INT IDENTITY(1,1) PRIMARY KEY,
    [order_id] INT NOT NULL,
    [product_id] INT NOT NULL,
    [item_quantity] INT NOT NULL,
    [unit_price] DECIMAL(18, 2) NOT NULL,
    [subtotal] DECIMAL(18, 2) NOT NULL,
    CONSTRAINT [FK_Order_Items_Orders] FOREIGN KEY([order_id]) REFERENCES [Sales].[Orders] ([order_id]),
    CONSTRAINT [FK_Order_Items_Products] FOREIGN KEY([product_id]) REFERENCES [Sales].[Products] ([product_id])
);

CREATE TABLE [Sales].[Refund_Status_Definition] (
    [status_id] TINYINT PRIMARY KEY DEFAULT (0),
    [status_name] NVARCHAR(20) NOT NULL
);

CREATE TABLE [Sales].[Refund] (
    [refund_id] INT IDENTITY(1,1) PRIMARY KEY,
    [order_id] INT NOT NULL,
    [reason] NVARCHAR(500) NULL,
    [refund_amount] DECIMAL(18, 2) NOT NULL,
    [refund_status] TINYINT NOT NULL,
    [created_at] DATETIME2(7) NULL DEFAULT (SYSDATETIME()),
    [updated_at] DATETIME2(7) NOT NULL DEFAULT (SYSDATETIME()),
    CONSTRAINT [FK_refund_Orders] FOREIGN KEY([order_id]) REFERENCES [Sales].[Orders] ([order_id]),
    CONSTRAINT [FK_refund_Status] FOREIGN KEY([refund_status]) REFERENCES [Sales].[refund_status_definition] ([status_id]),
    CONSTRAINT [CK_refund_amount_Positive] CHECK ([refund_amount] > (0))
);

-- =============================================
-- 3. Social 模組 (社群、揪團與追蹤)
-- =============================================

CREATE TABLE [Social].[Post_Type_Lookup] (
    [type_id] TINYINT PRIMARY KEY,
    [type_name] NVARCHAR(20) NOT NULL
);

CREATE TABLE [Social].[Posts] (
    [post_id] INT IDENTITY(1,1) PRIMARY KEY,
    [author_id] INT NOT NULL,
    [type_id] TINYINT NOT NULL DEFAULT (0),
    [title] NVARCHAR(100) NOT NULL,
    [content] NVARCHAR(MAX) NOT NULL,
    [view_count] INT NULL DEFAULT (0),
    [likes_count] INT NULL DEFAULT (0),
    [favorite_count] INT NULL DEFAULT (0),
    [is_published] BIT NULL DEFAULT (1),
    [created_at] DATETIME2(7) NULL DEFAULT (GETDATE()),
    CONSTRAINT [FK_Posts_Profile] FOREIGN KEY([author_id]) REFERENCES [User].[User_Profile] ([user_id]),
    CONSTRAINT [FK_Posts_Type] FOREIGN KEY([type_id]) REFERENCES [Social].[Post_Type_Lookup] ([type_id])
);

CREATE TABLE [Social].[Post_Attachments] (
    [image_id] INT IDENTITY(1,1) PRIMARY KEY,
    [post_id] INT NOT NULL,
    [file_url] NVARCHAR(2048) NOT NULL,
    [alt_text] NVARCHAR(100) NULL,
    [is_cover] BIT NULL DEFAULT (0),
    [sort_order] INT NULL DEFAULT (0),
    CONSTRAINT [FK_Attach_Post] FOREIGN KEY([post_id]) REFERENCES [Social].[Posts] ([post_id]) ON DELETE CASCADE
);

CREATE TABLE [Social].[Tags] (
    [tag_id] INT IDENTITY(1,1) PRIMARY KEY,
    [tag_name] NVARCHAR(20) NOT NULL UNIQUE,
    CONSTRAINT [CK_Tag_NotEmpty] CHECK (LEN([tag_name]) >= (1))
);

CREATE TABLE [Social].[Post_Tag_Mapping] (
    [post_id] INT NOT NULL,
    [tag_id] INT NOT NULL,
    PRIMARY KEY ([post_id], [tag_id]),
    CONSTRAINT [FK_Mapping_Post] FOREIGN KEY([post_id]) REFERENCES [Social].[Posts] ([post_id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Mapping_Tag] FOREIGN KEY([tag_id]) REFERENCES [Social].[Tags] ([tag_id]) ON DELETE CASCADE
);

CREATE TABLE [Social].[Post_Likes] (
    [user_id] INT NOT NULL,
    [post_id] INT NOT NULL,
    PRIMARY KEY ([user_id], [post_id]),
    [created_at] DATETIME2(7) NULL DEFAULT (GETDATE()),
    CONSTRAINT [FK_Likes_Profile] FOREIGN KEY([user_id]) REFERENCES [User].[User_Profile] ([user_id]),
    CONSTRAINT [FK_Likes_Post] FOREIGN KEY([post_id]) REFERENCES [Social].[Posts] ([post_id])
);

CREATE TABLE [Social].[Post_Favorites] (
    [user_id] INT NOT NULL,
    [post_id] INT NOT NULL,
    PRIMARY KEY ([user_id], [post_id]),
    [created_at] DATETIME2(7) NULL DEFAULT (GETDATE()),
    CONSTRAINT [FK_Favorites_Profile] FOREIGN KEY([user_id]) REFERENCES [User].[User_Profile] ([user_id]),
    CONSTRAINT [FK_Favorites_Post] FOREIGN KEY([post_id]) REFERENCES [Social].[Posts] ([post_id])
);

CREATE TABLE [Social].[Follows] (
    [follower_id] INT NOT NULL,
    [befollowed_id] INT NOT NULL,
    PRIMARY KEY ([follower_id], [befollowed_id]),
    [created_at] DATETIME2(7) NULL DEFAULT (GETDATE()),
    CONSTRAINT [FK_Follower_Profile] FOREIGN KEY([follower_id]) REFERENCES [User].[User_Profile] ([user_id]),
    CONSTRAINT [FK_Befollowed_Profile] FOREIGN KEY([befollowed_id]) REFERENCES [User].[User_Profile] ([user_id])
);

CREATE TABLE [Social].[Event_Type_Lookup] (
    [event_type_id] TINYINT PRIMARY KEY,
    [event_type_name] NVARCHAR(50) NOT NULL
);

CREATE TABLE [Social].[Event_Status_Lookup] (
    [status_id] TINYINT PRIMARY KEY,
    [status_name] NVARCHAR(20) NOT NULL
);

CREATE TABLE [Social].[Event_Details] (
    [event_id] INT IDENTITY(1,1) PRIMARY KEY,
    [post_id] INT NULL,
    [event_type_id] TINYINT NOT NULL,
    [manual_status_id] TINYINT NOT NULL DEFAULT (0),
    [price] INT NULL DEFAULT (0),
    [max_participants] INT NOT NULL,
    [signup_start] DATETIME2(7) NOT NULL,
    [signup_deadline] DATETIME2(7) NOT NULL,
    [event_time] DATETIME2(7) NOT NULL,
    [event_end_time] DATETIME2(7) NOT NULL,
    [location_city] NVARCHAR(50) NULL,
    [location_address] NVARCHAR(200) NULL,
    CONSTRAINT [FK_Event_Post] FOREIGN KEY([post_id]) REFERENCES [Social].[Posts] ([post_id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Event_Type] FOREIGN KEY([event_type_id]) REFERENCES [Social].[Event_Type_Lookup] ([event_type_id]),
    CONSTRAINT [FK_Event_Status] FOREIGN KEY([manual_status_id]) REFERENCES [Social].[Event_Status_Lookup] ([status_id]),
    CONSTRAINT [CK_Event_Duration] CHECK ([event_time] < [event_end_time]),
    CONSTRAINT [CK_Event_Timeline] CHECK ([signup_deadline] < [event_time]),
    CONSTRAINT [CK_Signup_Period] CHECK ([signup_start] < [signup_deadline]),
    CONSTRAINT [CK_MaxParticipants] CHECK ([max_participants] > (0)),
    CONSTRAINT [CK_Price_NonNegative] CHECK ([price] >= (0))
);

CREATE TABLE [Social].[Regist_Status_Lookup] (
    [reg_status_id] TINYINT PRIMARY KEY,
    [reg_status_name] NVARCHAR(20) NOT NULL
);

CREATE TABLE [Social].[Event_Registrations] (
    [registration_id] INT IDENTITY(1,1) PRIMARY KEY,
    [event_id] INT NOT NULL,
    [user_id] INT NOT NULL,
    [num_participants] INT NOT NULL DEFAULT (1),
    [notes] NVARCHAR(200) NULL,
    [regist_status_id] TINYINT NOT NULL DEFAULT (0),
    [created_at] DATETIME2(7) NULL DEFAULT (GETDATE()),
    CONSTRAINT [FK_Reg_Event] FOREIGN KEY([event_id]) REFERENCES [Social].[event_details] ([event_id]),
    CONSTRAINT [FK_Reg_Profile] FOREIGN KEY([user_id]) REFERENCES [User].[User_Profile] ([user_id]),
    CONSTRAINT [FK_Reg_Status] FOREIGN KEY([regist_status_id]) REFERENCES [Social].[Regist_Status_Lookup] ([reg_status_id]),
    CONSTRAINT [CK_Reg_Num_Positive] CHECK ([num_participants] > (0))
);

-- =============================================
-- 4. Service 模組 (通訊、評價與系統通知)
-- =============================================

CREATE TABLE [Service].[Chat_Room] (
    [room_id] INT IDENTITY(1,1) PRIMARY KEY,
    [created_at] DATETIME2(7) NOT NULL DEFAULT (SYSDATETIME())
);

CREATE TABLE [Service].[Chat_Room_Member] (
    [room_id] INT NOT NULL,
    [user_id] INT NOT NULL,
    PRIMARY KEY ([room_id], [user_id]),
    [joined_at] DATETIME2(7) NOT NULL,
    CONSTRAINT [FK_Member_Room] FOREIGN KEY([room_id]) REFERENCES [Service].[chat_room] ([room_id]),
    CONSTRAINT [FK_Member_Profile] FOREIGN KEY([user_id]) REFERENCES [User].[User_Profile] ([user_id])
);

CREATE TABLE [Service].[Chat_Message] (
    [message_id] INT IDENTITY(1,1) PRIMARY KEY,
    [message] NVARCHAR(500) NOT NULL,
    [create_date] DATETIME2(7) NOT NULL DEFAULT (SYSDATETIME()),
    [is_read] BIT NOT NULL DEFAULT (0),
    [room_id] INT NOT NULL,
    [sender_id] INT NOT NULL,
    CONSTRAINT [FK_Message_Profile] FOREIGN KEY([sender_id]) REFERENCES [User].[User_Profile] ([user_id])
);

CREATE TABLE [Service].[Product_Review] (
    [review_id] INT IDENTITY(1,1) PRIMARY KEY,
    [product_id] INT NOT NULL,
    [user_id] INT NOT NULL,
    [order_id] INT NOT NULL,
    [user_rating] TINYINT NOT NULL DEFAULT (0),
    [comment] NVARCHAR(1000) NOT NULL,
    [created_at] DATETIME2(7) NOT NULL DEFAULT (SYSDATETIME()),
    CONSTRAINT [FK_Review_Products] FOREIGN KEY([product_id]) REFERENCES [Sales].[Products] ([product_id]),
    CONSTRAINT [FK_Review_Profile] FOREIGN KEY([user_id]) REFERENCES [User].[User_Profile] ([user_id]),
    CONSTRAINT [CK_Rating_Range] CHECK ([user_rating] >= (1) AND [user_rating] <= (5))
);

CREATE TABLE [Service].[Notify_Type] (
    [status_id] TINYINT PRIMARY KEY,
    [status_name] NVARCHAR(20) NOT NULL
);

CREATE TABLE [Service].[System_Notify] (
    [notify_id] INT IDENTITY(1,1) PRIMARY KEY,
    [create_date] DATETIME2(7) NOT NULL DEFAULT (SYSDATETIME()),
    [content_text] NVARCHAR(1000) NOT NULL,
    [sender_id] INT NULL,
    [recipient_id] INT NOT NULL,
    [is_read] BIT NOT NULL DEFAULT (0),
    [notify_type] TINYINT NOT NULL DEFAULT (0),
    CONSTRAINT [FK_Notify_Type] FOREIGN KEY([notify_type]) REFERENCES [Service].[notify_type] ([status_id]),
    CONSTRAINT [FK_Notify_Recipient] FOREIGN KEY([recipient_id]) REFERENCES [User].[User_Profile] ([user_id]),
    CONSTRAINT [FK_Notify_Sender] FOREIGN KEY([sender_id]) REFERENCES [User].[User_Profile] ([user_id])
);

-- =============================================
-- 5. Platform 模組 (金流與平台營運)
-- =============================================

CREATE TABLE [Platform].[Payment_Status_Definitions] (
    [status_id] TINYINT PRIMARY KEY,
    [status_name] NVARCHAR(20) NOT NULL
);

CREATE TABLE [Platform].[Transaction_Status_Definitions] (
    [status_id] TINYINT PRIMARY KEY,
    [status_name] NVARCHAR(20) NOT NULL
);

CREATE TABLE [Platform].[Payment_Transactions] (
    [transaction_id] INT PRIMARY KEY,
    [orders_id] INT NOT NULL,
    [payment_method] NVARCHAR(50) NOT NULL,
    [transaction_status] TINYINT NOT NULL DEFAULT (0),
    [created_at] DATETIME2(7) NOT NULL,
    CONSTRAINT [FK_Pay_Orders] FOREIGN KEY([orders_id]) REFERENCES [Sales].[Orders] ([order_id]),
    CONSTRAINT [FK_Pay_Status] FOREIGN KEY([transaction_status]) REFERENCES [Platform].[Transaction_Status_Definitions] ([status_id])
);

CREATE TABLE [Platform].[Platform_Escrow_Ledger] (
    [ledger_id] INT PRIMARY KEY,
    [order_id] INT NOT NULL,
    [held_amount] DECIMAL(18, 2) NOT NULL,
    [payment_status] TINYINT NOT NULL DEFAULT (0),
    CONSTRAINT [FK_Ledger_Orders] FOREIGN KEY([order_id]) REFERENCES [Sales].[Orders] ([order_id]),
    CONSTRAINT [FK_Ledger_Status] FOREIGN KEY([payment_status]) REFERENCES [Platform].[Payment_Status_Definitions] ([status_id])
);

CREATE TABLE [Platform].[Seller_Wallet_Status_Definitions] (
    [status_id] TINYINT PRIMARY KEY,
    [status_name] NVARCHAR(20) NOT NULL
);

CREATE TABLE [Platform].[Seller_Wallet] (
    [payout_id] INT PRIMARY KEY,
    [user_id] INT NOT NULL,
    [seller_amount] DECIMAL(18, 2) NOT NULL,
    [fee] DECIMAL(18, 2) NOT NULL,
    [payout_status] TINYINT NOT NULL DEFAULT (0),
    [bank_ref_id] NVARCHAR(100) NOT NULL,
    CONSTRAINT [FK_Wallet_Shop] FOREIGN KEY([user_id]) REFERENCES [Sales].[Shop] ([user_id]),
    CONSTRAINT [FK_Wallet_Status] FOREIGN KEY([payout_status]) REFERENCES [Platform].[Seller_Wallet_Status_Definitions] ([status_id])
);

CREATE TABLE [Platform].[User_Payment_Secrets] (
    [user_id] INT PRIMARY KEY,
    [encrypted_bank_acc] VARBINARY(MAX) NOT NULL,
    CONSTRAINT [FK_Secret_Auth] FOREIGN KEY([user_id]) REFERENCES [User].[Account_Auth] ([user_id])
);
GO