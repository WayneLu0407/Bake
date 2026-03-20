using Bake.Data;
using Bake.Models.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Security.Claims;


namespace Bake.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly BakeContext _context;
        public ChatHub(BakeContext context)
        {
            _context = context;
        }
        public async Task JoinRoom(int roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Room-{roomId}");
        }

        public async Task SendMessage(int roomId, string message)
        {
            int senderId = await GetUserId();

            var chatMessage = new ChatMessage
            {
                RoomId = roomId,
                SenderId = senderId,
                Message = message,
                CreateDate = DateTime.Now,
                IsRead = false
            };

            _context.ChatMessages.Add(chatMessage);
            await _context.SaveChangesAsync();

            string senderName = await _context.UserProfiles
                .Where(u => u.UserId == senderId)
                .Select(u => u.FullName)
                .FirstOrDefaultAsync() ?? "未知";

            await Clients.Group($"Room-{roomId}").SendAsync("ReceiveMessage", new
            {
                MessageId = chatMessage.MessageId,
                SenderId = senderId,
                SenderName = senderName,
                Message = message,
                CreateDate = chatMessage.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }

        private async Task<int> GetUserId()
        {
            var claim = Context.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null && int.TryParse(claim.Value, out int userId))
            {
                return userId;
            }
            var email = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new HubException("找不到登入資訊");
            }
            int dbUserId = await _context.AccountAuths
                .Where(x => x.Email == email)
                .Select(x => x.UserId)
                .FirstOrDefaultAsync();
            if (dbUserId == 0)
            {
                throw new HubException("找不到對應的使用者");
            }
            return dbUserId;
        }
    }
}