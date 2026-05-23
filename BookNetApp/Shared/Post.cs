using System.ComponentModel.DataAnnotations;

namespace BookNetApp.Shared
{
    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; } 
        public int UserId { get; set; }
        public User? User { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
