using System.Collections.Generic;
using LibraryManagementSystem.Domain.Models;

namespace LibraryManagementSystem.Business.Services
{
    public interface ILibraryService
    {
        // Book operations
        List<Book> GetAllBooks();
        Book GetBookById(int id);
        List<Book> SearchBooks(string searchTerm, string searchBy);
        void AddBook(Book book);
        void UpdateBook(Book book);
        void DeleteBook(int id);

        // Loan operations
        bool BorrowBook(int bookId, string borrowerName, string borrowerEmail, int loanDays = 14);
        bool ReturnBook(int bookId, string borrowerEmail);
        List<BookLoan> GetActiveLoans();
        List<BookLoan> GetLoansByBookId(int bookId);

        // Advanced feature - overdue notification system
        List<BookLoan> GetOverdueLoans();
    }
}
