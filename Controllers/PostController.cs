using Bake.Data;
using Bake.Models.Sales;
using Bake.Models.Social;
using Bake.ViewModel;
using Bake.ViewModels.Social;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using NuGet.ContentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using System.Text.Json;
using System.Xml.Linq;

namespace Bake.Controllers
{
    public class PostController : Controller
    {
        private readonly BakeContext _db;
        private readonly IWebHostEnvironment _env;

        private const string EventApplySessionKey = "EventApplyDraft";
        private const byte ConfirmedRegistStatusId = 1;
        private const byte CancelledRegistStatusId = 3;

        private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png"
        };


        public PostController(BakeContext context, IWebHostEnvironment env)
        {
            _db = context;
            _env = env;
        }

        private int CurrentUserId =>
            int.TryParse(User.FindFirst("UserId")?.Value, out var userId) ? userId : 0;

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult PostList()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostToggleLike(int postId)   // 貼文按讚
        {
            var user = User.FindFirst("UserId")?.Value;
            if (user == null) // 判斷有沒有登入
            {
                return Json(new { success = false, message = "請先登入!" });
            }
            var postExists = await _db.Posts.AnyAsync(p => p.PostId == postId);
            if (!postExists) // 判斷貼文存在
            {
                return Json(new { success = false, message = "找不到該貼文!" });
            }
            try
            {
                var like = await _db.PostLikes.FirstOrDefaultAsync(l =>    // 判斷有沒有按讚
                l.UserId == int.Parse(user) && l.PostId == postId);
                if (like == null)       // 如果還沒按讚   新增至按讚表
                {
                    var newLike = new PostLike
                    {
                        UserId = int.Parse(user),
                        PostId = postId,
                        CreatedAt = DateTime.Now,
                    };
                    _db.PostLikes.Add(newLike);
                    await _db.SaveChangesAsync();
                    return Json(new { success = true, isLiked = true, message = "已按讚!" });
                }
                else                    //如果已經按讚  移除按讚清單
                {
                    _db.PostLikes.Remove(like);
                    await _db.SaveChangesAsync();
                    return Json(new { success = true, isLiked = false, message = "已取消按讚!" });
                }
            }
            catch(Exception ex)
            {
                return Json(new {success = false, message = "伺服器發生錯誤"});
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostToggleFavorite(int postId)   // 貼文收藏
        {
            var user = User.FindFirst("UserId")?.Value;
            if (user == null)// 判斷有沒有登入
            {
                return Json(new { success = false, message = "請先登入!" });
            }
            var postExists = await _db.Posts.AnyAsync(p => p.PostId == postId);
            if (!postExists) // 判斷貼文存在
            {
                return Json(new { success = false, message = "找不到該貼文!" });
            }
            try
            {
                var favorite = await _db.PostFavorites.FirstOrDefaultAsync(f =>    // 判斷有沒有收藏
                f.UserId == int.Parse(user) && f.PostId == postId);
                if (favorite == null)       // 如果還沒收藏   新增至收藏表
                {
                    var newFavorite = new PostFavorite
                    {
                        UserId = int.Parse(user),
                        PostId = postId,
                        CreatedAt = DateTime.Now,
                    };
                    _db.PostFavorites.Add(newFavorite);
                    await _db.SaveChangesAsync();
                    return Json(new { success = true, isFavorited = true, message = "已收藏貼文!" });
                }
                else                    //如果已經收藏  移除收藏清單
                {
                    _db.PostFavorites.Remove(favorite);
                    await _db.SaveChangesAsync();
                    return Json(new { success = true, isFavorited = false, message = "已移除收藏!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "伺服器發生錯誤" });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserToggleFollow(int beFollowId)   // 用戶追蹤
        {
            var user = User.FindFirst("UserId")?.Value;
            if (user == null)  // 判斷有沒有登入
            {
                return Json(new { success = false, message = "請先登入!" });
            }
            if (int.Parse(user) == beFollowId)
            {
                return Json(new { success = false, message = "你不能追蹤你自己!" });
            }
            var userExists = await _db.UserProfiles.AnyAsync(p => p.UserId == beFollowId);
            if (!userExists)  // 判斷用戶存在
            {
                return Json(new { success = false, message = "找不到該用戶!" });
            }
            try
            {
                var follow = await _db.Follows.FirstOrDefaultAsync(f =>    // 判斷有沒有追蹤
                f.FollowerId == int.Parse(user) && f.BefollowedId == beFollowId);
                if (follow == null)       // 如果還沒追蹤   新增至追蹤表
                {
                    var newFollow = new Follow
                    {
                        FollowerId = int.Parse(user),
                        BefollowedId = beFollowId,
                        CreatedAt = DateTime.Now,
                    };
                    _db.Follows.Add(newFollow);
                    await _db.SaveChangesAsync();
                    return Json(new { success = true, isFollowed = true, message = "已追蹤用戶!" });
                }
                else                    //如果已經追蹤  移除追蹤清單
                {
                    _db.Follows.Remove(follow);
                    await _db.SaveChangesAsync();
                    return Json(new { success = true, isFollowed = false, message = "已取消追蹤!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "伺服器發生錯誤" });
            }
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


        //建立新活動 - 頭
        [Authorize]
        [HttpGet("/posts/events/new")]
        public async Task<IActionResult> NewEvent()
        {
            // 抓出發起者的資料
            var vm = new EventCreateViewModel();
            await FillOrganizerInfoAsync(vm, CurrentUserId);
            await LoadEventTypeOptionsAsync(vm);
            return View(vm);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost("/posts/events/new")]
        public async Task<IActionResult> NewEvent(EventCreateViewModel input)
        {

            // 抓出發起者的資料
            await FillOrganizerInfoAsync(input, CurrentUserId);
            await LoadEventTypeOptionsAsync(input);
            ValidateEventCreateInput(input);

            if (!ModelState.IsValid)
            {
                return View(input);
            }

            const byte postTypeId = 1;
            byte eventTypeId = input.EventTypeId;
            byte eventStatusId = 1;

            var eventStart = input.EventDate.Date.Add(input.StartTime);
            var eventEnd = input.EventDate.Date.Add(input.EndTime);
            var signupStart = input.SignupStartDate.Date;
            var signupDeadline = input.SignupEndDate.Date.AddDays(1).AddTicks(-1);

            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var post = new Post
                {
                    AuthorId = CurrentUserId,
                    TypeId = postTypeId,
                    Title = input.Title.Trim(),
                    Content = input.Content.Trim(),
                    CreatedAt = DateTime.Now,
                    IsPublished = true
                };

                _db.Posts.Add(post);
                await _db.SaveChangesAsync();

                var eventDetail = new EventDetail
                {
                    PostId = post.PostId,
                    EventTypeId = eventTypeId,
                    ManualStatusId = eventStatusId,
                    Price = input.Price,
                    MaxParticipants = input.MaxParticipants,
                    SignupStart = signupStart,
                    SignupDeadline = signupDeadline,
                    EventTime = eventStart,
                    EventEndTime = eventEnd,
                    LocationCity = input.LocationCity.Trim(),
                    LocationAddress = input.LocationAddress.Trim()
                };

                _db.EventDetails.Add(eventDetail);

                var attachment = await SaveEventPhotoAsync(input.Photo, post.PostId, input.Title.Trim());
                if (attachment != null)
                {
                    _db.PostAttachments.Add(attachment);
                }

                await AttachTagsToPostAsync(post, input.KeywordsText);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "活動建立成功";
                return RedirectToAction("PostDetail", new { id = post.PostId });
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        // ------建立活動(尾)

        [Authorize]
        [HttpGet("/Post/events/{postId:int}/edit")]
        public async Task<IActionResult> EditEvent(int postId)
        {
            var post = await GetOwnedEventPostAsync(postId, CurrentUserId);

            if (post == null)
            {
                return NotFound();
            }
            var vm = await BuildEventEditViewModelAsync(post);
            return View(vm);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost("/Post/events/{postId:int}/edit")]
        public async Task<IActionResult> EditEvent(int postId , EventEditViewModel input)
        {
            var post = await GetOwnedEventPostAsync(postId, CurrentUserId);

            if(post == null)
            {
                return NotFound();
            }

            var eventDetail = post.EventDetails.First();

            input.PostId = post.PostId;
            input.EventId = eventDetail.EventId;
            input.HasRegistrations = eventDetail.EventRegistrations
                .Any(x => x.RegistStatusId == ConfirmedRegistStatusId);
            input.ExistingPhotoUrl = post.PostAttachments
                .Where(a=>a.IsCover == true)
                .Select(a=>a.FileUrl)
                .FirstOrDefault();
            
            await FillOrganizerInfoAsync(input, CurrentUserId);
            await LoadEventTypeOptionsAsync(input);

            PreserveLockFieldsForRegisteredEvent(input, post);

            ValidateEventCreateInput(input);

            if (!ModelState.IsValid)
            {
                return View(input);
            }

            post.Title = input.Title.Trim();
            post.Content = input.Content.Trim();
            eventDetail.EventTypeId = input.EventTypeId;

            //價格應該也要鎖起來才對?
            eventDetail.Price = input.Price;

            //報名人數=false時,才可編輯
            if (!input.HasRegistrations)
            {
                eventDetail.EventTime = input.EventDate.Date.Add(input.StartTime);
                eventDetail.EventEndTime = input.EventDate.Date.Add(input.EndTime);

                eventDetail.LocationCity = input.LocationCity.Trim();
                eventDetail.LocationAddress = input.LocationAddress.Trim();

                eventDetail.MaxParticipants = input.MaxParticipants;
                eventDetail.SignupStart = input.SignupStartDate.Date;
                eventDetail.SignupDeadline = input.SignupEndDate.Date.AddDays(1).AddTicks(-1);
            }

            await ReplaceTagsForPostAsync(post, input.KeywordsText);
            await ReplaceEventCoverPhotoAsync(post, input.Photo, post.Title);

            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "活動更新成功";
            return RedirectToAction("PostDetail", new { id = post.PostId });
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost("/Post/events/{postId:int}/unpublish")]
        public async Task<IActionResult> UnpublishEvent(int postId)
        {
            var post = await GetOwnedEventPostAsync(postId, CurrentUserId);

            if(post== null)
            {
                return NotFound();
            }

            //已下架的不重複處理
            if (post.IsPublished != true)
            {
                TempData["SuccessMessage"] = "活動已經是下架狀態";
                return RedirectToAction(nameof(PostDetail), new { id = post.PostId });
            }

            post.IsPublished = false;
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "活動已下架";
            return RedirectToAction(nameof(PostDetail), new { id = post.PostId });
        }

        // ------ 申請參加活動(頭)
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

            var registration = new EventRegistration
            {
                EventId = vm.EventId,
                UserId = CurrentUserId,
                NumParticipants = vm.NumParticipants,
                RegistStatusId = ConfirmedRegistStatusId,
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

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost("/events/{eventId:int}/cancel")]
        public async Task<IActionResult> CancelRegistration(int eventId)
        {
            var error = await ValidateCancelRegistrationAsync(eventId, CurrentUserId);
            if (error != null)
            {
                TempData["ErrorMessage"] = error;
                return RedirectToAction(nameof(PostDetailByEventId), new { eventId });
            }

            var registration = await GetMyActiveRegistrationAsync(eventId, CurrentUserId);
            if (registration == null)
            {
                TempData["ErrorMessage"] = "找不到可取消的報名紀錄";
                return RedirectToAction(nameof(Events));
            }

            registration.RegistStatusId = CancelledRegistStatusId;
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "已取消報名";
            return RedirectToAction(nameof(PostDetailByEventId), new { eventId });
        }

        //讓Cancel成功能導回活動詳細頁面
        [HttpGet("/events/{eventId:int}/detail")]
        public async Task<IActionResult> PostDetailByEventId(int eventId)
        {
            var postId = await _db.EventDetails
                .Where(e => e.EventId == eventId)
                .Select(e => (int?)e.PostId)
                .FirstOrDefaultAsync();

            if (!postId.HasValue)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(PostDetail), new { id = postId.Value });
        }


        // ------ 申請參加活動(尾)

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

            if (post==null) return NotFound();

            // 判斷文章擁有者(編輯活動貼文用)
            var isOwner = post.AuthorId == this.CurrentUserId && this.CurrentUserId > 0;
            var isPublished = post.IsPublished == true;
            if(!isPublished && !isOwner)
            {
                return NotFound();
            }

            var eventDetail = post.EventDetails.FirstOrDefault();

            var user = User.FindFirst("UserId")?.Value;
            
            var isFollowed = false;
            var isLike = false;
            var isFavorited = false;

            if(int.TryParse(user,out int CurrentUserId))
            {
                isFollowed = await _db.Follows.AnyAsync(f =>
                    f.FollowerId == CurrentUserId && f.BefollowedId == post.AuthorId );
                isLike = await _db.PostLikes.AnyAsync(f =>
                    f.UserId == CurrentUserId && f.PostId == id);
                isFavorited = await _db.PostFavorites.AnyAsync(f =>
                    f.UserId == CurrentUserId && f.PostId == id);
            }

            

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

            // 判斷報名或取消報名活動用
            var isCurrentUserRegistered = false;
            var canCurrentUserCancelRegistration = false;

            if (eventDetail != null && CurrentUserId > 0)
            {
                isCurrentUserRegistered = eventDetail.EventRegistrations
                    .Any(x => x.UserId == CurrentUserId && x.RegistStatusId == ConfirmedRegistStatusId);

                canCurrentUserCancelRegistration = isCurrentUserRegistered;
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
                // 判斷文章擁有者(編輯活動貼文用)
                IsOwner = isOwner,
                IsPublished = isPublished,
                // 判斷報名或取消報名活動用
                IsCurrentUserRegistered = isCurrentUserRegistered,
                CanCurrentUserCancelRegistration = canCurrentUserCancelRegistration,

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
                    LocationAddress = eventDetail.LocationAddress,
                },



                Comments = comments,
                IsFavorited = isFavorited,
                IsFollowed = isFollowed,
                IsLiked = isLike,
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

        // ------ 申請參加活動方法(頭)
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

            if(eventDetail.Post.IsPublished != true)
            {
                return "活動已下架";
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

            var hasRegistered = await ActiveRegistrationsQuery()
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

            var currentTotalParticipants = await ActiveRegistrationsQuery()
                .Where(x => x.EventId == eventId)
                .SumAsync(x => (int?)x.NumParticipants) ?? 0;

            if (currentTotalParticipants + numParticipants > eventDetail.MaxParticipants)
            {
                return "剩餘名額不足";
            }

            return null;
        }
        private IQueryable<EventRegistration> ActiveRegistrationsQuery()  //尚未執行的查詢，比IEnumable省資源
        {
            return _db.EventRegistrations.Where(x => x.RegistStatusId == ConfirmedRegistStatusId);
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

        //找出我的報名
        private async Task<EventRegistration?> GetMyActiveRegistrationAsync(int eventId, int userId)
        {
            return await _db.EventRegistrations
                .FirstOrDefaultAsync(x =>
                    x.EventId == eventId &&
                    x.UserId == userId &&
                    x.RegistStatusId == ConfirmedRegistStatusId);
        }

        //取消報名用
        private async Task<string?> ValidateCancelRegistrationAsync(int eventId, int userId)
        {
            var eventDetail = await _db.EventDetails
                .Include(e => e.Post)
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (eventDetail?.Post == null)
            {
                return "找不到活動資料";
            }

            var registration = await GetMyActiveRegistrationAsync(eventId, userId);
            if (registration == null)
            {
                return "你目前沒有可取消的有效報名紀錄";
            }

            return null;
        }

        // ------ 申請參加活動方法(尾)


        // ------ 編輯發起的活動方法(頭)

        private async Task<Post?> GetOwnedEventPostAsync(int postId, int userId)
        {
            return await _db.Posts
                .Include(p => p.EventDetails)
                    .ThenInclude(e => e.EventRegistrations)
                .Include(p => p.PostAttachments)
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(p =>
                p.PostId == postId &&
                p.TypeId == 1 && //限定活動
                p.AuthorId == userId); //限定主辦人
        }

        private async Task<EventEditViewModel> BuildEventEditViewModelAsync(Post post)
        {
            var eventDetail = post.EventDetails.First();

            var vm = new EventEditViewModel
            {
                PostId = post.PostId,
                EventId = eventDetail.EventId,

                Title = post.Title,
                Content = post.Content,

                EventDate = eventDetail.EventTime.Date,
                StartTime = eventDetail.EventTime.TimeOfDay,
                EndTime = eventDetail.EventEndTime.TimeOfDay,

                LocationCity = eventDetail.LocationCity ?? string.Empty,
                LocationAddress = eventDetail.LocationAddress ?? string.Empty,

                MaxParticipants = eventDetail.MaxParticipants,
                Price = eventDetail.Price ?? 0,

                SignupStartDate = eventDetail.SignupStart.Date,
                SignupEndDate = eventDetail.SignupDeadline.Date,

                EventTypeId = eventDetail.EventTypeId,

                KeywordsText = string.Join(",", post.Tags.Select(t => t.TagName)),
                ExistingPhotoUrl = post.PostAttachments   //這邊是多圖片的寫法，可能要改掉
                    .Where(a => a.IsCover == true)
                    .Select(a => a.FileUrl)
                    .FirstOrDefault(),

                HasRegistrations = eventDetail.EventRegistrations.Any(x => x.RegistStatusId == ConfirmedRegistStatusId) //回傳布林
            };

            await FillOrganizerInfoAsync(vm, CurrentUserId);
            await LoadEventTypeOptionsAsync(vm);

            return vm;
        }

        //後端鎖定編輯欄位
        private static void PreserveLockFieldsForRegisteredEvent (EventEditViewModel input,Post post)
        {
            var eventDetail = post.EventDetails.First();

            if (!eventDetail.EventRegistrations.Any(x => x.RegistStatusId == ConfirmedRegistStatusId))
            {
                return;
            }

            input.EventDate = eventDetail.EventTime.Date;
            input.StartTime = eventDetail.EventTime.TimeOfDay;
            input.EndTime = eventDetail.EventEndTime.TimeOfDay;

            input.LocationCity = eventDetail.LocationCity ?? string.Empty;
            input.LocationAddress = eventDetail.LocationAddress ?? string.Empty;

            input.MaxParticipants = eventDetail.MaxParticipants;

            input.SignupStartDate = eventDetail.SignupStart.Date;
            input.SignupEndDate = eventDetail.SignupDeadline.Date;
        }

        //先把舊的tag殺掉再重新存入
        private async Task ReplaceTagsForPostAsync(Post post, string? keywordsText)
        {
            post.Tags.Clear();
            await AttachTagsToPostAsync(post, keywordsText);
        }

        //刪舊圖片用
        private void DeleteLocalFileIfExists(string? fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
            {
                return;
            }
            //避免其他環境讀取時路徑規則不一樣
            var relativePath = fileUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
            var absolutePath = Path.Combine(_env.WebRootPath, relativePath);

            //Exists是否存在
            if (System.IO.File.Exists(absolutePath))
            {
                System.IO.File.Delete(absolutePath);
            }
        }
        //更換封面圖
        private async Task ReplaceEventCoverPhotoAsync(Post post, IFormFile? newPhoto, string title)
        {
            if (newPhoto == null || newPhoto.Length == 0)
            {
                return;
            }

            var newAttachment = await SaveEventPhotoAsync(newPhoto, post.PostId, title);
            if (newAttachment == null)
            {
                return;
            }

            var oldCover = post.PostAttachments
                .OrderByDescending(a => a.IsCover == true)
                .ThenBy(a => a.SortOrder ?? int.MaxValue)
                .FirstOrDefault();

            if (oldCover != null)
            {
                DeleteLocalFileIfExists(oldCover.FileUrl);
                _db.PostAttachments.Remove(oldCover);
            }

            _db.PostAttachments.Add(newAttachment);
        }


        // ------ 編輯發起的活動方法(尾)

        // ------ 管理活動狀態用(頭)
        private static (string StatusText, string BadgeClass) GetHostedEventDisplayStatus(bool isPublished, EventDetail eventDetail, DateTime now)
        {
            if (!isPublished)
            {
                return ("已下架", "secondary");
            }

            if (now > eventDetail.EventEndTime)
            {
                return ("已結束", "dark");
            }

            if (now >= eventDetail.EventTime && now <= eventDetail.EventEndTime)
            {
                return ("活動進行中", "primary");
            }

            if (now > eventDetail.SignupDeadline)
            {
                return ("報名已截止", "warning");
            }

            if (now < eventDetail.SignupStart)
            {
                return ("報名尚未開始", "info");
            }

            return ("報名中", "success");
        }

        private async Task<MyHostedEventsViewModel> BuildHostedEventsSectionAsync(int userId)
        {
            var posts = await _db.Posts
                .AsNoTracking()
                .Where(p => p.AuthorId == userId
                            && p.TypeId == 1
                            && p.EventDetails.Any())
                .Include(p => p.EventDetails)
                    .ThenInclude(e => e.EventType)
                .Include(p => p.EventDetails)
                    .ThenInclude(e => e.EventRegistrations)
                .Include(p => p.PostAttachments)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var now = DateTime.Now;

            var items = posts.Select(post =>
            {
                var eventDetail = post.EventDetails.First();

                var (statusText, badgeClass) = GetHostedEventDisplayStatus(
                    post.IsPublished == true,
                    eventDetail,
                    now);

                return new MyHostedEventItemViewModel
                {
                    PostId = post.PostId,
                    EventId = eventDetail.EventId,
                    Title = post.Title,
                    CoverImageUrl = post.PostAttachments
                        .OrderByDescending(a => a.IsCover == true)
                        .ThenBy(a => a.SortOrder ?? int.MaxValue)
                        .Select(a => a.FileUrl)
                        .FirstOrDefault(),

                    EventTypeName = eventDetail.EventType?.EventTypeName ?? "未分類",

                    EventTime = eventDetail.EventTime,
                    EventEndTime = eventDetail.EventEndTime,
                    SignupStart = eventDetail.SignupStart,
                    SignupDeadline = eventDetail.SignupDeadline,

                    LocationText = string.Join(" ", new[]
                    {
                eventDetail.LocationCity,
                eventDetail.LocationAddress
            }.Where(x => !string.IsNullOrWhiteSpace(x))),

                    ParticipantCount = eventDetail.EventRegistrations   //改成加總報名人數
                        .Where(x => x.RegistStatusId == ConfirmedRegistStatusId)
                        .Sum(x => x.NumParticipants),
                    MaxParticipants = eventDetail.MaxParticipants,

                    IsPublished = post.IsPublished == true,

                    StatusText = statusText,
                    BadgeClass = badgeClass
                };
            }).ToList();

            return new MyHostedEventsViewModel
            {
                TotalCount = items.Count,
                PublishedCount = items.Count(x => x.IsPublished),
                UnpublishedCount = items.Count(x => !x.IsPublished),
                Items = items
            };
        }


        // ------ 管理活動狀態用(尾)

        // ------ 建立活動用方法(頭)

        //編輯也有用到
        private async Task FillOrganizerInfoAsync(EventCreateViewModel vm, int userId)
        {
            var account = await _db.AccountAuths
                .Include(a => a.UserProfile)
                .FirstOrDefaultAsync(a => a.UserId == userId);

            vm.OrganizerName = account?.UserProfile?.FullName ?? "未命名主辦人";

            //幫頭像URL注意"/"，沒有就加入
            var avatarUrl = account?.UserProfile?.AvatarUrl;

            if (string.IsNullOrWhiteSpace(avatarUrl))
            {
                vm.OrganizerAvatarUrl = null;
            }
            else
            {
                vm.OrganizerAvatarUrl = avatarUrl.StartsWith("/")
                    ? avatarUrl
                    : "/" + avatarUrl;
            }

        }

        private void ValidateEventCreateInput(EventCreateViewModel input)
        {
            if (input.SignupStartDate > input.SignupEndDate)
            {
                ModelState.AddModelError(nameof(input.SignupEndDate), "報名截止日不可早於報名開始日");
            }

            if (input.StartTime >= input.EndTime)
            {
                ModelState.AddModelError(nameof(input.EndTime), "結束時間必須晚於開始時間");
            }

            if (input.SignupEndDate.Date > input.EventDate.Date)
            {
                ModelState.AddModelError(nameof(input.SignupEndDate), "報名截止日不可晚於活動日期");
            }

            if (input.Photo == null || input.Photo.Length == 0)
            {
                return;
            }

            var extension = Path.GetExtension(input.Photo.FileName).ToLower();

            if (!AllowedImageExtensions.Contains(extension))
            {
                ModelState.AddModelError(nameof(input.Photo), "照片只接受 jpg、jpeg、png");
            }

            if (input.Photo.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError(nameof(input.Photo), "照片不可超過 5MB");
            }
        }


        private async Task<PostAttachment?> SaveEventPhotoAsync(IFormFile? file, int postId, string title)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            //轉小寫+檢查副檔名
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!AllowedImageExtensions.Contains(extension))
            {
                return null;
            }

            // 實體資料夾：wwwroot/PostImage/activity
            var relativeFolder = Path.Combine("PostImage", "activity");
            var absoluteFolder = Path.Combine(_env.WebRootPath, relativeFolder);

            Directory.CreateDirectory(absoluteFolder);

            // 產生安全檔名：時間 + postId
            var timestamp = DateTime.Now.Ticks.ToString();
            var fileName = $"{timestamp}-{postId}{extension.ToLower()}";
            var savePath = Path.Combine(absoluteFolder, fileName);
            // 存入
            await using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 寫入DB路徑: /PostImage/activity/xxx.jpg
            var fileUrl = "/" + Path.Combine(relativeFolder, fileName).Replace("\\", "/");

            return new PostAttachment
            {
                PostId = postId,
                FileUrl = fileUrl,
                AltText = "EventPhoto",
                IsCover = true,
                SortOrder = 1
            };
        }

        private async Task AttachTagsToPostAsync(Post post, string? keywordsText)
        {
            var tagNames = SplitKeywords(keywordsText);
            if (tagNames.Count == 0)
            {
                return;
            }

            var existingTags = await _db.Tags
                .Where(t => tagNames.Contains(t.TagName))
                .ToListAsync();

            var existingNames = existingTags
                .Select(t => t.TagName)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var tag in existingTags)
            {
                post.Tags.Add(tag);
            }

            foreach (var name in tagNames.Where(n => !existingNames.Contains(n)))
            {
                post.Tags.Add(new Tag
                {
                    TagName = name
                });
            }
        }

        private static List<string> SplitKeywords(string? keywordsText)
        {
            if (string.IsNullOrWhiteSpace(keywordsText))
            {
                return new List<string>();
            }

            var delimiters = new[] { ',', '，', ' ', '　' }; // 包含全型空格

            return keywordsText
                .Split(delimiters, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(NormalizeTagName)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static string NormalizeTagName(string input)
        {
            var value = input.Trim();

            if (!value.StartsWith("#"))
            {
                value = "#" + value;
            }

            return value;
        }

        //處理EventType    //編輯也有用到
        private async Task LoadEventTypeOptionsAsync(EventCreateViewModel vm)
        {
            vm.EventTypeOptions = await _db.EventTypeLookups
                .OrderBy(x => x.EventTypeId)
                .Select(x => new SelectListItem
                {
                    Value = x.EventTypeId.ToString(),
                    Text = x.EventTypeName
                })
                .ToListAsync();
        }
        // ------ 建立活動用方法(尾)

        
    }
}