using System;

namespace практика.Model
{
    /// <summary>
    /// Класс, представляющий книгу
    /// </summary>
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public bool IsAvailable { get; set; }  

        public Book()
        {
            Id = 0;
            Title = string.Empty;
            Author = string.Empty;
            Genre = string.Empty;
            IsAvailable = true;
        }

        public Book(int id, string title, string author, string genre, bool isAvailable)
        {
            Id = id;
            Title = title;
            Author = author;
            Genre = genre;
            IsAvailable = isAvailable;
        }

        public override string ToString()
        {
            return $"{Title} - {Author} ({Genre}) {(IsAvailable ? "Доступна" : "Занята")}";
        }
    }
}