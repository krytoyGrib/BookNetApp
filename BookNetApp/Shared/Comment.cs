namespace BookNetApp.Shared
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; } 
        public int PostId { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
