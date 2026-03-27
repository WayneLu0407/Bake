using Bake.Data;
using Bake.Models.Social;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bake.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostApiController : ControllerBase
    {
        private readonly BakeContext _db;
        public PostApiController(BakeContext db)
        {
            this._db = db;
        }
        public class PostCardDto
        {
            public int PostId { get; set; }
            public string Title { get; set; } = "";
            public string? Image { get; set; }
            public int? ViewCount { get; set; }
            public int? LikesCount { get; set; }
        }

        // ---------- 共用的「組卡片」查詢 ----------

        // 共用的「基底查詢」— 只做篩選，不做 Select
        private IQueryable<Post> BaseQuery()
        {
            return _db.Posts
                .Where(p => p.IsPublished == true);
        }
        // 共用的「投影成卡片 DTO」— 各 endpoint 排序完再呼叫
        private IQueryable<PostCardDto> ToCard(IQueryable<Post> query)
        {
            return query.Select(p => new PostCardDto
            {
                PostId = p.PostId,
                Title = p.Title,
                Image = p.PostAttachments
                    .Where(a => a.IsCover == true)
                    .Select(a => a.FileUrl)
                    .FirstOrDefault(),
                ViewCount = p.ViewCount,
                LikesCount = p.LikesCount
            });
        }

        // 1) 最新貼文（純文章，不含活動）
        [HttpGet("Latest")]
        public async Task<IActionResult> Latest()
        {
            var list = await ToCard(
                BaseQuery().OrderByDescending(p => p.CreatedAt)
                )
                .Take(10)
                .ToListAsync();
            return Ok(list);
        }

        // 2) 熱門貼文（按瀏覽數 or 按讚數排序）
        [HttpGet("Hot")]
        public async Task<IActionResult> Hot()
        {
            var list = await ToCard(
                        BaseQuery().OrderByDescending(p => p.ViewCount)
                    )
                    .Take(10)
                    .ToListAsync();
            return Ok(list);
        }
        // 3) 熱門活動分類（就是 EventTypeLookup）
        [HttpGet("EventTypes")]
        public async Task<IActionResult> EventTypes()
        {
            var list = await _db.EventTypeLookups
                .Select(t => new {
                    t.EventTypeId,
                    t.EventTypeName,
                    // 如果分類有圖片欄位就用，沒有就先寫死或留空
                })
                .ToListAsync();
            return Ok(list);
        }

        // 4) 揪團中的活動（Post + EventDetail，報名中的）
        [HttpGet("GroupEvents")]
        public async Task<IActionResult> GroupEvents()
        {
            var now = DateTime.Now;
            var list = await _db.Posts
                .Where(p => p.IsPublished == true
                && p.EventDetails.Any(e => e.SignupDeadline > now))
                .Select(p => new
                {
                    p.PostId,
                    p.Title,
                    Image = p.PostAttachments
                        .Where(a => a.IsCover == true)
                        .Select(a => a.FileUrl)
                        .FirstOrDefault(),
                    EventTime = p.EventDetails
                        .Select(e => e.EventTime)
                        .FirstOrDefault(),
                    SingupDeadline = p.EventDetails
                        .Select(e => e.SignupDeadline)
                        .FirstOrDefault(),
                })
                .OrderByDescending(p => p.EventTime)
                .Take(20)
                .ToListAsync();
            return Ok(list);
        }

        // 全部文章+活動（給 PostList 頁用）
        [HttpGet("All")]
        public async Task<IActionResult> All(string? keyword)
        {
            var query = BaseQuery();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(p =>
                p.Title.Contains(keyword) ||
                p.Content.Contains(keyword));
            }
            var list = await query
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new
                {
                    p.PostId,
                    p.Title,
                    Content = p.Content.Length > 100 ? p.Content.Substring(0, 100) + "..." : p.Content,
                    p.TypeId,
                    p.ViewCount,
                    p.CreatedAt,

                    Image = p.PostAttachments
                        .Where(a => a.IsCover == true)
                        .Select(a => a.FileUrl)
                        .FirstOrDefault(),
                    EventTypeId = (int?)p.EventDetails
                        .Select(e => (int?)e.EventTypeId)
                        .FirstOrDefault(),
                    EventTypeName = p.EventDetails
                        .Select(e => e.EventType.EventTypeName)
                        .FirstOrDefault(),
                    Price = p.EventDetails
                        .Select(e => e.Price)
                        .FirstOrDefault(),
                    EventTime = p.EventDetails
                        .Select(e => (DateTime?)e.EventTime)
                        .FirstOrDefault(),
                    LocationCity = p.EventDetails
                        .Select(e => e.LocationCity)
                        .FirstOrDefault(),
                }).ToListAsync();
            return Ok(list);
        }
    }
}
