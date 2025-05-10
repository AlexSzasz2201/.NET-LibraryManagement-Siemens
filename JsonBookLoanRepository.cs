using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibraryManagementSystem.Domain.Models;
using Newtonsoft.Json;

namespace LibraryManagementSystem.Data.Repositories
{
    public class JsonBookLoanRepository : IBookLoanRepository
    {
        private readonly string _filePath;
        private List<BookLoan> _loans;

        public JsonBookLoanRepository(string filePath)
        {
            _filePath = filePath;
            LoadLoans();
        }

        private void LoadLoans()
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                _loans = JsonConvert.DeserializeObject<List<BookLoan>>(json) ?? new List<BookLoan>();
            }
            else
            {
                _loans = new List<BookLoan>();
                SaveChanges(); // Create the file
            }
        }

        public List<BookLoan> GetAllLoans() => _loans.ToList();

        public BookLoan GetLoanById(int id) => _loans.FirstOrDefault(l => l.Id == id);

        public List<BookLoan> GetLoansByBookId(int bookId) => _loans.Where(l => l.BookId == bookId).ToList();

        public List<BookLoan> GetLoansByBorrowerEmail(string email) =>
            _loans.Where(l => l.BorrowerEmail.Equals(email, StringComparison.OrdinalIgnoreCase)).ToList();

        public List<BookLoan> GetActiveLoans() => _loans.Where(l => !l.IsReturned).ToList();

        public List<BookLoan> GetOverdueLoans() =>
            _loans.Where(l => !l.IsReturned && l.DueDate < DateTime.Now).ToList();

        public void AddLoan(BookLoan loan)
        {
            if (loan.Id == 0)
            {
                loan.Id = _loans.Count > 0 ? _loans.Max(l => l.Id) + 1 : 1;
            }

            _loans.Add(loan);
        }

        public void UpdateLoan(BookLoan loan)
        {
            var existingLoan = _loans.FirstOrDefault(l => l.Id == loan.Id);
            if (existingLoan != null)
            {
                var index = _loans.IndexOf(existingLoan);
                _loans[index] = loan;
            }
        }

        public void DeleteLoan(int id)
        {
            var loan = _loans.FirstOrDefault(l => l.Id == id);
            if (loan != null)
            {
                _loans.Remove(loan);
            }
        }

        public bool Exists(int id) => _loans.Any(l => l.Id == id);

        public void SaveChanges()
        {
            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonConvert.SerializeObject(_loans, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
    }
}