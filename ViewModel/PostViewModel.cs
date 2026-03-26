using Microsoft.AspNetCore.Mvc;
using Bake.Models.Social;
using System.Collections.Generic;

namespace Bake.ViewModels.Social;

/// <summary>
/// 貼文/活動 詳細頁 ViewModel
/// Controller 負責組裝，View 只管顯示
/// </summary>
public class PostDetailViewModel
{
    // ---- 核心資料 ----
    public int PostId { get; set; }
    public byte TypeId { get; set; }               // 1=貼文, 2=活動（對應 PostTypeLookup）
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;    // HTML 內文
    public int ViewCount { get; set; }
    public int LikesCount { get; set; }
    public int FavoriteCount { get; set; }

    /// <summary>TypeId == 1 就是活動</summary>
    public bool IsActivity => TypeId == 1;

    // ---- 圖片 ----
    public List<AttachmentDto> Attachments { get; set; } = new();

    public string? CoverImageUrl => Attachments.FirstOrDefault(x => x.IsCover)?.FileUrl
                                  ?? Attachments.FirstOrDefault()?.FileUrl;

    // ---- 標籤（多對多）----
    public List<TagDto> Tags { get; set; } = new();

    // ---- 活動細節（貼文時為 null）----
    public EventDetailDto? EventDetail { get; set; }

    // ---- 作者 ----
    public AuthorDto Author { get; set; } = null!;

    // ---- 當前登入用戶的互動狀態 ----
    public bool IsLiked { get; set; }
    public bool IsFavorited { get; set; }
    public bool IsFollowed { get; set; }

    // =========== 內部 DTO ===========

    public class AttachmentDto
    {
        public int ImageId { get; set; }
        public string FileUrl { get; set; } = null!;
        public string? AltText { get; set; }
        public bool IsCover { get; set; }
        public int SortOrder { get; set; }
    }

    public class TagDto
    {
        public int TagId { get; set; }
        public string TagName { get; set; } = null!;
    }

    public class EventDetailDto
    {
        public int EventId { get; set; }
        public string EventTypeName { get; set; } = null!;  // 從 EventTypeLookup 拿
        public string StatusName { get; set; } = null!;      // 從 EventStatusLookup 拿
        public int? Price { get; set; }
        public int MaxParticipants { get; set; }
        public int CurrentParticipants { get; set; }         // Count(EventRegistrations)
        public DateTime SignupStart { get; set; }
        public DateTime SignupDeadline { get; set; }
        public DateTime EventTime { get; set; }
        public DateTime EventEndTime { get; set; }
        public string? LocationCity { get; set; }
        public string? LocationAddress { get; set; }
    }

    public class AuthorDto
    {
        public int UserId { get; set; }
        public string DisplayName { get; set; } = null!;
        public string? Avatar { get; set; }
        public string? Bio { get; set; }
        public int ShareCount { get; set; }     // 該作者的貼文數
        public int FollowerCount { get; set; }  // 粉絲數
    }
}