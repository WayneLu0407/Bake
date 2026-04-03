using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bake.Models.Sales
{
    public class Coupon
    {
        [Key]
        public int CouponId { get; set; }

        [Required]
        [Display(Name = "優惠碼")]
        [StringLength(20)]
        public string Code { get; set; }

        [Display(Name = "賣家ID")]
        public int? SellerId { get; set; }

        [Required]
        [Display(Name = "折扣金額")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal DiscountValue { get; set; }

        [Display(Name = "最低消費門檻")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinimumPurchase { get; set; } = 0;

        [Required]
        [Display(Name = "開始日期")]
        public DateTime StartDate { get; set; } = DateTime.Now.AddMonths(1);

        [Required]
        [Display(Name = "結束日期")]
        public DateTime ExpirationDate { get; set; } = DateTime.Now.AddMonths(1);

        [Display(Name = "是否啟用")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "已使用次數")]
        public int UsedCount { get; set; } = 0;
    }
}
