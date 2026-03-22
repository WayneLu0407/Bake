using Bake.Data;
using Bake.Models.Service;
using Bake.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
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

            bool isMember = await _context.ChatRoomMembers
                .AnyAsync(x => x.RoomId == id && x.UserId == userId);

            if (!isMember)
            {
                return Forbid();
            }

            var messages = await _context.ChatMessages
                .Where(x => x.RoomId == id)
                .OrderBy(x => x.CreateDate)
                .Select(x => new ChatMessageItemViewModel
                {
                    MessageId = x.MessageId,
                    SenderId = x.SenderId,
                    //SenderName =  = x.Sender.FullName?? x.Sender.FullName,
                    Message = x.Message,
                    CreateDate = x.CreateDate
                })
                .ToListAsync();

            var otherMember = await _context.ChatRoomMembers
                .Where(m => m.RoomId == id && m.UserId != userId)
                .Select(m => new 
                {
                    UserName = m.User.FullName,
                    shopName=_context.Shops.Where(s => s.UserId == m.UserId)
                        .Select(s => s.ShopName)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            var roomTile = otherMember != null ? (otherMember.shopName??otherMember.UserName):"聊天室";

            //聊天列表：顯示我參與的所有聊天室

            var chatList = await _context.ChatRoomMembers
                .Where(m=>m.UserId == userId)
                .Select(m => new ChatListItemViewModel
                {
                    RoomId = m.RoomId,

                    OtherName = m.Room.ChatRoomMembers
                    .Where(x => x.UserId != userId)
                    .Select(x =>
                        _context.Shops
                            .Where(s => s.UserId == x.UserId)
                            .Select(s => s.ShopName)
                            .FirstOrDefault()
                        ?? x.User.FullName)
                    .FirstOrDefault() ?? "未知",

                    LastMessage = m.Room.ChatMessages
                    .OrderByDescending(msg => msg.CreateDate)
                    .Select(msg => msg.Message)
                    .FirstOrDefault() ?? "",

                    LastMessageTime = m.Room.ChatMessages
                    .OrderByDescending(msg => msg.CreateDate)
                    .Select(msg => msg.CreateDate)
                    .FirstOrDefault()

                }).OrderByDescending(x => x.LastMessageTime).ToListAsync();

            var vm = new ChatRoomPageViewModel
                {
                    RoomId = id,
                    CurrentUserId = userId,
                    RoomTitle = roomTile,
                    Messages = messages,
                    ChatList = chatList
                };

            return View(vm);
        }

        public async Task<IActionResult> StartChat(int targetId) 
        {
            int currentUserId = await GetCurrentUserIdAsync();
            if (currentUserId == targetId)
                return BadRequest("無法與自己聊天");
            var existingRoom = await _context.ChatRooms
                .Where(r => r.RoomType == 0)
                .Where(r => 
                    r.ChatRoomMembers.Any(m => m.UserId == currentUserId) &&
                    r.ChatRoomMembers.Any(m => m.UserId == targetId))
                .Select(r=>r.RoomId)
                .FirstOrDefaultAsync();
            if(existingRoom != 0)
            {
                return RedirectToAction("Room", new { id = existingRoom });
            }
            // 3. 建立新的聊天室
            var newRoom = new ChatRoom
            {
                RoomType = 0,
                CreatedAt = DateTime.Now
            };
            _context.ChatRooms.Add(newRoom);
            await _context.SaveChangesAsync();
            
            // 4. 用 SQL 插入成員（繞過 EF 的關聯追蹤問題）
            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            INSERT INTO Service.Chat_Room_Member (room_id, user_id, joined_at) 
            VALUES ({newRoom.RoomId}, {currentUserId}, SYSDATETIME()),
                   ({newRoom.RoomId}, {targetId}, SYSDATETIME())
            ");

            // 5. 跳轉到聊天室頁面
            return RedirectToAction("Room", new { id = newRoom.RoomId });
        }
    }
}