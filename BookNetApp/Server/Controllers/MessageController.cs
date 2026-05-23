using BookNetApp.Server.Data;
using BookNetApp.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookNetApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly DataContext _context;
        public MessageController(DataContext context) => _context = context;

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        [HttpGet("{otherUserId}")]
        public async Task<ActionResult<List<Message>>> GetChatHistory(int otherUserId)
        {
            var myId = GetUserId(); 
            return await _context.Messages
                .Where(m => (m.SenderId == myId && m.ReceiverId == otherUserId) ||
                            (m.SenderId == otherUserId && m.ReceiverId == myId))
                .OrderBy(m => m.DateSent)
                .ToListAsync();
        }


        [HttpPost]
        public async Task<ActionResult> SendMessage(Message message)
        {
            message.SenderId = GetUserId();
            message.DateSent = DateTime.Now;
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpGet("chats")]
        public async Task<ActionResult<List<ChatSummaryDto>>> GetMyChats()
        {
            var myId = GetUserId();

            var messages = await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => m.SenderId == myId || m.ReceiverId == myId)
                .OrderByDescending(m => m.DateSent)
                .ToListAsync();

            var chats = messages
                .GroupBy(m => m.SenderId == myId ? m.ReceiverId : m.SenderId)
                .Select(g => new ChatSummaryDto
                {
                    UserId = g.Key,
                    UserName = g.First().SenderId == myId ? g.First().Receiver?.Username : g.First().Sender?.Username,
                    LastMessage = g.First().Content
                }).ToList();

            return Ok(chats);
        }

    }
}
