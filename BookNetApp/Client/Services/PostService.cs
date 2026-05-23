using BookNetApp.Shared;
using System.Net.Http.Json;

namespace BookNetApp.Client.Services
{
    public interface IPostService
    {
        List<Post> Posts { get; set; }
        Task GetPosts();
        Task CreatePost(Post post);
        Task UpdatePost(Post post);
        Task DeletePost(int postId);
        Task AddComment(Comment comment);
        Task DeleteComment(int commentId);
    }

    public class PostService : IPostService
    {
        private readonly HttpClient _http;
        public List<Post> Posts { get; set; } = new List<Post>();

        public PostService(HttpClient http) => _http = http;

        public async Task GetPosts()
        {
            var result = await _http.GetAsync("api/post");

            if (result.IsSuccessStatusCode)
            {
                Posts = await result.Content.ReadFromJsonAsync<List<Post>>() ?? new();
            }
            else
            {
                Posts = new List<Post>();
            }
        }


        public async Task CreatePost(Post post) => await _http.PostAsJsonAsync("api/post", post);
        public async Task UpdatePost(Post post) => await _http.PutAsJsonAsync($"api/post/{post.Id}", post);
        public async Task DeletePost(int postId) => await _http.DeleteAsync($"api/post/{postId}");
        public async Task AddComment(Comment comment) => await _http.PostAsJsonAsync("api/post/comment", comment);
        public async Task DeleteComment(int commentId) => await _http.DeleteAsync($"api/post/comment/{commentId}");
    }
}
