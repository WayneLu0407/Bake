using System.ComponentModel.DataAnnotations;

namespace Bake.ViewModel
{
    public class EventApplyViewModel
    {
        public int EventId { get; set; }

        public string EventTitle { get; set; } = string.Empty;

        public DateTime EventTime { get; set; }

        public string LocationCity { get; set; } = string.Empty;

        public string LocationAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "請填寫姓名")]
        [StringLength(50, ErrorMessage = "姓名長度不可超過50字")]
        [Display(Name = "姓名")]
        public string? ApplicantName { get; set; }

        [Required(ErrorMessage = "請填寫手機號碼")]
        [RegularExpression(@"^[0-9+\-\(\)\s]{8,20}$", ErrorMessage = "手機號碼格式不正確")]
        [Display(Name = "手機號碼")]
        public string? ApplicantPhone { get; set; }

        [Required(ErrorMessage = "請填寫電子郵件")]
        [EmailAddress(ErrorMessage = "電子郵件格式不正確")]
        [Display(Name = "電子郵件")]
        public string? ApplicantEmail { get; set; }

        [Display(Name = "性別")]
        public string? GenderText { get; set; }

        [Required(ErrorMessage = "請選擇報名人數")]
        [Range(1, 10, ErrorMessage = "報名人數至少 1 人")]
        [Display(Name = "報名人數")]
        public int NumParticipants { get; set; } = 1;
    }
}