using System;
using System.Collections.Generic;
using System.Linq;
using LibraryManagementSystem.Data.Repositories;
using LibraryManagementSystem.Domain.Models;

namespace LibraryManagementSystem.Business.Services
{
    public class LibraryService : ILibraryService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IBookLoanRepository _loanRepository;

        public LibraryService(IBookRepository bookRepository, IBookLoanRepository loanRepository)
        {
            _bookRepository = bookRepository;
            _loanRepository = loanRepository;
        }

        // Book operations
        public List<Book> GetAllBooks() => _bookRepository.GetAllBooks();

        public Book GetBookById(int id) => _bookRepository.GetBookById(id);

        public List<Book> SearchBooks(string searchTerm, string searchBy)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAllBooks();

            searchTerm = searchTerm.ToLower();

            return searchBy.ToLower() switch
            {
                "title" => _bookRepository.SearchBooks(b => b.Title.ToLower().Contains(searchTerm)),
                "author" => _bookRepository.SearchBooks(b => b.Author.ToLower().Contains(searchTerm)),
                "isbn" => _bookRepository.SearchBooks(b => b.ISBN.ToLower().Contains(searchTerm)),
                "genre" => _bookRepository.SearchBooks(b => b.Genre.ToLower().Contains(searchTerm)),
                _ => _bookRepository.SearchBooks(b =>
                    b.Title.ToLower().Contains(searchTerm) ||
                    b.Author.ToLower().Contains(searchTerm) ||
                    b.ISBN.ToLower().Contains(searchTerm) ||
                    b.Genre.ToLower().Contains(searchTerm))
            };
        }

        public void AddBook(Book book)
        {
            _bookRepository.AddBook(book);
            _bookRepository.SaveChanges();
        }

        public void UpdateBook(Book book)
        {
            if (!_bookRepository.Exists(book.Id))
            {
                throw new ArgumentException($"Book with ID {book.Id} does not exist.", nameof(book));
            }

            _bookRepository.UpdateBook(book);
            _bookRepository.SaveChanges();
        }

        public void DeleteBook(int id)
        {
            if (!_bookRepository.Exists(id))
            {
                throw new ArgumentException($"Book with ID {id} does not exist.", nameof(id));
            }

            var loans = _loanRepository.GetLoansByBookId(id);
            if (loans.Any(l => !l.IsReturned))
            {
                throw new InvalidOperationException("Cannot delete a book that has active loans.");
            }

            _bookRepository.DeleteBook(id);
            _bookRepository.SaveChanges();
        }

        // Loan operations
        public bool BorrowBook(int bookId, string borrowerName, string borrowerEmail, int loanDays = 14)
        {
            var book = _bookRepository.GetBookById(bookId);
            if (book == null)
            {
                throw new ArgumentException($"Book with ID {bookId} does not exist.", nameof(bookId));
            }

            if (book.AvailableQuantity <= 0)
            {
                return false;
            }

            var loan = new BookLoan
            {
                BookId = bookId,
                BorrowerName = borrowerName,
                BorrowerEmail = borrowerEmail,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(loanDays)
            };

            _loanRepository.AddLoan(loan);

            // Update book availability
            book.AvailableQuantity--;
            _bookRepository.UpdateBook(book);

            _loanRepository.SaveChanges();
            _bookRepository.SaveChanges();

            return true;
        }

        public bool ReturnBook(int bookId, string borrowerEmail)
        {
            var loans = _loanRepository.GetLoansByBookId(bookId)
                .Where(l => l.BorrowerEmail.Equals(borrowerEmail, StringComparison.OrdinalIgnoreCase) && !l.IsReturned)
                .ToList();

            if (!loans.Any())
            {
                return false;
            }

            var loan = loans.First(); // Get the first active loan
            loan.ReturnDate = DateTime.Now;
            _loanRepository.UpdateLoan(loan);

            // Update book availability
            var book = _bookRepository.GetBookById(bookId);
            book.AvailableQuantity++;
            _bookRepository.UpdateBook(book);

            _loanRepository.SaveChanges();
            _bookRepository.SaveChanges();

            return true;
        }

        public List<BookLoan> GetActiveLoans() => _loanRepository.GetActiveLoans();

        public List<BookLoan> GetLoansByBookId(int bookId) => _loanRepository.GetLoansByBookId(bookId);

        // Advanced feature - Get overdue loans for notification system
        public List<BookLoan> GetOverdueLoans() => _loanRepository.GetOverdueLoans();
    }
}