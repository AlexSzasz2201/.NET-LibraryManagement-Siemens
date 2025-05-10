using System;
using System.Collections.Generic;
using LibraryManagementSystem.Domain.Models;

namespace LibraryManagementSystem.Data.Repositories
{
    public interface IBookRepository
    {
        List<Book> GetAllBooks();
        Book GetBookById(int id);
        List<Book> SearchBooks(Func<Book, bool> predicate);
        void AddBook(Book book);
        void UpdateBook(Book book);
        void DeleteBook(int id);
        bool Exists(int id);
        void SaveChanges();
    }
}