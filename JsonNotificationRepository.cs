using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibraryManagementSystem.Domain.Models;
using Newtonsoft.Json;

namespace LibraryManagementSystem.Data.Repositories
{
    public class JsonNotificationRepository : INotificationRepository
    {
        private readonly string _filePath;
        private List<BookNotification> _notifications;

        public JsonNotificationRepository(string filePath)
        {
            _filePath = filePath;
            LoadNotifications();
        }

        private void LoadNotifications()
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                _notifications = JsonConvert.DeserializeObject<List<BookNotification>>(json) ?? new List<BookNotification>();
            }
            else
            {
                _notifications = new List<BookNotification>();
                SaveChanges(); // Create the file
            }
        }

        public List<BookNotification> GetAllNotifications() => _notifications.ToList();

        public BookNotification GetNotificationById(int id) => _notifications.FirstOrDefault(n => n.Id == id);

        public List<BookNotification> GetNotificationsByLoanId(int loanId) =>
            _notifications.Where(n => n.LoanId == loanId).ToList();

        public void AddNotification(BookNotification notification)
        {
            if (notification.Id == 0)
            {
                notification.Id = _notifications.Count > 0 ? _notifications.Max(n => n.Id) + 1 : 1;
            }

            _notifications.Add(notification);
        }

        public void SaveChanges()
        {
            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonConvert.SerializeObject(_notifications, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
    }
}
