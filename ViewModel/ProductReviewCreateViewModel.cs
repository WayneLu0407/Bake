namespace Bake.ViewModel
{
    public class ProductReviewCreateViewModel
    {
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public byte UserRating { get; set; } = 5;  // 先預設初始值
        public string Comment { get; set; } = "";
    }
}
