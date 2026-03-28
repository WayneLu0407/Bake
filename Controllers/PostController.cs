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
using Bake.Models.Social;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Bake.Controllers
{
    public class PostController : Controller
    {

        private readonly BakeContext _db;

        private const string EventApplySessionKey = "EventApplyDraft";

        private int CurrentUserId =>
            int.TryParse(User.FindFirst("UserId")?.Value, out var userId) ? userId : 0;


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


        //public IActionResult Latest()
        //{
        //    return View();
        //}

        //public IActionResult Trending()
        //{
        //    return View();
        //}

        public IActionResult Events()
        {
            return View();
        }

        [Authorize]
        [HttpGet("/apply/{eventId}")]
        public async Task<IActionResult> Apply(int eventId)
        {
            var accessError = await ValidateEventAccessAsync(eventId, CurrentUserId);
            if (accessError != null)
            {
                TempData["ErrorMessage"] = accessError;
                return RedirectToAction(nameof(Events));
            }

            var vm = await BuildApplyViewModelAsync(eventId);
            if (vm == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost("/apply/{eventId}")]
        public async Task<IActionResult> Apply(int eventId, EventApplyViewModel input)
        {
            var vm = await BuildApplyViewModelAsync(eventId);
            if (vm == null)
            {
                return NotFound();
            }

            vm.ApplicantName = input.ApplicantName;
            vm.ApplicantPhone = input.ApplicantPhone;
            vm.ApplicantEmail = input.ApplicantEmail;
            vm.NumParticipants = input.NumParticipants;
            vm.GenderText = input.GenderText;

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var accessError = await ValidateEventAccessAsync(eventId, CurrentUserId);
            if (accessError != null)
            {
                ModelState.AddModelError(string.Empty, accessError);
                return View(vm);
            }

            var capacityError = await ValidateCapacityAsync(eventId, vm.NumParticipants);
            if (capacityError != null)
            {
                ModelState.AddModelError(string.Empty, capacityError);
                return View(vm);
            }

            SaveApplyDraft(vm);
            //重新導向
            return RedirectToAction(nameof(Confirmed));
        }

        [Authorize]
        [HttpGet("/confirmed")]
        public IActionResult Confirmed()
        {
            var vm = GetApplyDraft();
            if (vm == null)
            {
                TempData["ErrorMessage"] = "請先完成報名資料填寫";
                return RedirectToAction(nameof(Events));
            }
            return View(vm);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [ActionName("Confirmed")]
        [HttpPost("/confirmed")]
        public async Task<IActionResult> ConfirmedSubmit()
        {
            var vm = GetApplyDraft();
            if(vm == null)
            {
            TempData["ErrorMessage"] = "報名資料已逾期，請重新填寫";
            return RedirectToAction(nameof(Events));
            }

            var accessError = await ValidateEventAccessAsync(vm.EventId, CurrentUserId);
            if (accessError != null)
            {
                TempData["ErrorMessage"] = accessError;
                return RedirectToAction(nameof(Apply), new { eventId = vm.EventId });
            }



            var capacityError = await ValidateCapacityAsync(vm.EventId, vm.NumParticipants);
            if (capacityError != null)
            {
                TempData["ErrorMessage"] = capacityError;
                return RedirectToAction(nameof(Apply), new { eventId = vm.EventId });
            }



            var defaultRegistStatusId = await GetDefaultRegistStatusIdAsync();
            if (!defaultRegistStatusId.HasValue)
            {
                return StatusCode(500, "找不到預設報名狀態，請先確認 Regist_Status_Lookup 是否有資料");
            }


            var registration = new EventRegistration
            {
                EventId = vm.EventId,
                UserId = CurrentUserId,
                NumParticipants = vm.NumParticipants,
                RegistStatusId = defaultRegistStatusId.Value,
                CreatedAt = DateTime.Now
            };

            _db.EventRegistrations.Add(registration);
            await _db.SaveChangesAsync();

            ClearApplyDraft();

            return RedirectToAction(nameof(Success), new { eventId = vm.EventId });
        }



        [Authorize]
        [HttpGet("/events/{eventId:int}/success")]
        public IActionResult Success(int eventId)
        {
            ViewBag.EventId = eventId;

            // 等活動詳情頁完成再導回/posts/events/{eventId}
            ViewBag.ReturnUrl = "/posts/events";

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

        private async Task<EventApplyViewModel?> BuildApplyViewModelAsync(int eventId)
        {
            var eventDetail = await _db.EventDetails
                .Include(e => e.Post)
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (eventDetail?.Post == null)
            {
                return null;
            }

            var account = await _db.AccountAuths
                .Include(a => a.UserProfile)
                .ThenInclude(p => p.UserGenderNavigation)
                .FirstOrDefaultAsync(a => a.UserId == CurrentUserId);

            if (account?.UserProfile == null)
            {
                return null;
            }

            return new EventApplyViewModel
            {
                EventId = eventDetail.EventId,
                EventTitle = eventDetail.Post.Title,
                EventTime = eventDetail.EventTime,
                LocationCity = eventDetail.LocationCity ?? string.Empty,
                LocationAddress = eventDetail.LocationAddress ?? string.Empty,

                ApplicantName = account.UserProfile.FullName,
                ApplicantPhone = account.UserProfile.UserPhone,
                ApplicantEmail = account.Email,
                GenderText = account.UserProfile.UserGenderNavigation?.StatusName ?? "未提供",

                NumParticipants = 1
            };
        }

        private async Task<string?> ValidateEventAccessAsync(int eventId, int userId)
        {
            var eventDetail = await _db.EventDetails
                .Include(e => e.Post)
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (eventDetail?.Post == null)
            {
                return "找不到活動資料";
            }

            if (eventDetail.Post.AuthorId == userId)
            {
                return "發起人不能報名自己的活動";
            }

            if (DateTime.Now < eventDetail.SignupStart)
            {
                return "活動尚未開放報名";
            }

            if (DateTime.Now > eventDetail.SignupDeadline)
            {
                return "活動報名已截止";
            }

            var hasRegistered = await _db.EventRegistrations
                .AnyAsync(x => x.EventId == eventId && x.UserId == userId);

            if (hasRegistered)
            {
                return "你已經報名過這個活動";
            }

            return null;
        }

        private async Task<string?> ValidateCapacityAsync(int eventId, int numParticipants)
        {
            var eventDetail = await _db.EventDetails
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (eventDetail == null)
            {
                return "找不到活動資料";
            }

            var currentTotalParticipants = await _db.EventRegistrations
                .Where(x => x.EventId == eventId)
                .SumAsync(x => (int?)x.NumParticipants) ?? 0;

            if (currentTotalParticipants + numParticipants > eventDetail.MaxParticipants)
            {
                return "剩餘名額不足";
            }

            return null;
        }

        private async Task<byte?> GetDefaultRegistStatusIdAsync()
        {
            return await _db.RegistStatusLookups
                .OrderBy(x => x.RegStatusId)
                .Select(x => (byte?)x.RegStatusId)
                .FirstOrDefaultAsync();
        }

        private void SaveApplyDraft(EventApplyViewModel vm)
        {
            HttpContext.Session.SetString(
                EventApplySessionKey,
                JsonSerializer.Serialize(vm)
                );
        }

        private EventApplyViewModel? GetApplyDraft()
        {
            var json = HttpContext.Session.GetString(EventApplySessionKey);

            return string.IsNullOrWhiteSpace(json)
                ? null
                : JsonSerializer.Deserialize<EventApplyViewModel>(json);
        }

        private void ClearApplyDraft()
        {
            HttpContext.Session.Remove(EventApplySessionKey);
        }
    }
}