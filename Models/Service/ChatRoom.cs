using System;
using System.Collections.Generic;

namespace Bake.Models.Service;

public partial class ChatRoom
{
    public int RoomId { get; set; }

    public byte RoomType { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ChatRoomType RoomTypeNavigation { get; set; } = null!;

    public virtual ICollection<ChatRoomMember> ChatRoomMembers { get; set; } = new List<ChatRoomMember>();
    
    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

}
