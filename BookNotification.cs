using System;

namespace LibraryManagementSystem.Domain.Models
{
    public class BookNotification
    {
        public int Id { get; set; }
        public int LoanId { get; set; }
        public DateTime NotificationDate { get; set; }
        public string NotificationType { get; set; } // "Overdue", "ReminderBeforeDue", etc.
        public string NotificationContent { get; set; }
        public bool IsSuccessful { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, LoanId: {LoanId}, Type: {NotificationType}, " +
                   $"Date: {NotificationDate:yyyy-MM-dd HH:mm}, Status: {(IsSuccessful ? "Sent" : "Failed")}";
        }
    }
}