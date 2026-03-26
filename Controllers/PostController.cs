using Bake.Data;
using Bake.ViewModels.Social;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

namespace Bake.Controllers
{
    public class PostController : Controller
    {

        private readonly BakeContext _db;

        public PostController(BakeContext db)
        {
            this._db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PostList()
        {
            return View();
        }

        public IActionResult Trending()
        {
            return View();
        }
        public IActionResult Events()
        {
            return View();
        }

        public IActionResult Apply()
        {
            return View();
        }
        public IActionResult Confirmed()
        {
            return View();
        }
        public IActionResult UserProfile()
        {
            return View();
        }

        //GET:Post/PostDetail/1
        public async Task<IActionResult> PostDetail(int id)
        {
            var post = await _db.Posts
                .Include(p => p.Author)
                    .ThenInclude(a => a.FollowBefolloweds)
                .Include(p => p.Tags)
                .Include(p => p.PostAttachments)
                .Include(p => p.EventDetails)
                    .ThenInclude(e => e.EventType)
                .Include(p => p.EventDetails)
                    .ThenInclude(e => e.ManualStatus)
                .Include(p => p.EventDetails)
                    .ThenInclude(e => e.EventRegistrations)
                .FirstOrDefaultAsync(p => p.PostId == id);

            if(post==null) return NotFound();

            var eventDetail = post.EventDetails.FirstOrDefault();
            
            
            var vm = new PostDetailViewModel
            {
                PostId = post.PostId,
                TypeId = post.TypeId,
                Title = post.Title,
                Content = post.Content,
                ViewCount = post.ViewCount ?? 0,
                LikesCount = post.LikesCount ?? 0,
                FavoriteCount = post.FavoriteCount ?? 0,

                Attachments = post.PostAttachments
                    .OrderBy(pa => pa.SortOrder)
                    .Select(pa => new PostDetailViewModel.AttachmentDto
                    {
                        ImageId = pa.ImageId,
                        FileUrl = pa.FileUrl,
                        AltText = pa.AltText,
                        IsCover = pa.IsCover ?? false,
                        SortOrder = pa.SortOrder ?? 0,
                    }).ToList(),

                Tags = post.Tags.Select(t => new PostDetailViewModel.TagDto
                {
                    TagId = t.TagId,
                    TagName = t.TagName
                }).ToList(),

                Author = new PostDetailViewModel.AuthorDto
                {
                    UserId = post.Author.UserId,
                    DisplayName = post.Author.FullName,
                    Avatar = post.Author.AvatarUrl,
                    Bio = post.Author.Bio,
                    ShareCount = await _db.Posts.CountAsync(p => p.AuthorId == post.AuthorId),
                    FollowerCount = post.Author.FollowBefolloweds.Count,

                },
                EventDetail = eventDetail == null ? null : new PostDetailViewModel.EventDetailDto
                {
                    EventId = eventDetail.EventId,
                    EventTypeName = eventDetail.EventType.EventTypeName,
                    StatusName = eventDetail.ManualStatus.StatusName,
                    Price = eventDetail.Price,
                    MaxParticipants = eventDetail.MaxParticipants,
                    CurrentParticipants = eventDetail.EventRegistrations.Count,
                    SignupStart = eventDetail.SignupStart,
                    SignupDeadline = eventDetail.SignupDeadline,
                    EventTime = eventDetail.EventTime,
                    EventEndTime = eventDetail.EventEndTime,
                    LocationCity = eventDetail.LocationCity,
                    LocationAddress = eventDetail.LocationAddress
                },
                IsFavorited = false,
                IsFollowed = false,
                IsLiked = false,
            };
            return View(vm);
        }
    }
}
