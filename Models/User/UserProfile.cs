using Bake.Models.Sales;
using Bake.Models.Service;
using Bake.Models.Social;
using System;
using System.Collections.Generic;

namespace Bake.Models.User;

public partial class UserProfile
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Persona { get; set; } = null!;

    public string AvatarUrl { get; set; } = null!;

    public string? Bio { get; set; }

    public string UserPhone { get; set; } = null!;

    public byte UserGender { get; set; }

    public DateTime UserBirthdate { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

    public virtual ICollection<ChatRoomMember> ChatRoomMembers { get; set; } = new List<ChatRoomMember>();

    public virtual ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();

    public virtual ICollection<Follow> FollowBefolloweds { get; set; } = new List<Follow>();

    public virtual ICollection<Follow> FollowFollowers { get; set; } = new List<Follow>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<PostFavorite> PostFavorites { get; set; } = new List<PostFavorite>();

    public virtual ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();

    public virtual ICollection<SystemNotify> SystemNotifyRecipients { get; set; } = new List<SystemNotify>();

    public virtual ICollection<SystemNotify> SystemNotifySenders { get; set; } = new List<SystemNotify>();

    public virtual AccountAuth User { get; set; } = null!;

    public virtual UserGenderStatusDefinition UserGenderNavigation { get; set; } = null!;
}
