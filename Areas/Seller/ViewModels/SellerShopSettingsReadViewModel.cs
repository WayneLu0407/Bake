using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Bake.Areas.Seller.ViewModels
{    
    public class SellerShopSettingsReadViewModel
    {
        [Display(Name = "店鋪名稱")]
        [Required(ErrorMessage = "請輸入店鋪名稱")]
        [StringLength(100, ErrorMessage = "店鋪名稱最多 100 字")]
        public string ShopName { get; set; } = string.Empty;

        [Display(Name = "店鋪描述")]
        [StringLength(1000, ErrorMessage = "店鋪介紹最多 1000 字")]
        public string? ShopDescription { get; set; }

        public string? ShopImg { get; set; }

        public IFormFile? ShopImageFile { get; set; }

        // 目前先保留顯示用
        public byte? StatusId { get; set; }

            [Display(Name = "連結FB")]
            [StringLength(2048)]
            [Url(ErrorMessage = "FB 連結格式不正確")]
            public string? FacebookUrl { get; set; }

            [Display(Name = "連結IG")]
            [StringLength(2048)]
            [Url(ErrorMessage = "IG 連結格式不正確")]
            public string? InstagramUrl { get; set; }

            [Display(Name = "連結YT")]
            [StringLength(2048)]
            [Url(ErrorMessage = "YT 連結格式不正確")]
            public string? YoutubeUrl { get; set; }

            [Display(Name = "連Pinterest")]
            [StringLength(2048)]
            [Url(ErrorMessage = "Pinterest 連結格式不正確")]
            public string? PinterestUrl { get; set; }
    }
}