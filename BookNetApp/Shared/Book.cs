using System.ComponentModel.DataAnnotations;

namespace BookNetApp.Shared
{
    public enum BookStatus
    {
        Reading = 0,    
        PlanToRead = 1, 
        Finished = 2   
    }

    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название книги")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите автора")]
        public string Author { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string CoverUrl { get; set; } = string.Empty;


        public BookStatus Status { get; set; } = BookStatus.PlanToRead;

        public string UserComment { get; set; } = string.Empty;

        public int UserId { get; set; }
    }
}
