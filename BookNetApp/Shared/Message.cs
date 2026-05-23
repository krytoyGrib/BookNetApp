using System.ComponentModel.DataAnnotations;

namespace BookNetApp.Shared
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public User? Sender { get; set; }
        public int ReceiverId { get; set; }
        public User? Receiver { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime DateSent { get; set; } = DateTime.Now;
    }
}
