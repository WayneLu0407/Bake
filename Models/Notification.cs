using Bake.Models.Sales;

namespace Bake.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public int OrderId { get; set; }
        public int UserId {  get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string URL { get; set; }
        public bool IsRead { get; set; } = false;

        public DateTime CreateAt { get; set; }
        
        public Order Order { get; set; }
    }
}
