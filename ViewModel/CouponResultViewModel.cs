namespace Bake.ViewModel
{
    public class CouponResultViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public decimal DiscountAmount { get; set; }
    }

    public class CouponApplyRequest
    {
        public string CouponCode { get; set; }
    }
}
