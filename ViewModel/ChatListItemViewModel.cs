namespace Bake.ViewModel
{
    public class ChatListItemViewModel
    {
        public int RoomId { get; set; }
        public string OtherName { get; set; } = string.Empty;
        public string LastMessage { get; set; } = string.Empty;
        public DateTime LastMessageTime { get; set; }
    }
}