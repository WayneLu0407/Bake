using System.ComponentModel.DataAnnotations;

namespace Bake.ViewModel
{
    public class PostArticleViewModel
    {
        public int PostId { get; set; }
        public int? PostTypeId { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }

        public string CreatedAtDisplay => CreatedAt.ToString("yyyy-MM-dd HH:mm");
        public DateTime CreatedAt { get; set; }
        public bool IsPublished { get; set; }

        public List<IFormFile>? Images { get; set; }
        public string? CoverImgName { get; set; }
        public List<ExistingImageViewModel>? ExistingImages { get; set; }
    }

    public class ExistingImageViewModel
    {
        public int ImageId { get; set; }
        public string FileUrl { get; set; }
        public bool IsCover { get; set; }
    }
}

