using System;
using System.Collections.Generic;
using System.Linq;
using LibraryManagementSystem.Business.Services;
using LibraryManagementSystem.Domain.Models;

namespace LibraryManagementSystem.Presentation
{
    public class LibraryConsoleUI
    {
        private readonly ILibraryService _libraryService;
        private readonly NotificationService _notificationService;

        public LibraryConsoleUI(ILibraryService libraryService, NotificationService notificationService)
        {
            _libraryService = libraryService;
            _notificationService = notificationService;
        }

        public void Run()
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("==== Library Management System ====");
                Console.WriteLine("1. View All Books");
                Console.WriteLine("2. Search Books");
                Console.WriteLine("3. Add New Book");
                Console.WriteLine("4. Update Book");
                Console.WriteLine("5. Delete Book");
                Console.WriteLine("6. Borrow Book");
                Console.WriteLine("7. Return Book");
                Console.WriteLine("8. View Active Loans");
                Console.WriteLine("9. Check Overdue Books"); // Advanced feature
                Console.WriteLine("0. Exit");
                Console.Write("\nEnter your choice: ");

                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.Clear();

                    switch (choice)
                    {
                        case 1:
                            DisplayAllBooks();
                            break;
                        case 2:
                            SearchBooks();
                            break;
                        case 3:
                            AddBook();
                            break;
                        case 4:
                            UpdateBook();
                            break;
                        case 5:
                            DeleteBook();
                            break;
                        case 6:
                            BorrowBook();
                            break;
                        case 7:
                            ReturnBook();
                            break;
                        case 8:
                            ViewActiveLoans();
                            break;
                        case 9:
                            CheckOverdueBooks(); // Advanced feature
                            break;
                        case 0:
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Press any key to continue...");
                            Console.ReadKey();
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        private void DisplayAllBooks()
        {
            var books = _libraryService.GetAllBooks();

            Console.WriteLine("==== All Books ====");
            if (books.Count == 0)
            {
                Console.WriteLine("No books in the library.");
            }
            else
            {
                DisplayBooks(books);
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void DisplayBooks(List<Book> books)
        {
            Console.WriteLine("\n{0,-5} {1,-30} {2,-20} {3,-10} {4,-15} {5,-5}",
                "ID", "Title", "Author", "Genre", "ISBN", "Available");
            Console.WriteLine(new string('-', 90));

            foreach (var book in books)
            {
                Console.WriteLine("{0,-5} {1,-30} {2,-20} {3,-10} {4,-15} {5,-5}/{6,-5}",
                    book.Id,
                    book.Title.Length > 28 ? book.Title.Substring(0, 25) + "..." : book.Title,
                    book.Author.Length > 18 ? book.Author.Substring(0, 15) + "..." : book.Author,
                    book.Genre,
                    book.ISBN,
                    book.AvailableQuantity,
                    book.TotalQuantity);
            }
        }

        private void SearchBooks()
        {
            Console.WriteLine("==== Search Books ====");
            Console.WriteLine("Search by: ");
            Console.WriteLine("1. Title");
            Console.WriteLine("2. Author");
            Console.WriteLine("3. ISBN");
            Console.WriteLine("4. Genre");
            Console.WriteLine("5. All Fields");
            Console.Write("\nEnter your choice: ");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= 5)
            {
                string searchBy = choice switch
                {
                    1 => "title",
                    2 => "author",
                    3 => "isbn",
                    4 => "genre",
                    _ => "all"
                };

                Console.Write("\nEnter search term: ");
                string searchTerm = Console.ReadLine() ?? string.Empty;

                var books = _libraryService.SearchBooks(searchTerm, searchBy);

                Console.WriteLine($"\n==== Search Results ({books.Count} books found) ====");
                if (books.Count == 0)
                {
                    Console.WriteLine("No books found matching your search criteria.");
                }
                else
                {
                    DisplayBooks(books);
                }
            }
            else
            {
                Console.WriteLine("Invalid choice.");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void AddBook()
        {
            Console.WriteLine("==== Add New Book ====");

            var book = new Book();

            Console.Write("Title: ");
            book.Title = Console.ReadLine() ?? string.Empty;

            Console.Write("Author: ");
            book.Author = Console.ReadLine() ?? string.Empty;

            Console.Write("ISBN: ");
            book.ISBN = Console.ReadLine() ?? string.Empty;

            Console.Write("Publisher: ");
            book.Publisher = Console.ReadLine() ?? string.Empty;

            Console.Write("Publication Year: ");
            if (int.TryParse(Console.ReadLine(), out int year))
            {
                book.PublicationYear = year;
            }

            Console.Write("Genre: ");
            book.Genre = Console.ReadLine() ?? string.Empty;

            Console.Write("Quantity: ");
            if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
            {
                book.TotalQuantity = quantity;
            }
            else
            {
                book.TotalQuantity = 1;
            }

            try
            {
                _libraryService.AddBook(book);
                Console.WriteLine("\nBook added successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError adding book: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void UpdateBook()
        {
            Console.WriteLine("==== Update Book ====");
            Console.Write("Enter Book ID to update: ");

            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var book = _libraryService.GetBookById(id);

                if (book == null)
                {
                    Console.WriteLine("\nBook not found.");
                }
                else
                {
                    Console.WriteLine("\nCurrent Book Details:");
                    Console.WriteLine($"Title: {book.Title}");
                    Console.WriteLine($"Author: {book.Author}");
                    Console.WriteLine($"ISBN: {book.ISBN}");
                    Console.WriteLine($"Publisher: {book.Publisher}");
                    Console.WriteLine($"Publication Year: {book.PublicationYear}");
                    Console.WriteLine($"Genre: {book.Genre}");
                    Console.WriteLine($"Total Quantity: {book.TotalQuantity}");
                    Console.WriteLine($"Available Quantity: {book.AvailableQuantity}");

                    Console.WriteLine("\nEnter new details (leave blank to keep current value):");

                    Console.Write("Title: ");
                    string title = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(title))
                    {
                        book.Title = title;
                    }

                    Console.Write("Author: ");
                    string author = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(author))
                    {
                        book.Author = author;
                    }

                    Console.Write("ISBN: ");
                    string isbn = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(isbn))
                    {
                        book.ISBN = isbn;
                    }

                    Console.Write("Publisher: ");
                    string publisher = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(publisher))
                    {
                        book.Publisher = publisher;
                    }

                    Console.Write("Publication Year: ");
                    string yearStr = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(yearStr) && int.TryParse(yearStr, out int year))
                    {
                        book.PublicationYear = year;
                    }

                    Console.Write("Genre: ");
                    string genre = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(genre))
                    {
                        book.Genre = genre;
                    }

                    Console.Write("Total Quantity: ");
                    string quantityStr = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(quantityStr) && int.TryParse(quantityStr, out int quantity) && quantity >= book.TotalQuantity - book.AvailableQuantity)
                    {
                        int difference = quantity - book.TotalQuantity;
                        book.TotalQuantity = quantity;
                        book.AvailableQuantity += difference;
                    }
                    else if (!string.IsNullOrWhiteSpace(quantityStr))
                    {
                        Console.WriteLine("\nInvalid quantity. Cannot set quantity less than number of books currently borrowed.");
                    }

                    try
                    {
                        _libraryService.UpdateBook(book);
                        Console.WriteLine("\nBook updated successfully!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\nError updating book: {ex.Message}");
                    }
                }
            }
            else
            {
                Console.WriteLine("\nInvalid ID.");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void DeleteBook()
        {
            Console.WriteLine("==== Delete Book ====");
            Console.Write("Enter Book ID to delete: ");

            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var book = _libraryService.GetBookById(id);

                if (book == null)
                {
                    Console.WriteLine("\nBook not found.");
                }
                else
                {
                    Console.WriteLine("\nBook Details:");
                    Console.WriteLine($"Title: {book.Title}");
                    Console.WriteLine($"Author: {book.Author}");
                    Console.WriteLine($"ISBN: {book.ISBN}");

                    Console.Write("\nAre you sure you want to delete this book? (Y/N): ");
                    string confirmation = Console.ReadLine()?.ToUpper() ?? "N";

                    if (confirmation == "Y")
                    {
                        try
                        {
                            _libraryService.DeleteBook(id);
                            Console.WriteLine("\nBook deleted successfully!");
                        }
                        catch (InvalidOperationException ex)
                        {
                            Console.WriteLine($"\nCannot delete book: {ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"\nError deleting book: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nDeletion canceled.");
                    }
                }
            }
            else
            {
                Console.WriteLine("\nInvalid ID.");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void BorrowBook()
        {
            Console.WriteLine("==== Borrow Book ====");
            Console.Write("Enter Book ID: ");

            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var book = _libraryService.GetBookById(id);

                if (book == null)
                {
                    Console.WriteLine("\nBook not found.");
                }
                else if (book.AvailableQuantity <= 0)
                {
                    Console.WriteLine("\nNo copies of this book are available for borrowing.");
                }
                else
                {
                    Console.WriteLine("\nBook Details:");
                    Console.WriteLine($"Title: {book.Title}");
                    Console.WriteLine($"Author: {book.Author}");
                    Console.WriteLine($"Available Copies: {book.AvailableQuantity}/{book.TotalQuantity}");

                    Console.WriteLine("\nBorrower Information:");
                    Console.Write("Name: ");
                    string borrowerName = Console.ReadLine() ?? string.Empty;

                    Console.Write("Email: ");
                    string borrowerEmail = Console.ReadLine() ?? string.Empty;

                    Console.Write("Loan Period (days, default 14): ");
                    int loanDays = 14;
                    if (int.TryParse(Console.ReadLine(), out int days) && days > 0)
                    {
                        loanDays = days;
                    }

                    if (!string.IsNullOrWhiteSpace(borrowerName) && !string.IsNullOrWhiteSpace(borrowerEmail))
                    {
                        bool success = _libraryService.BorrowBook(id, borrowerName, borrowerEmail, loanDays);

                        if (success)
                        {
                            Console.WriteLine($"\nBook borrowed successfully! Due back in {loanDays} days.");
                        }
                        else
                        {
                            Console.WriteLine("\nFailed to borrow the book. No copies available.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nBorrower name and email are required.");
                    }
                }
            }
            else
            {
                Console.WriteLine("\nInvalid ID.");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void ReturnBook()
        {
            Console.WriteLine("==== Return Book ====");
            Console.Write("Enter Book ID: ");

            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var book = _libraryService.GetBookById(id);

                if (book == null)
                {
                    Console.WriteLine("\nBook not found.");
                }
                else
                {
                    Console.WriteLine("\nBook Details:");
                    Console.WriteLine($"Title: {book.Title}");
                    Console.WriteLine($"Author: {book.Author}");

                    Console.Write("\nBorrower Email: ");
                    string borrowerEmail = Console.ReadLine() ?? string.Empty;

                    if (!string.IsNullOrWhiteSpace(borrowerEmail))
                    {
                        bool success = _libraryService.ReturnBook(id, borrowerEmail);

                        if (success)
                        {
                            Console.WriteLine("\nBook returned successfully!");
                        }
                        else
                        {
                            Console.WriteLine("\nFailed to return the book. No active loans found for this book and borrower.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nBorrower email is required.");
                    }
                }
            }
            else
            {
                Console.WriteLine("\nInvalid ID.");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void ViewActiveLoans()
        {
            var loans = _libraryService.GetActiveLoans();

            Console.WriteLine("==== Active Loans ====");

            if (loans.Count == 0)
            {
                Console.WriteLine("No active loans found.");
            }
            else
            {
                Console.WriteLine("\n{0,-5} {1,-30} {2,-20} {3,-15} {4,-12} {5,-12}",
                    "ID", "Book Title", "Borrower", "Email", "Borrow Date", "Due Date");
                Console.WriteLine(new string('-', 100));

                foreach (var loan in loans)
                {
                    var book = _libraryService.GetBookById(loan.BookId);
                    string title = book?.Title ?? "Unknown Book";

                    Console.WriteLine("{0,-5} {1,-30} {2,-20} {3,-15} {4,-12} {5,-12}",
                        loan.Id,
                        title.Length > 28 ? title.Substring(0, 25) + "..." : title,
                        loan.BorrowerName.Length > 18 ? loan.BorrowerName.Substring(0, 15) + "..." : loan.BorrowerName,
                        loan.BorrowerEmail.Length > 13 ? loan.BorrowerEmail.Substring(0, 10) + "..." : loan.BorrowerEmail,
                        loan.BorrowDate.ToString("yyyy-MM-dd"),
                        loan.DueDate.ToString("yyyy-MM-dd"));
                }
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private void CheckOverdueBooks()
        {
            var overdueLoans = _notificationService.GetOverdueLoansWithDetails();

            Console.WriteLine("==== Overdue Books ====");

            if (overdueLoans.Count == 0)
            {
                Console.WriteLine("No overdue books found.");
            }
            else
            {
                Console.WriteLine("\n{0,-5} {1,-30} {2,-20} {3,-15} {4,-12} {5,-12}",
                    "ID", "Book Title", "Borrower", "Email", "Due Date", "Days Overdue");
                Console.WriteLine(new string('-', 100));

                foreach (var (loan, book, daysOverdue) in overdueLoans)
                {
                    Console.WriteLine("{0,-5} {1,-30} {2,-20} {3,-15} {4,-12} {5,-12}",
                        loan.Id,
                        book.Title.Length > 28 ? book.Title.Substring(0, 25) + "..." : book.Title,
                        loan.BorrowerName.Length > 18 ? loan.BorrowerName.Substring(0, 15) + "..." : loan.BorrowerName,
                        loan.BorrowerEmail.Length > 13 ? loan.BorrowerEmail.Substring(0, 10) + "..." : loan.BorrowerEmail,
                        loan.DueDate.ToString("yyyy-MM-dd"),
                        daysOverdue);
                }

                Console.WriteLine("\nDo you want to send notifications to borrowers with overdue books? (Y/N)");
                var input = Console.ReadLine()?.Trim().ToUpper() ?? "N";

                if (input == "Y")
                {
                    foreach (var (loan, book, _) in overdueLoans)
                    {
                        _notificationService.SendOverdueNotification(loan, book);
                        Console.WriteLine($"Notification sent to {loan.BorrowerEmail} for book '{book.Title}'");
                    }

                    Console.WriteLine("\nAll notifications sent successfully!");
                }
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
