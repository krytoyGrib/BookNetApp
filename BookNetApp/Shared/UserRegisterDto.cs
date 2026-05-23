using System.ComponentModel.DataAnnotations;

namespace BookNetApp.Shared
{
    public class UserRegisterDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required, StringLength(20, MinimumLength = 3)]
        public string Username { get; set; }
        [Required, StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }
    }
}
