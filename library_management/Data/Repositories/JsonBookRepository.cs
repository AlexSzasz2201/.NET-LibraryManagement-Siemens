using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibraryManagementSystem.Domain.Models;
using Newtonsoft.Json;

namespace LibraryManagementSystem.Data.Repositories
{
    public class JsonBookRepository : IBookRepository
    {
        private readonly string _filePath;
        private List<Book> _books;

        public JsonBookRepository(string filePath)
        {
            _filePath = filePath;
            LoadBooks();
        }

        private void LoadBooks()
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                _books = JsonConvert.DeserializeObject<List<Book>>(json) ?? new List<Book>();
            }
            else
            {
                _books = new List<Book>();
                SaveChanges(); // Create the file
            }
        }

        public List<Book> GetAllBooks() => _books.ToList();

        public Book GetBookById(int id) => _books.FirstOrDefault(b => b.Id == id);

        public List<Book> SearchBooks(Func<Book, bool> predicate) => _books.Where(predicate).ToList();

        public void AddBook(Book book)
        {
            if (book.Id == 0)
            {
                book.Id = _books.Count > 0 ? _books.Max(b => b.Id) + 1 : 1;
            }

            book.AvailableQuantity = book.TotalQuantity;
            _books.Add(book);
        }

        public void UpdateBook(Book book)
        {
            var existingBook = _books.FirstOrDefault(b => b.Id == book.Id);
            if (existingBook != null)
            {
                var index = _books.IndexOf(existingBook);
                _books[index] = book;
            }
        }

        public void DeleteBook(int id)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);
            if (book != null)
            {
                _books.Remove(book);
            }
        }

        public bool Exists(int id) => _books.Any(b => b.Id == id);

        public void SaveChanges()
        {
            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonConvert.SerializeObject(_books, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
    }
}
