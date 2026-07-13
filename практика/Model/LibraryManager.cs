using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;

#nullable disable

namespace практика.Model
{
    /// <summary>
    /// Класс управления библиотекой
    /// </summary>
    public class LibraryManager : INotifyPropertyChanged
    {
        private ObservableCollection<Book> _books;
        private string _saveFilePath;

        public event EventHandler BooksChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Book> Books
        {
            get => _books;
            set
            {
                _books = value;
                OnPropertyChanged();
                OnBooksChanged();
            }
        }

        public LibraryManager(string saveFilePath = "books.json")
        {
            _saveFilePath = saveFilePath;
            _books = new ObservableCollection<Book>();
            LoadFromFile();
        }

        public void AddBook(Book book)
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book), "Книга не может быть null");

            book.Id = _books.Count > 0 ? _books.Max(b => b.Id) + 1 : 1;
            _books.Add(book);
            SaveToFile();
            OnBooksChanged();
        }

        public void EditBook(int id, string title, string author, string genre, bool isAvailable)
        {
            Book book = _books.FirstOrDefault(b => b.Id == id);
            if (book == null)
                throw new Exception($"Книга с ID {id} не найдена");

            book.Title = title;
            book.Author = author;
            book.Genre = genre;
            book.IsAvailable = isAvailable;

            SaveToFile();
            OnBooksChanged();
        }

        public void DeleteBook(int id)
        {
            Book book = _books.FirstOrDefault(b => b.Id == id);
            if (book == null)
                throw new Exception($"Книга с ID {id} не найдена");

            _books.Remove(book);
            SaveToFile();
            OnBooksChanged();
        }

        public List<Book> SearchBooks(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return _books.ToList();

            keyword = keyword.ToLower();
            return _books
                .Where(b => b.Title.ToLower().Contains(keyword) ||
                            b.Author.ToLower().Contains(keyword) ||
                            b.Genre.ToLower().Contains(keyword))
                .ToList();
        }

        public List<Book> FilterByStatus(bool isAvailable)
        {
            return _books.Where(b => b.IsAvailable == isAvailable).ToList();
        }

        public List<Book> FilterByGenre(string genre)
        {
            if (string.IsNullOrWhiteSpace(genre))
                return _books.ToList();

            return _books.Where(b => b.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public List<Book> SortBooks(string sortBy)
        {
            switch (sortBy)
            {
                case "Название":
                    return _books.OrderBy(b => b.Title).ToList();
                case "Автор":
                    return _books.OrderBy(b => b.Author).ToList();
                case "Жанр":
                    return _books.OrderBy(b => b.Genre).ToList();
                default:
                    return _books.ToList();
            }
        }

        public List<string> GetAllGenres()
        {
            return _books.Select(b => b.Genre).Distinct().OrderBy(g => g).ToList();
        }

        public void SaveToFile()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                string json = JsonSerializer.Serialize(_books, options);
                File.WriteAllText(_saveFilePath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка сохранения файла: {ex.Message}");
            }
        }

        public void LoadFromFile()
        {
            try
            {
                if (File.Exists(_saveFilePath))
                {
                    string json = File.ReadAllText(_saveFilePath);
                    var loaded = JsonSerializer.Deserialize<List<Book>>(json);
                    if (loaded != null)
                    {
                        _books.Clear();
                        foreach (var book in loaded)
                            _books.Add(book);
                    }
                }
                else
                {
                    AddTestBooks();
                }
            }
            catch (Exception)
            {
                _books.Clear();
                AddTestBooks();
            }
        }

        private void AddTestBooks()
        {
            _books.Add(new Book(1, "Война и мир", "Лев Толстой", "Роман", true));
            _books.Add(new Book(2, "Преступление и наказание", "Фёдор Достоевский", "Роман", false));
            _books.Add(new Book(3, "Мастер и Маргарита", "Михаил Булгаков", "Роман", true));
            _books.Add(new Book(4, "Евгений Онегин", "Александр Пушкин", "Поэзия", true));
            _books.Add(new Book(5, "Мёртвые души", "Николай Гоголь", "Роман", false));
            SaveToFile();
        }

        private void OnBooksChanged()
        {
            BooksChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}