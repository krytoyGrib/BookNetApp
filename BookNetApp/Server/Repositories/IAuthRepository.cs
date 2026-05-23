using BookNetApp.Shared;

namespace BookNetApp.Server.Repositories
{
    public interface IAuthRepository
    {
        Task<int> Register(User user, string password);
        Task<string> Login(string username, string password);
        Task<bool> UserExists(string username);
        Task<User?> GetUserById(int id);
    }
}
