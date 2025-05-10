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
                            CheckOverdueBooks();
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
            Console.WriteLine("\n{0,-5} {1,-30} {2,-20} {3,-10} {4,-5}", "ID", "Title", "Author", "Genre", "Available");
            Console.WriteLine(new string('-', 75));

            foreach (var book in books)
            {
                Console.WriteLine("{0,-5} {1,-30} {2,-20} {3,-10} {4,-5}/{5,-5}",
                    book.Id,
                    book.Title.Length > 28 ? book.Title.Substring(0, 25) + "..." : book.Title,
                    book.Author.Length > 18 ? book.Author.Substring(0, 15) + "..." : book.Author,
                    book.Genre,
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
