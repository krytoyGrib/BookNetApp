using BookNetApp.Server.Data;
using BookNetApp.Server.Repositories;
using BookNetApp.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookNetApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;
        private readonly DataContext _context;

        public AuthController(IAuthRepository authRepo, DataContext context)
        {
            _authRepo = authRepo;
            _context = context;
        }

        [HttpGet("me"), Authorize]
        public async Task<ActionResult<User>> GetCurrentUserInfo()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString)) return Unauthorized();

            var userId = int.Parse(userIdString);
            var user = await _authRepo.GetUserById(userId);

            if (user == null) return NotFound("Пользователь не найден");

            return Ok(user);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<User>> GetUserInfo(int userId)
        {
            var user = await _authRepo.GetUserById(userId);
            if (user == null) return NotFound();

            return Ok(new User
            {
                Id = user.Id,
                Username = user.Username,
                AvatarUrl = user.AvatarUrl,
                Bio = user.Bio
            });
        }

        [HttpGet("search/{query}")]
        [Authorize]
        public async Task<ActionResult<List<User>>> SearchUsers(string query)
        {
            var myId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (query != "null" && !string.IsNullOrWhiteSpace(query))
            {
                var users = await _context.Users
                    .Where(u => u.Id != myId && u.Username.ToLower().Contains(query.ToLower()))
                    .Select(u => new User
                    {
                        Id = u.Id,
                        Username = u.Username,
                        AvatarUrl = u.AvatarUrl,
                        Bio = u.Bio
                    })
                    .ToListAsync();

                return Ok(users);
            }

            var myBookTitles = await _context.Books
                .Where(b => b.UserId == myId && b.Title != null)
                .Select(b => b.Title.ToLower())
                .ToListAsync();

            if (myBookTitles == null || !myBookTitles.Any())
            {
                var randomUsers = await _context.Users
                    .Where(u => u.Id != myId)
                    .Take(5)
                    .Select(u => new User { Id = u.Id, Username = u.Username, AvatarUrl = u.AvatarUrl, Bio = u.Bio })
                    .ToListAsync();

                return Ok(randomUsers);
            }

            var recommendedUserIds = await _context.Books
                .Where(b => b.UserId != myId && myBookTitles.Contains(b.Title.ToLower()))
                .GroupBy(b => b.UserId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .Take(5)
                .ToListAsync();

            var recommendedUsers = await _context.Users
                .Where(u => recommendedUserIds.Contains(u.Id))
                .Select(u => new User
                {
                    Id = u.Id,
                    Username = u.Username,
                    AvatarUrl = u.AvatarUrl,
                    Bio = u.Bio
                })
                .ToListAsync();

            return Ok(recommendedUsers);
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> Register(UserRegisterDto request)
        {
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                Bio = "Привет! Я новый пользователь BookNet.",
                AvatarUrl = "default_avatar.png"
            };

            var userId = await _authRepo.Register(user, request.Password);

            if (userId == -1)
            {
                return BadRequest("Пользователь уже существует.");
            }

            var token = await _authRepo.Login(request.Username, request.Password);
            return Ok(token);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserLoginDto request)
        {
            var response = await _authRepo.Login(request.Username, request.Password);

            if (response == "user_not_found")
            {
                return BadRequest("Пользователь не найден.");
            }
            else if (response == "wrong_password")
            {
                return BadRequest("Неверный пароль.");
            }

            return Ok(response);
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<ActionResult> UpdateProfile(User updatedUser)
        {
            var myId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users.FindAsync(myId);

            if (user == null) return NotFound();

            user.Bio = updatedUser.Bio;
            user.AvatarUrl = updatedUser.AvatarUrl;

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
