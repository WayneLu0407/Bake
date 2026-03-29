using Bake.Models.User;
using System;
using System.Collections.Generic;

namespace Bake.Models.Service;

public partial class ChatMessage
{
    public int MessageId { get; set; }

    public string Message { get; set; } = null!;

    public DateTime CreateDate { get; set; }

    public bool IsRead { get; set; }

    public int RoomId { get; set; }

    public int SenderId { get; set; }

    public virtual ChatRoom Room { get; set; } = null!;

    public virtual UserProfile Sender { get; set; } = null!;
}
