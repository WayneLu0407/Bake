using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Bake.ViewModel
{
    public class EventCreateViewModel
    {
        public string OrganizerName { get; set; } = string.Empty;
        public string? OrganizerAvatarUrl { get; set; }

        [Required(ErrorMessage = "請填寫活動名稱")]
        [StringLength(100, ErrorMessage = "活動名稱不可超過 100 字")]
        [Display(Name = "活動名稱")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "請填寫活動日期")]
        [DataType(DataType.Date)]
        [Display(Name = "活動日期")]
        public DateTime EventDate { get; set; } = DateTime.Today.AddDays(14);

        [Required(ErrorMessage = "請填寫開始時間")]
        [Display(Name = "開始時間")]
        public TimeSpan StartTime { get; set; } = new(13, 0, 0);

        [Required(ErrorMessage = "請填寫結束時間")]
        [Display(Name = "結束時間")]
        public TimeSpan EndTime { get; set; } = new(15, 0, 0);

        [Required(ErrorMessage = "請填寫活動縣市")]
        [StringLength(10, ErrorMessage = "活動縣市不可超過 10 字")]
        [Display(Name = "活動縣市")]
        public string? LocationCity { get; set; } = string.Empty;

        [Required(ErrorMessage = "請填寫活動地點")]
        [StringLength(200, ErrorMessage = "活動地點不可超過 200 字")]
        [Display(Name = "活動地點")]
        public string LocationAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "請填寫活動簡介")]
        [StringLength(3000, ErrorMessage = "活動簡介不可超過 3000 字")]
        [Display(Name = "活動簡介")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "請填寫活動人數")]
        [Range(1, 100, ErrorMessage = "活動人數至少要 1 人")]
        [Display(Name = "活動人數")]
        public int MaxParticipants { get; set; } = 20;

        [Required(ErrorMessage = "請填寫活動費用")]
        [Range(0, 10000, ErrorMessage = "活動費用不可小於 0")]
        [Display(Name = "活動費用")]
        public int Price { get; set; } = 0;

        [Required(ErrorMessage = "請填寫報名開始日")]
        [DataType(DataType.Date)]
        [Display(Name = "報名開始日")]
        public DateTime SignupStartDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "請填寫報名截止日")]
        [DataType(DataType.Date)]
        [Display(Name = "報名截止日")]
        public DateTime SignupEndDate { get; set; } = DateTime.Today.AddDays(7);

        [Display(Name = "備註")]
        [StringLength(500, ErrorMessage = "備註不可超過 500 字")]
        public string? Remark { get; set; }

        [Display(Name = "活動關鍵字")]
        [StringLength(300, ErrorMessage = "關鍵字不可超過 300 字")]
        public string? KeywordsText { get; set; }

        [Display(Name = "照片")]
        public IFormFile? Photo { get; set; }

        [Required(ErrorMessage = "請選擇活動分類")]
        [Display(Name = "活動分類")]
        public byte EventTypeId { get; set; } = 0;

        public List<SelectListItem> EventTypeOptions { get; set; } = new();
    }
}