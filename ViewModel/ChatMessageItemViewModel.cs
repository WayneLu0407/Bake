namespace Bake.ViewModel
{
    public class ChatMessageItemViewModel
    {
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; }
    }
}