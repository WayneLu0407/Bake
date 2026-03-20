using System;
using System.Collections.Generic;

namespace Bake.Models.Service;
public partial class ChatRoomType
{
    public byte TypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<ChatRoom> ChatRooms { get; set; } = new List<ChatRoom>();
}