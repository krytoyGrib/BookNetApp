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
    public class BookController : ControllerBase
    {
        private readonly DataContext _context;

        public BookController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Book>>> GetAllBooks()
        {
            return await _context.Books.ToListAsync();
        }

        [HttpGet("my"), Authorize]
        public async Task<ActionResult<List<Book>>> GetMyBooks()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return await _context.Books.Where(b => b.UserId == userId).ToListAsync();
        }

        [HttpPost, Authorize]
        public async Task<ActionResult> AddBook(Book book)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var exists = await _context.Books.AnyAsync(b =>
                b.UserId == userId &&
                b.Title.ToLower() == book.Title.ToLower() &&
                b.Author.ToLower() == book.Author.ToLower());

            if (exists)
            {
                return BadRequest("Эта книга уже есть в вашей библиотеке.");
            }

            book.UserId = userId;
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("update-status/{id}"), Authorize]
        public async Task<ActionResult> UpdateBookStatus(int id, [FromBody] BookStatus newStatus)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (book == null) return NotFound();

            book.Status = newStatus;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}"), Authorize]
        public async Task<ActionResult> DeleteBook(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (book == null) return NotFound();

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<Book>>> GetUserBooks(int userId)
        {
            return await _context.Books.Where(b => b.UserId == userId).ToListAsync();
        }
    }
}
