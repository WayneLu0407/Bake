namespace Bake.Enum
{
    public enum OrderStatusEnum
    {
        Unpaid = 0,
        Paid = 1,      // 待出貨
        Shipped = 2,   // 已出貨
        Completed = 3, // 已完成
        Cancelled = 4  // 已取消
    }
}
