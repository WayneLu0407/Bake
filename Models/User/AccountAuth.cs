using Bake.Models.Platform;
using Bake.Models.Sales;
using System;
using System.Collections.Generic;

namespace Bake.Models.User;

public partial class AccountAuth
{
    public int UserId { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public byte Role { get; set; }

    public byte AccountStatus { get; set; }

    public bool IsSeller { get; set; }

    public bool IsEmailConfirmed { get; set; }

    public string? ConfirmationToken { get; set; }

    public DateTime? EmailVerifiedAt { get; set; }

    public virtual AccountStatusDefinition AccountStatusNavigation { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual RoleStatusDefinition RoleNavigation { get; set; } = null!;

    public virtual Shop? Shop { get; set; }

    public virtual SystemMetadatum? SystemMetadatum { get; set; }

    public virtual UserPaymentSecret? UserPaymentSecret { get; set; }

    public virtual UserProfile? UserProfile { get; set; }
}
