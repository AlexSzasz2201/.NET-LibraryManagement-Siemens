using System;
using System.Text.Json.Serialization;

namespace LibraryManagementSystem.Domain.Models
{
    public class BookLoan
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BorrowerName { get; set; }
        public string BorrowerEmail { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        [JsonIgnore]
        public bool IsReturned => ReturnDate.HasValue;

        [JsonIgnore]
        public bool IsOverdue => !IsReturned && DateTime.Now > DueDate;

        [JsonIgnore]
        public int DaysOverdue => IsOverdue ? (int)(DateTime.Now - DueDate).TotalDays : 0;

        public override string ToString()
        {
            return $"Id: {Id}, BookId: {BookId}, Borrower: {BorrowerName}, " +
                   $"BorrowDate: {BorrowDate:yyyy-MM-dd}, DueDate: {DueDate:yyyy-MM-dd}, " +
                   $"Status: {(IsReturned ? $"Returned on {ReturnDate:yyyy-MM-dd}" : IsOverdue ? $"Overdue by {DaysOverdue} days" : "Active")}";
        }
    }
}
