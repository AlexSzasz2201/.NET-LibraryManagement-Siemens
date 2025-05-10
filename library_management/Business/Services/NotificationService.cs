using System;
using System.Collections.Generic;
using System.Linq;
using LibraryManagementSystem.Data.Repositories;
using LibraryManagementSystem.Domain.Models;

namespace LibraryManagementSystem.Business.Services
{
    public class NotificationService
    {
        private readonly IBookLoanRepository _loanRepository;
        private readonly IBookRepository _bookRepository;
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(
            IBookLoanRepository loanRepository,
            IBookRepository bookRepository,
            INotificationRepository notificationRepository)
        {
            _loanRepository = loanRepository;
            _bookRepository = bookRepository;
            _notificationRepository = notificationRepository;
        }

        public List<(BookLoan Loan, Book Book, int DaysOverdue)> GetOverdueLoansWithDetails()
        {
            var overdueLoans = _loanRepository.GetOverdueLoans();
            var result = new List<(BookLoan, Book, int)>();

            foreach (var loan in overdueLoans)
            {
                var book = _bookRepository.GetBookById(loan.BookId);
                var daysOverdue = (int)(DateTime.Now - loan.DueDate).TotalDays;
                result.Add((loan, book, daysOverdue));
            }

            return result.OrderByDescending(x => x.Item3).ToList();
        }

        public void SendOverdueNotification(BookLoan loan, Book book)
        {
            // In a real application, this would send an actual email
            // For this demo, we'll just generate the content and log it

            int daysOverdue = (int)(DateTime.Now - loan.DueDate).TotalDays;

            string emailSubject = $"Overdue Book: {book.Title}";
            string emailContent = $@"Dear {loan.BorrowerName},

This is a friendly reminder that the book ""{book.Title}"" by {book.Author} is now {daysOverdue} days overdue.

The book was due on {loan.DueDate:yyyy-MM-dd}.

Please return the book to the library at your earliest convenience.

Thank you,
Library Management System";

            Console.WriteLine("\n=== NOTIFICATION EMAIL ===");
            Console.WriteLine($"To: {loan.BorrowerEmail}");
            Console.WriteLine($"Subject: {emailSubject}");
            Console.WriteLine("\nContent:");
            Console.WriteLine(emailContent);

            // Record the notification
            var notification = new BookNotification
            {
                LoanId = loan.Id,
                NotificationDate = DateTime.Now,
                NotificationType = "Overdue",
                NotificationContent = emailContent,
                IsSuccessful = true // Assuming success for this demo
            };

            _notificationRepository.AddNotification(notification);
            _notificationRepository.SaveChanges();
        }

        public List<BookNotification> GetNotificationsByLoanId(int loanId)
        {
            return _notificationRepository.GetNotificationsByLoanId(loanId);
        }
    }
}
