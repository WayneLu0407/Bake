using Bake.Data;
using Bake.Models.Social;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bake.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDetailApiController : ControllerBase
    {
        private readonly BakeContext _db;
        public UserDetailApiController(BakeContext db)
        {
            this._db = db;
        }
        public class PostCardDto
        {
            public int AuthorId { get; set; }
            public int PostId { get; set; }
            public string Title { get; set; } = "";
            public string? Image { get; set; }
            public int? ViewCount { get; set; }
            public int? LikesCount { get; set; }
        }

        // ---------- 共用的「組卡片」查詢 ----------

        // 共用的「基底查詢」— 只做篩選，不做 Select
        private IQueryable<Post> BaseQuery(int userId)
        {
            return _db.Posts
                .Where(p => p.IsPublished == true && p.AuthorId == userId);
        }
        // 共用的「投影成卡片 DTO」— 各 endpoint 排序完再呼叫
        private IQueryable<PostCardDto> ToCard(IQueryable<Post> query)
        {
            return query
                .Select(p => new PostCardDto
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

        // 1) 純文章，不含活動
        [HttpGet("Article/{userId}")]
        public async Task<IActionResult> Article(int userId)
        {
            var list = await ToCard(
                BaseQuery(userId).OrderByDescending(p => p.CreatedAt)
                )
                .Take(10)
                .ToListAsync();
            return Ok(list);
        }


        // 發起的活動（Post + EventDetail，報名中的）
        [HttpGet("OnGoingEvents/{userId}")]
        public async Task<IActionResult> OnGoingEvents(int userId)
        {
            var now = DateTime.Now;
            var list = await _db.Posts
                .Where(p => p.IsPublished == true
                && p.EventDetails.Any(e => e.SignupDeadline > now) && p.AuthorId == userId)
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
                .Take(10)
                .ToListAsync();
            return Ok(list);
        }

        // 發起過的活動（Post + EventDetail，報名結束的）
        [HttpGet("EndEvents/{userId}")]
        public async Task<IActionResult> EndEvents(int userId)
        {
            var now = DateTime.Now;
            var list = await _db.Posts
                .Where(p => p.IsPublished == true
                && p.EventDetails.Any(e => e.EventTime < now)
                && p.AuthorId== userId)
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
                })
                .OrderByDescending(p => p.EventTime)
                .Take(10)
                .ToListAsync();
            return Ok(list);
        }

        // 將參加的活動（Post + EventDetail，報名未結束的）
        [HttpGet("WillAttendEvents/{userId}")]
        public async Task<IActionResult> WillAttendEvents(int userId)
        {
            var now = DateTime.Now;
            var list = await _db.Posts
                .Where(p => p.IsPublished == true
                && p.AuthorId != userId
                && p.EventDetails.Any(e => e.EventTime > now
                    && e.EventRegistrations.Any(r => r.UserId == userId && r.RegistStatusId == 1)))
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
                })
                .OrderByDescending(p => p.EventTime)
                .Take(10)
                .ToListAsync();
            return Ok(list);
        }

        // 參加過的活動（Post + EventDetail，活動日期已結束的）
        [HttpGet("AttendedEvents/{userId}")]
        public async Task<IActionResult> AttendedEvents(int userId)
        {
            var now = DateTime.Now;
            var list = await _db.Posts
                .Where(p => p.IsPublished == true
                && p.AuthorId != userId
                && p.EventDetails.Any(e => e.EventTime < now
                    && e.EventRegistrations.Any(r => r.UserId == userId && r.RegistStatusId == 1)))
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
                })
                .OrderByDescending(p => p.EventTime)
                .Take(10)
                .ToListAsync();
            return Ok(list);
        }
        [HttpGet("Profile/{userId}")]
        public async Task<IActionResult> Profile(int userId)
        {
           
            var user = await _db.UserProfiles
                .Where(u => u.UserId== userId)
                .Select(u => new
                {
                    u.UserId,
                    u.FullName,
                    AvatarUrl = u.AvatarUrl != null
                        ? (u.AvatarUrl.StartsWith("/") ? u.AvatarUrl : "/" + u.AvatarUrl)
                        : "/seller_assets/images/profile/NoImage.png",
                    u.Bio,
                })
                .FirstOrDefaultAsync();
            if (user == null) return NotFound();
            return Ok(user);
        }
    }
}
