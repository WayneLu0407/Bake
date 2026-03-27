using Bake.Data;
using Bake.Models.Sales;
using Bake.ViewModel;
using Bake.ViewModels.Social;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using NuGet.ContentModel;
using System.ComponentModel.Design;
using System.Net.Mail;
using System.Xml.Linq;

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

            var comments = await _db.PostComments
                    .Where(c => c.PostId == id)
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => new PostDetailViewModel.PostComment
                    {
                        CommentId = c.CommentId,
                        UserId = c.UserId,
                        UserName = c.User.FullName ?? "匿名",    // 透過導覽屬性拿名字
                        AvatarUrl = c.User.AvatarUrl,             // 透過導覽屬性拿頭像
                        Content = c.Content,
                        CreatedAt = c.CreatedAt,
                        ParentCommentId = c.ParentCommentId,
                    })
                    .ToListAsync();

            foreach (var c in comments)
            {
                if (string.IsNullOrEmpty(c.AvatarUrl))
                {
                    c.AvatarUrl = "/ProductPicture/NoImage.jpg";
                }
                else if (!c.AvatarUrl.StartsWith("/"))
                {
                    c.AvatarUrl = "/" + c.AvatarUrl;
                }
            }

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

                

                Comments = comments,
                IsFavorited = false,
                IsFollowed = false,
                IsLiked = false,
            };

            if (string.IsNullOrEmpty(vm.Author.Avatar))
            {
                vm.Author.Avatar = "/ProductPicture/NoImage.jpg";
            }
            else if (!vm.Author.Avatar.StartsWith("/"))
            {
                vm.Author.Avatar = "/" + vm.Author.Avatar;
            }

            return View(vm);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int PostId, string Content)
        {

            var userId = int.Parse(User.FindFirst("UserId")?.Value??"0");
            
            if (string.IsNullOrWhiteSpace(Content))
            {
                return RedirectToAction("PostDetail", new { id = PostId });
            }
            
            var comment = new Models.Social.PostComment
            {
                PostId = PostId,
                UserId = userId,
                Content = Content.Trim(),
            };

            _db.PostComments.Add(comment);
            await _db.SaveChangesAsync();

            // 留言完導回同一篇文章
            return RedirectToAction("PostDetail", new { id = PostId });
        }
    }
}
