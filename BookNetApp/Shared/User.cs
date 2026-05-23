namespace BookNetApp.Shared
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
        public string Bio { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty; 

    }
}
    