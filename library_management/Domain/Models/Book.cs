using System;
using System.Text.Json.Serialization;

namespace LibraryManagementSystem.Domain.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public int PublicationYear { get; set; }
        public string Genre { get; set; }
        public int TotalQuantity { get; set; }
        public int AvailableQuantity { get; set; }

        [JsonIgnore]
        public bool IsAvailable => AvailableQuantity > 0;

        public override string ToString()
        {
            return $"Id: {Id}, Title: {Title}, Author: {Author}, Available: {AvailableQuantity}/{TotalQuantity}";
        }
    }
}