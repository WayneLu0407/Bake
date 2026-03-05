using System;
using System.Collections.Generic;
using Bake.Models;
using Bake.Models.Platform;
using Bake.Models.Sales;
using Bake.Models.Service;
using Bake.Models.Social;
using Bake.Models.User;
using Microsoft.EntityFrameworkCore;

namespace Bake.Data;

public partial class BakeContext : DbContext
{
    public BakeContext()
    {
    }

    public BakeContext(DbContextOptions<BakeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccountAuth> AccountAuths { get; set; }

    public virtual DbSet<AccountStatusDefinition> AccountStatusDefinitions { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<CartStatus> CartStatuses { get; set; }

    public virtual DbSet<ChatMessage> ChatMessages { get; set; }

    public virtual DbSet<ChatRoom> ChatRooms { get; set; }

    public virtual DbSet<ChatRoomMember> ChatRoomMembers { get; set; }

    public virtual DbSet<EventDetail> EventDetails { get; set; }

    public virtual DbSet<EventRegistration> EventRegistrations { get; set; }

    public virtual DbSet<EventStatusLookup> EventStatusLookups { get; set; }

    public virtual DbSet<EventTypeLookup> EventTypeLookups { get; set; }

    public virtual DbSet<Follow> Follows { get; set; }

    public virtual DbSet<NotifyType> NotifyTypes { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<PaymentStatusDefinition> PaymentStatusDefinitions { get; set; }

    public virtual DbSet<PaymentTransaction> PaymentTransactions { get; set; }

    public virtual DbSet<PlatformEscrowLedger> PlatformEscrowLedgers { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostAttachment> PostAttachments { get; set; }

    public virtual DbSet<PostFavorite> PostFavorites { get; set; }

    public virtual DbSet<PostLike> PostLikes { get; set; }

    public virtual DbSet<PostTypeLookup> PostTypeLookups { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<ProductDetail> ProductDetails { get; set; }

    public virtual DbSet<ProductReview> ProductReviews { get; set; }

    public virtual DbSet<Refund> Refunds { get; set; }

    public virtual DbSet<RefundStatusDefinition> RefundStatusDefinitions { get; set; }

    public virtual DbSet<RegistStatusLookup> RegistStatusLookups { get; set; }

    public virtual DbSet<RoleStatusDefinition> RoleStatusDefinitions { get; set; }

    public virtual DbSet<SellerWallet> SellerWallets { get; set; }

    public virtual DbSet<SellerWalletStatusDefinition> SellerWalletStatusDefinitions { get; set; }

    public virtual DbSet<Shop> Shops { get; set; }

    public virtual DbSet<ShopStatus> ShopStatuses { get; set; }

    public virtual DbSet<SystemMetadatum> SystemMetadata { get; set; }

    public virtual DbSet<SystemNotify> SystemNotifies { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<TransactionStatusDefinition> TransactionStatusDefinitions { get; set; }

    public virtual DbSet<UserGenderStatusDefinition> UserGenderStatusDefinitions { get; set; }

    public virtual DbSet<UserPaymentSecret> UserPaymentSecrets { get; set; }

    public virtual DbSet<UserProfile> UserProfiles { get; set; }

    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountAuth>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Account___B9BE370F373FAEB4");

            entity.ToTable("Account_Auth", "User");

            entity.HasIndex(e => e.UserName, "UQ__Account___7C9273C4B53441EF").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Account___AB6E616446F1839A").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.AccountStatus).HasColumnName("account_status");
            entity.Property(e => e.ConfirmationToken)
                .HasMaxLength(100)
                .HasColumnName("confirmation_token");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.EmailVerifiedAt).HasColumnName("email_verified_at");
            entity.Property(e => e.IsEmailConfirmed).HasColumnName("is_email_confirmed");
            entity.Property(e => e.IsSeller).HasColumnName("is_seller");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.Role).HasColumnName("role");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .HasColumnName("user_name");

            entity.HasOne(d => d.AccountStatusNavigation).WithMany(p => p.AccountAuths)
                .HasForeignKey(d => d.AccountStatus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Account_Auth_Account_Status");

            entity.HasOne(d => d.RoleNavigation).WithMany(p => p.AccountAuths)
                .HasForeignKey(d => d.Role)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Account_Auth_Role_Status");
        });

        modelBuilder.Entity<AccountStatusDefinition>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Account___3683B531C403EB5D");

            entity.ToTable("Account_Status_Definitions", "User");

            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(20)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Cart__2EF52A278338B2D9");

            entity.ToTable("Cart", "Sales");

            entity.Property(e => e.CartId).HasColumnName("cart_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.Carts)
                .HasForeignKey(d => d.Status)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_cart_Status");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_cart_Profile");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.CartItemId).HasName("PK__CartItem__5D9A6C6E165A4511");

            entity.ToTable("CartItem", "Sales");

            entity.Property(e => e.CartItemId).HasColumnName("cart_item_id");
            entity.Property(e => e.CartId).HasColumnName("cart_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductQuantity).HasColumnName("product_quantity");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.CartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_cartItem_cart");
        });

        modelBuilder.Entity<CartStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Cart_Sta__3683B531F40F6FF0");

            entity.ToTable("Cart_Status", "Sales");

            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(20)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Chat_Mes__0BBF6EE670CE81C2");

            entity.ToTable("Chat_Message", "Service");

            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("create_date");
            entity.Property(e => e.IsRead).HasColumnName("is_read");
            entity.Property(e => e.Message)
                .HasMaxLength(500)
                .HasColumnName("message");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.SenderId).HasColumnName("sender_id");

            entity.HasOne(d => d.Sender).WithMany(p => p.ChatMessages)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Message_Profile");
        });

        modelBuilder.Entity<ChatRoom>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__Chat_Roo__19675A8A5EAF03AF");

            entity.ToTable("Chat_Room", "Service");

            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
        });

        modelBuilder.Entity<ChatRoomMember>(entity =>
        {
            entity.HasKey(e => new { e.RoomId, e.UserId }).HasName("PK__Chat_Roo__F2FCB9FA646E7D20");

            entity.ToTable("Chat_Room_Member", "Service");

            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.JoinedAt).HasColumnName("joined_at");

            entity.HasOne(d => d.Room).WithMany(p => p.ChatRoomMembers)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Member_Room");

            entity.HasOne(d => d.User).WithMany(p => p.ChatRoomMembers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Member_Profile");
        });

        modelBuilder.Entity<EventDetail>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Event_De__2370F7274139FFD8");

            entity.ToTable("Event_Details", "Social");

            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.EventEndTime).HasColumnName("event_end_time");
            entity.Property(e => e.EventTime).HasColumnName("event_time");
            entity.Property(e => e.EventTypeId).HasColumnName("event_type_id");
            entity.Property(e => e.LocationAddress)
                .HasMaxLength(200)
                .HasColumnName("location_address");
            entity.Property(e => e.LocationCity)
                .HasMaxLength(50)
                .HasColumnName("location_city");
            entity.Property(e => e.ManualStatusId).HasColumnName("manual_status_id");
            entity.Property(e => e.MaxParticipants).HasColumnName("max_participants");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.Price)
                .HasDefaultValue(0)
                .HasColumnName("price");
            entity.Property(e => e.SignupDeadline).HasColumnName("signup_deadline");
            entity.Property(e => e.SignupStart).HasColumnName("signup_start");

            entity.HasOne(d => d.EventType).WithMany(p => p.EventDetails)
                .HasForeignKey(d => d.EventTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Event_Type");

            entity.HasOne(d => d.ManualStatus).WithMany(p => p.EventDetails)
                .HasForeignKey(d => d.ManualStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Event_Status");

            entity.HasOne(d => d.Post).WithMany(p => p.EventDetails)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Event_Post");
        });

        modelBuilder.Entity<EventRegistration>(entity =>
        {
            entity.HasKey(e => e.RegistrationId).HasName("PK__Event_Re__22A298F68C7318A2");

            entity.ToTable("Event_Registrations", "Social");

            entity.Property(e => e.RegistrationId).HasColumnName("registration_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.Notes)
                .HasMaxLength(200)
                .HasColumnName("notes");
            entity.Property(e => e.NumParticipants)
                .HasDefaultValue(1)
                .HasColumnName("num_participants");
            entity.Property(e => e.RegistStatusId).HasColumnName("regist_status_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Event).WithMany(p => p.EventRegistrations)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reg_Event");

            entity.HasOne(d => d.RegistStatus).WithMany(p => p.EventRegistrations)
                .HasForeignKey(d => d.RegistStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reg_Status");

            entity.HasOne(d => d.User).WithMany(p => p.EventRegistrations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reg_Profile");
        });

        modelBuilder.Entity<EventStatusLookup>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Event_St__3683B531FAE7C579");

            entity.ToTable("Event_Status_Lookup", "Social");

            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(20)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<EventTypeLookup>(entity =>
        {
            entity.HasKey(e => e.EventTypeId).HasName("PK__Event_Ty__BB84C6F37D26A3D8");

            entity.ToTable("Event_Type_Lookup", "Social");

            entity.Property(e => e.EventTypeId).HasColumnName("event_type_id");
            entity.Property(e => e.EventTypeName)
                .HasMaxLength(50)
                .HasColumnName("event_type_name");
        });

        modelBuilder.Entity<Follow>(entity =>
        {
            entity.HasKey(e => new { e.FollowerId, e.BefollowedId }).HasName("PK__Follows__ED9DDEC41E3954E9");

            entity.ToTable("Follows", "Social");

            entity.Property(e => e.FollowerId).HasColumnName("follower_id");
            entity.Property(e => e.BefollowedId).HasColumnName("befollowed_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");

            entity.HasOne(d => d.Befollowed).WithMany(p => p.FollowBefolloweds)
                .HasForeignKey(d => d.BefollowedId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Befollowed_Profile");

            entity.HasOne(d => d.Follower).WithMany(p => p.FollowFollowers)
                .HasForeignKey(d => d.FollowerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Follower_Profile");
        });

        modelBuilder.Entity<NotifyType>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Notify_T__3683B5318DF87776");

            entity.ToTable("Notify_Type", "Service");

            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(20)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__4659622948DE6D11");

            entity.ToTable("Orders", "Sales");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.PaymentMethod).HasColumnName("payment_method");
            entity.Property(e => e.ShippingAddress)
                .HasMaxLength(500)
                .HasColumnName("shipping_address");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("total_amount");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Status).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Status");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Profile");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__Order_It__52020FDD1BE8135E");

            entity.ToTable("Order_Items", "Sales");

            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.ItemQuantity).HasColumnName("item_quantity");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Subtotal)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("subtotal");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("unit_price");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Items_Orders");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Order_St__3683B531F0F39FA7");

            entity.ToTable("Order_Status", "Sales");

            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(20)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<PaymentStatusDefinition>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Payment___3683B531856E8AD6");

            entity.ToTable("Payment_Status_Definitions", "Platform");

            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(20)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<PaymentTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Payment___85C600AF79EB0C0D");

            entity.ToTable("Payment_Transactions", "Platform");

            entity.Property(e => e.TransactionId)
                .ValueGeneratedNever()
                .HasColumnName("transaction_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.OrdersId).HasColumnName("orders_id");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .HasColumnName("payment_method");
            entity.Property(e => e.TransactionStatus).HasColumnName("transaction_status");

            entity.HasOne(d => d.Orders).WithMany(p => p.PaymentTransactions)
                .HasForeignKey(d => d.OrdersId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pay_Orders");

            entity.HasOne(d => d.TransactionStatusNavigation).WithMany(p => p.PaymentTransactions)
                .HasForeignKey(d => d.TransactionStatus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pay_Status");
        });

        modelBuilder.Entity<PlatformEscrowLedger>(entity =>
        {
            entity.HasKey(e => e.LedgerId).HasName("PK__Platform__97EDEDA129A989F6");

            entity.ToTable("Platform_Escrow_Ledger", "Platform");

            entity.Property(e => e.LedgerId)
                .ValueGeneratedNever()
                .HasColumnName("ledger_id");
            entity.Property(e => e.HeldAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("held_amount");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.PaymentStatus).HasColumnName("payment_status");

            entity.HasOne(d => d.Order).WithMany(p => p.PlatformEscrowLedgers)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ledger_Orders");

            entity.HasOne(d => d.PaymentStatusNavigation).WithMany(p => p.PlatformEscrowLedgers)
                .HasForeignKey(d => d.PaymentStatus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ledger_Status");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__Posts__3ED78766908ED1D3");

            entity.ToTable("Posts", "Social");

            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.FavoriteCount)
                .HasDefaultValue(0)
                .HasColumnName("favorite_count");
            entity.Property(e => e.IsPublished)
                .HasDefaultValue(true)
                .HasColumnName("is_published");
            entity.Property(e => e.LikesCount)
                .HasDefaultValue(0)
                .HasColumnName("likes_count");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");
            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.ViewCount)
                .HasDefaultValue(0)
                .HasColumnName("view_count");

            entity.HasOne(d => d.Author).WithMany(p => p.Posts)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Posts_Profile");

            entity.HasOne(d => d.Type).WithMany(p => p.Posts)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Posts_Type");

            entity.HasMany(d => d.Tags).WithMany(p => p.Posts)
                .UsingEntity<Dictionary<string, object>>(
                    "PostTagMapping",
                    r => r.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagId")
                        .HasConstraintName("FK_Mapping_Tag"),
                    l => l.HasOne<Post>().WithMany()
                        .HasForeignKey("PostId")
                        .HasConstraintName("FK_Mapping_Post"),
                    j =>
                    {
                        j.HasKey("PostId", "TagId").HasName("PK__Post_Tag__4AFEED4D6A1B85AD");
                        j.ToTable("Post_Tag_Mapping", "Social");
                        j.IndexerProperty<int>("PostId").HasColumnName("post_id");
                        j.IndexerProperty<int>("TagId").HasColumnName("tag_id");
                    });
        });

        modelBuilder.Entity<PostAttachment>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__Post_Att__DC9AC955774B700E");

            entity.ToTable("Post_Attachments", "Social");

            entity.Property(e => e.ImageId).HasColumnName("image_id");
            entity.Property(e => e.AltText)
                .HasMaxLength(100)
                .HasColumnName("alt_text");
            entity.Property(e => e.FileUrl)
                .HasMaxLength(2048)
                .HasColumnName("file_url");
            entity.Property(e => e.IsCover)
                .HasDefaultValue(false)
                .HasColumnName("is_cover");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.SortOrder)
                .HasDefaultValue(0)
                .HasColumnName("sort_order");

            entity.HasOne(d => d.Post).WithMany(p => p.PostAttachments)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK_Attach_Post");
        });

        modelBuilder.Entity<PostFavorite>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.PostId }).HasName("PK__Post_Fav__CA534F79346BBFD2");

            entity.ToTable("Post_Favorites", "Social");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");

            entity.HasOne(d => d.Post).WithMany(p => p.PostFavorites)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Favorites_Post");

            entity.HasOne(d => d.User).WithMany(p => p.PostFavorites)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Favorites_Profile");
        });

        modelBuilder.Entity<PostLike>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.PostId }).HasName("PK__Post_Lik__CA534F79CE46A682");

            entity.ToTable("Post_Likes", "Social");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");

            entity.HasOne(d => d.Post).WithMany(p => p.PostLikes)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Likes_Post");

            entity.HasOne(d => d.User).WithMany(p => p.PostLikes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Likes_Profile");
        });

        modelBuilder.Entity<PostTypeLookup>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__Post_Typ__2C0005987B9A8CC3");

            entity.ToTable("Post_Type_Lookup", "Social");

            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.TypeName)
                .HasMaxLength(20)
                .HasColumnName("type_name");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__47027DF5901740A0");

            entity.ToTable("Products", "Sales");

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.ProductDate)
                .HasColumnType("datetime")
                .HasColumnName("product_date");
            entity.Property(e => e.ProductDescription).HasColumnName("product_description");
            entity.Property(e => e.ProductImage)
                .HasMaxLength(2048)
                .HasColumnName("product_image");
            entity.Property(e => e.ProductMethod)
                .HasMaxLength(50)
                .HasColumnName("product_method");
            entity.Property(e => e.ProductName)
                .HasMaxLength(100)
                .HasColumnName("product_name");
            entity.Property(e => e.ProductRating)
                .HasColumnType("decimal(2, 1)")
                .HasColumnName("product_rating");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Prodeuct_Category");

            entity.HasOne(d => d.User).WithMany(p => p.Products)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Auth");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK_Prodeuct_Category");

            entity.ToTable("Product_Category", "Sales");

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CategoryDescription)
                .HasMaxLength(200)
                .HasColumnName("category_description");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(50)
                .HasColumnName("category_name");
            entity.Property(e => e.DisplayOrder).HasColumnName("display_order");
        });

        modelBuilder.Entity<ProductDetail>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Product___47027DF5C980F170");

            entity.ToTable("Product_Details", "Sales");

            entity.Property(e => e.ProductId)
                .ValueGeneratedNever()
                .HasColumnName("product_id");
            entity.Property(e => e.ExpireDate)
                .HasColumnType("datetime")
                .HasColumnName("expire_date");
            entity.Property(e => e.ProductDiscount)
                .HasColumnType("decimal(3, 2)")
                .HasColumnName("product_discount");
            entity.Property(e => e.ProductPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("product_price");
            entity.Property(e => e.ProductQuantity).HasColumnName("product_quantity");
        });

        modelBuilder.Entity<ProductReview>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Product___60883D90B2F109EB");

            entity.ToTable("Product_Review", "Service");

            entity.Property(e => e.ReviewId).HasColumnName("review_id");
            entity.Property(e => e.Comment)
                .HasMaxLength(1000)
                .HasColumnName("comment");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.UserRating).HasColumnName("user_rating");

            entity.HasOne(d => d.User).WithMany(p => p.ProductReviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Review_Profile");
        });

        modelBuilder.Entity<Refund>(entity =>
        {
            entity.HasKey(e => e.RefundId).HasName("PK__Refund__897E9EA304602809");

            entity.ToTable("Refund", "Sales");

            entity.Property(e => e.RefundId).HasColumnName("refund_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Reason)
                .HasMaxLength(500)
                .HasColumnName("reason");
            entity.Property(e => e.RefundAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("refund_amount");
            entity.Property(e => e.RefundStatus).HasColumnName("refund_status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Order).WithMany(p => p.Refunds)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_refund_Orders");

            entity.HasOne(d => d.RefundStatusNavigation).WithMany(p => p.Refunds)
                .HasForeignKey(d => d.RefundStatus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_refund_Status");
        });

        modelBuilder.Entity<RefundStatusDefinition>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Refund_S__3683B531B6C2B1EB");

            entity.ToTable("Refund_Status_Definition", "Sales");

            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(20)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<RegistStatusLookup>(entity =>
        {
            entity.HasKey(e => e.RegStatusId).HasName("PK__Regist_S__E15399458474C6B6");

            entity.ToTable("Regist_Status_Lookup", "Social");

            entity.Property(e => e.RegStatusId).HasColumnName("reg_status_id");
            entity.Property(e => e.RegStatusName)
                .HasMaxLength(20)
                .HasColumnName("reg_status_name");
        });

        modelBuilder.Entity<RoleStatusDefinition>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Role_Sta__3683B5311F0212F8");

            entity.ToTable("Role_Status_Definitions", "User");

            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(20)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<SellerWallet>(entity =>
        {
            entity.HasKey(e => e.PayoutId).HasName("PK__Seller_W__3B0771EC01AAF1C9");

            entity.ToTable("Seller_Wallet", "Platform");

            entity.Property(e => e.PayoutId)
                .ValueGeneratedNever()
                .HasColumnName("payout_id");
            entity.Property(e => e.BankRefId)
                .HasMaxLength(100)
                .HasColumnName("bank_ref_id");
            entity.Property(e => e.Fee)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("fee");
            entity.Property(e => e.PayoutStatus).HasColumnName("payout_status");
            entity.Property(e => e.SellerAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("seller_amount");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.PayoutStatusNavigation).WithMany(p => p.SellerWallets)
                .HasForeignKey(d => d.PayoutStatus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wallet_Status");

            entity.HasOne(d => d.User).WithMany(p => p.SellerWallets)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wallet_Shop");
        });

        modelBuilder.Entity<SellerWalletStatusDefinition>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Seller_W__3683B531B5D0DB1F");

            entity.ToTable("Seller_Wallet_Status_Definitions", "Platform");

            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(20)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<Shop>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Shop__B9BE370F36C4789E");

            entity.ToTable("Shop", "Sales");

            entity.Property(e => e.UserId)
                .ValueGeneratedOnAdd()
                .HasColumnName("user_id");
            entity.Property(e => e.SellerApprovedAt).HasColumnName("seller_approved_at");
            entity.Property(e => e.ShopDescription).HasColumnName("shop_description");
            entity.Property(e => e.ShopImg)
                .HasMaxLength(2048)
                .HasColumnName("shop_img");
            entity.Property(e => e.ShopName)
                .HasMaxLength(100)
                .HasColumnName("shop_name");
            entity.Property(e => e.ShopRating)
                .HasColumnType("decimal(2, 1)")
                .HasColumnName("shop_rating");
            entity.Property(e => e.ShopTime)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("shop_time");
            entity.Property(e => e.StatusId).HasColumnName("status_id");

            entity.HasOne(d => d.Status).WithMany(p => p.Shops)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Shop_Status");

            entity.HasOne(d => d.User).WithOne(p => p.Shop)
                .HasForeignKey<Shop>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Shop_Auth");
        });

        modelBuilder.Entity<ShopStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Shop_Sta__3683B53184C3F953");

            entity.ToTable("Shop_Status", "Sales");

            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(20)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<SystemMetadatum>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__System_M__B9BE370F6E92A76A");

            entity.ToTable("System_Metadata", "User");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.LastLoginAt).HasColumnName("last_login_at");
            entity.Property(e => e.LastLoginIp)
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasColumnName("last_login_ip");
            entity.Property(e => e.RegisterIp)
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasColumnName("register_ip");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.User).WithOne(p => p.SystemMetadatum)
                .HasForeignKey<SystemMetadatum>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_System_Metadata_Auth");
        });

        modelBuilder.Entity<SystemNotify>(entity =>
        {
            entity.HasKey(e => e.NotifyId).HasName("PK__System_N__DD351C96AB33B043");

            entity.ToTable("System_Notify", "Service");

            entity.Property(e => e.NotifyId).HasColumnName("notify_id");
            entity.Property(e => e.ContentText)
                .HasMaxLength(1000)
                .HasColumnName("content_text");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("create_date");
            entity.Property(e => e.IsRead).HasColumnName("is_read");
            entity.Property(e => e.NotifyType).HasColumnName("notify_type");
            entity.Property(e => e.RecipientId).HasColumnName("recipient_id");
            entity.Property(e => e.SenderId).HasColumnName("sender_id");

            entity.HasOne(d => d.NotifyTypeNavigation).WithMany(p => p.SystemNotifies)
                .HasForeignKey(d => d.NotifyType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notify_Type");

            entity.HasOne(d => d.Recipient).WithMany(p => p.SystemNotifyRecipients)
                .HasForeignKey(d => d.RecipientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notify_Recipient");

            entity.HasOne(d => d.Sender).WithMany(p => p.SystemNotifySenders)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("FK_Notify_Sender");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.TagId).HasName("PK__Tags__4296A2B63A112AAE");

            entity.ToTable("Tags", "Social");

            entity.HasIndex(e => e.TagName, "UQ__Tags__E298655C3498945A").IsUnique();

            entity.Property(e => e.TagId).HasColumnName("tag_id");
            entity.Property(e => e.TagName)
                .HasMaxLength(20)
                .HasColumnName("tag_name");
        });

        modelBuilder.Entity<TransactionStatusDefinition>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Transact__3683B531D5616479");

            entity.ToTable("Transaction_Status_Definitions", "Platform");

            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(20)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<UserGenderStatusDefinition>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__User_Gen__3683B531490A5DAD");

            entity.ToTable("User_Gender_Status_Definitions", "User");

            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(20)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<UserPaymentSecret>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User_Pay__B9BE370F7A61D912");

            entity.ToTable("User_Payment_Secrets", "Platform");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.EncryptedBankAcc).HasColumnName("encrypted_bank_acc");

            entity.HasOne(d => d.User).WithOne(p => p.UserPaymentSecret)
                .HasForeignKey<UserPaymentSecret>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Secret_Auth");
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User_Pro__B9BE370FA505BCCE");

            entity.ToTable("User_Profile", "User");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(2048)
                .HasColumnName("avatar_url");
            entity.Property(e => e.Bio)
                .HasMaxLength(500)
                .HasColumnName("bio");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.Persona)
                .HasMaxLength(50)
                .HasColumnName("persona");
            entity.Property(e => e.UserBirthdate).HasColumnName("user_birthdate");
            entity.Property(e => e.UserGender).HasColumnName("user_gender");
            entity.Property(e => e.UserPhone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("user_phone");

            entity.HasOne(d => d.UserGenderNavigation).WithMany(p => p.UserProfiles)
                .HasForeignKey(d => d.UserGender)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Profile_Gender");

            entity.HasOne(d => d.User).WithOne(p => p.UserProfile)
                .HasForeignKey<UserProfile>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Profile_Auth");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
