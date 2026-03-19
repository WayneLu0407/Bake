namespace Bake.ViewModel
{
    public class ChatRoomPageViewModel
    {
        public int RoomId { get; set; }
        public string RoomTitle { get; set; } = string.Empty;
        public List<ChatMessageItemViewModel> Messages { get; set; } = new();
    }
}