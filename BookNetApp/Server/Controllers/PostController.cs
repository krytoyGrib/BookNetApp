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
    public class PostController : ControllerBase
    {
        private readonly DataContext _context;

        public PostController(DataContext context)
        {
            _context = context;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        [HttpGet]
        public async Task<ActionResult<List<Post>>> GetPosts()
        {
            return await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)
                .OrderByDescending(p => p.DateCreated)
                .ToListAsync();
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreatePost(Post post)
        {
            var newPost = new Post
            {
                Content = post.Content,
                UserId = GetUserId(),
                DateCreated = DateTime.Now
            };

            _context.Posts.Add(newPost);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> UpdatePost(int id, Post post)
        {
            var dbPost = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == GetUserId());

            if (dbPost == null)
            {
                return NotFound("Пост не найден или у вас нет прав на его редактирование.");
            }

            dbPost.Content = post.Content;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeletePost(int id)
        {
            var dbPost = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == GetUserId());

            if (dbPost == null)
            {
                return NotFound("Пост не найден или у вас нет прав на удаление.");
            }

            _context.Posts.Remove(dbPost);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("comment")]
        [Authorize]
        public async Task<ActionResult> AddComment(Comment comment)
        {
            var newComment = new Comment
            {
                Text = comment.Text,
                PostId = comment.PostId,
                UserId = GetUserId(),
                DateCreated = DateTime.Now
            };

            _context.Comments.Add(newComment);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("comment/{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteComment(int id)
        {
            var dbComment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == GetUserId());

            if (dbComment == null)
            {
                return NotFound("Комментарий не найден или у вас нет прав на удаление.");
            }

            _context.Comments.Remove(dbComment);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
