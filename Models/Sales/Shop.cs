using Bake.Models.Platform;
using Bake.Models.User;
using System;
using System.Collections.Generic;

namespace Bake.Models.Sales;

public partial class Shop
{
    public int UserId { get; set; }

    public string ShopName { get; set; } = null!;

    public string? ShopDescription { get; set; }

    public decimal ShopRating { get; set; }

    public string ShopImg { get; set; } = null!;

    public DateTime ShopTime { get; set; }

    public DateTime SellerApprovedAt { get; set; }

    public byte StatusId { get; set; }

    public string? FacebookUrl { get; set; }

    public string? InstagramUrl { get; set; }

    public string? YoutubeUrl { get; set; }

    public string? PinterestUrl { get; set; }

    public virtual ICollection<SellerWallet> SellerWallets { get; set; } = new List<SellerWallet>();

    public virtual ShopStatus Status { get; set; } = null!;

    public virtual AccountAuth User { get; set; } = null!;
}
