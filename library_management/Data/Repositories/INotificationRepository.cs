using System.Collections.Generic;
using LibraryManagementSystem.Domain.Models;

namespace LibraryManagementSystem.Data.Repositories
{
    public interface INotificationRepository
    {
        List<BookNotification> GetAllNotifications();
        BookNotification GetNotificationById(int id);
        List<BookNotification> GetNotificationsByLoanId(int loanId);
        void AddNotification(BookNotification notification);
        void SaveChanges();
    }
}
