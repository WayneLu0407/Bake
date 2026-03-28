using Bake.Data;
using Bake.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bake.Models.Social;
using System.Text.Json;

namespace Bake.Controllers
{
    public class PostController : Controller
    {
        private readonly BakeContext _context;
        private const string EventApplySessionKey = "EventApplyDraft";

        public PostController(BakeContext context)
        {
            _context = context;
        }

        private int CurrentUserId =>
            int.TryParse(User.FindFirst("UserId")?.Value, out var userId) ? userId : 0;

        public IActionResult Latest()
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

            _context.EventRegistrations.Add(registration);
            await _context.SaveChangesAsync();

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

        private async Task<EventApplyViewModel?> BuildApplyViewModelAsync(int eventId)
        {
            var eventDetail = await _context.EventDetails
                .Include(e => e.Post)
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (eventDetail?.Post == null)
            {
                return null;
            }

            var account = await _context.AccountAuths
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
            var eventDetail = await _context.EventDetails
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

            var hasRegistered = await _context.EventRegistrations
                .AnyAsync(x => x.EventId == eventId && x.UserId == userId);

            if (hasRegistered)
            {
                return "你已經報名過這個活動";
            }

            return null;
        }

        private async Task<string?> ValidateCapacityAsync(int eventId, int numParticipants)
        {
            var eventDetail = await _context.EventDetails
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (eventDetail == null)
            {
                return "找不到活動資料";
            }

            var currentTotalParticipants = await _context.EventRegistrations
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
            return await _context.RegistStatusLookups
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