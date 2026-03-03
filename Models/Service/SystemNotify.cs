using Bake.Models.User;
using System;
using System.Collections.Generic;

namespace Bake.Models.Service;

public partial class SystemNotify
{
    public int NotifyId { get; set; }

    public DateTime CreateDate { get; set; }

    public string ContentText { get; set; } = null!;

    public int? SenderId { get; set; }

    public int RecipientId { get; set; }

    public bool IsRead { get; set; }

    public byte NotifyType { get; set; }

    public virtual NotifyType NotifyTypeNavigation { get; set; } = null!;

    public virtual UserProfile Recipient { get; set; } = null!;

    public virtual UserProfile? Sender { get; set; }
}
