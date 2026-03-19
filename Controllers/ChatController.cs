using Bake.Data;
using Bake.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Bake.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly BakeContext _context;

        public ChatController(BakeContext context)
        {
            _context = context;
        }

        private async Task<int> GetCurrentUserIdAsync()
        {
            string? nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(nameIdentifier, out int userId))
            {
                return userId;
            }

            string? email = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new UnauthorizedAccessException("找不到登入資訊");
            }

            int dbUserId = await _context.AccountAuths
                .Where(x => x.Email == email)
                .Select(x => x.UserId)
                .FirstOrDefaultAsync();

            if (dbUserId == 0)
            {
                throw new UnauthorizedAccessException("找不到對應的使用者");
            }

            return dbUserId;
        }

        public async Task<IActionResult> Room(int id)
        {
            int userId = await GetCurrentUserIdAsync();

            //bool isMember = await _context.ChatRoomMembers
            //    .AnyAsync(x => x.RoomId == id && x.UserId == userId);

            //if (!isMember)
            //{
            //    return Forbid();
            //}

            var messages = await _context.ChatMessages
                .Where(x => x.RoomId == id)
                .OrderBy(x => x.CreateDate)
                .Select(x => new ChatMessageItemViewModel
                {
                    MessageId = x.MessageId,
                    SenderId = x.SenderId,
                    SenderName = x.Sender.FullName,
                    Message = x.Message,
                    CreateDate = x.CreateDate
                })
                .ToListAsync();

            var memberNames = await _context.ChatRoomMembers
                .Where(x => x.RoomId == id)
                .Select(x => x.User.FullName)
                .ToListAsync();

            var vm = new ChatRoomPageViewModel
            {
                RoomId = id,
                RoomTitle = memberNames.Count > 0
                    ? string.Join("、", memberNames)
                    : $"聊天室 {id}",
                Messages = messages
            };

            return View(vm);
        }
    }
}