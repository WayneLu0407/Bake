using Bake.Models.User;
using System;
using System.Collections.Generic;

namespace Bake.Models.Service;

public partial class ChatRoomMember
{
    public int RoomId { get; set; }

    public int UserId { get; set; }

    public DateTime JoinedAt { get; set; }

    public virtual ChatRoom Room { get; set; } = null!;

    public virtual UserProfile User { get; set; } = null!;
}
