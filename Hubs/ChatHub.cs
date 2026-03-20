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
            int senderId = GetUserId();

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

        private int GetUserId()
        {
            var claim = Context.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null && int.TryParse(claim.Value, out int userId))
            {
                return userId;
            }
            throw new HubException("無法取得登入者 ID");
        }
    }
}