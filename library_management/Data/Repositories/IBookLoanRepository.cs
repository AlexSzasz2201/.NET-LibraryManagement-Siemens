using System.Collections.Generic;
using LibraryManagementSystem.Domain.Models;

namespace LibraryManagementSystem.Data.Repositories
{
    public interface IBookLoanRepository
    {
        List<BookLoan> GetAllLoans();
        BookLoan GetLoanById(int id);
        List<BookLoan> GetLoansByBookId(int bookId);
        List<BookLoan> GetLoansByBorrowerEmail(string email);
        List<BookLoan> GetActiveLoans();
        List<BookLoan> GetOverdueLoans();
        void AddLoan(BookLoan loan);
        void UpdateLoan(BookLoan loan);
        void DeleteLoan(int id);
        bool Exists(int id);
        void SaveChanges();
    }
}