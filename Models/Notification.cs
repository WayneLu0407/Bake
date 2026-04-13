using Bake.Models.Sales;
using Bake.Models.User;

namespace Bake.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public int? OrderId { get; set; }// 改 nullable，優惠券通知時不會有訂單
        public int UserId {  get; set; }
        public int? SenderId { get; set; }       // 新增：賣家 ID（nullable，系統通知沒有寄件者）
        public int? CouponId { get; set; }       // 新增：優惠券 ID（nullable，訂單通知沒有優惠券）
        public string Title { get; set; }
        public string Content { get; set; }
        public string URL { get; set; }
        public bool IsRead { get; set; } = false;

        public DateTime CreateAt { get; set; }
        
        public Order? Order { get; set; }
        public Coupon? Coupon { get; set; }              // 新增
        public AccountAuth? Sender { get; set; }         // 新增
    }
}
