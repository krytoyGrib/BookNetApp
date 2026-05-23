using BookNetApp.Shared;
using System.Net.Http.Json;

namespace BookNetApp.Client.Services
{
    public interface IBookService
    {
        List<Book> AllBooks { get; set; }
        List<Book> MyBooks { get; set; }
        Task GetAllBooks();
        Task GetMyBooks();
        Task AddBook(Book book);
        Task UpdateStatus(int id, BookStatus status);
        Task DeleteBook(int id);
    }

    public class BookService : IBookService
    {
        private readonly HttpClient _http;

        public BookService(HttpClient http)
        {
            _http = http;
        }

        public List<Book> AllBooks { get; set; } = new List<Book>();
        public List<Book> MyBooks { get; set; } = new List<Book>();

        public async Task GetAllBooks() => AllBooks = await _http.GetFromJsonAsync<List<Book>>("api/book") ?? new();
        public async Task GetMyBooks() => MyBooks = await _http.GetFromJsonAsync<List<Book>>("api/book/my") ?? new();
        public async Task AddBook(Book book) => await _http.PostAsJsonAsync("api/book", book);
        public async Task UpdateStatus(int id, BookStatus status) => await _http.PutAsJsonAsync($"api/book/{id}", status);
        public async Task DeleteBook(int id) => await _http.DeleteAsync($"api/book/{id}");
    }
}
