using Microsoft.AspNetCore.SignalR;
using WEBCOMIC_FINALPROJECT_.Data; // Thay bằng namespace thực tế của bạn
using WEBCOMIC_FINALPROJECT_.Models;

namespace WEBCOMIC_FINALPROJECT_.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string user, string message)
        {
            // 1. Tạo đối tượng tin nhắn
            var chatMsg = new ChatMessage
            {
                User = user,
                Message = message,
                SentAt = DateTime.Now
            };

            // 2. Lưu vào Database
            _context.ChatMessages.Add(chatMsg);
            await _context.SaveChangesAsync();

            // 3. Gửi tin nhắn đến tất cả mọi người (Real-time)
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}