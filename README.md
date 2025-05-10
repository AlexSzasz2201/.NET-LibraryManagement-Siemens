# Library Management System Documentation

## Project Overview

The Library Management System is a .NET console application designed to help library administrators efficiently manage their book collection. This document provides detailed information about the system architecture, components, and implementation.

## System Architecture

### Multi-Layer Architecture

The application follows a clean, multi-layer architecture to enhance modularity and future scalability:

1. **Domain Layer** (Models)
   - Contains the core business entities
   - Independent of other layers
   - Represents real-world objects like books and loans

2. **Data Access Layer** (Repositories)
   - Handles data persistence and retrieval
   - Implements the repository pattern
   - Uses JSON files for storage

3. **Business Logic Layer** (Services)
   - Implements core business rules
   - Coordinates operations between repositories
   - Enforces validation and business constraints

4. **Presentation Layer** (Console UI)
   - Provides user interface
   - Handles user input and output formatting
   - Communicates with business layer

### Component Diagram

```
┌─────────────────────────┐     ┌─────────────────────────┐
│                         │     │                         │
│     Console UI Layer    │     │     Business Layer      │
│    (LibraryConsoleUI)   │────▶│   (Library Services)    │
│                         │     │                         │
└─────────────────────────┘     └──────────────┬──────────┘
                                                │
                                                ▼
┌─────────────────────────┐     ┌─────────────────────────┐
│                         │     │                         │
│     Domain Layer        │◀────│    Data Access Layer    │
│        (Models)         │     │     (Repositories)      │
│                         │     │                         │
└─────────────────────────┘     └─────────────────────────┘
```

## Implementation Details

### Domain Models

#### Book Entity
```csharp
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
    
    public bool IsAvailable => AvailableQuantity > 0;
}
```

#### BookLoan Entity
```csharp
public class BookLoan
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string BorrowerName { get; set; }
    public string BorrowerEmail { get; set; }
    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    
    public bool IsReturned => ReturnDate.HasValue;
    public bool IsOverdue => !IsReturned && DateTime.Now > DueDate;
    public int DaysOverdue => IsOverdue ? (int)(DateTime.Now - DueDate).TotalDays : 0;
}
```

#### BookNotification Entity
```csharp
public class BookNotification
{
    public int Id { get; set; }
    public int LoanId { get; set; }
    public DateTime NotificationDate { get; set; }
    public string NotificationType { get; set; }
    public string NotificationContent { get; set; }
    public bool IsSuccessful { get; set; }
}
```

### Repository Interfaces

Each entity has a corresponding repository interface that defines operations for that entity:

- **IBookRepository**: For managing Book entities
- **IBookLoanRepository**: For managing BookLoan entities
- **INotificationRepository**: For managing BookNotification entities

### JSON Data Persistence

The application uses JSON files to store data persistently:

- **books.json**: Stores all book records
- **loans.json**: Stores all loan transactions
- **notifications.json**: Stores notification history

Example of the JSON repository implementation:

```csharp
public class JsonBookRepository : IBookRepository
{
    private readonly string _filePath;
    private List<Book> _books;

    public JsonBookRepository(string filePath)
    {
        _filePath = filePath;
        LoadBooks();
    }

    private void LoadBooks()
    {
        if (File.Exists(_filePath))
        {
            var json = File.ReadAllText(_filePath);
            _books = JsonConvert.DeserializeObject<List<Book>>(json) ?? new List<Book>();
        }
        else
        {
            _books = new List<Book>();
            SaveChanges(); // Create the file
        }
    }

    // Repository methods implementation...

    public void SaveChanges()
    {
        var json = JsonConvert.SerializeObject(_books, Formatting.Indented);
        File.WriteAllText(_filePath, json);
    }
}
```

### Business Services

The business logic layer contains the following services:

#### LibraryService
Handles core library operations:
- Book management (add, update, delete)
- Search functionality
- Borrowing and returning books
- Loan tracking

Key business rules implemented:
- Cannot delete a book that has active loans
- Cannot borrow a book if no copies are available
- Updates book availability when borrowed or returned

#### NotificationService
Handles the overdue notification system:
- Identifies overdue loans
- Generates notification content
- Records notification history

### User Interface

The console-based user interface provides a menu-driven interaction model:

```
==== Library Management System ====
1. View All Books
2. Search Books
3. Add New Book
4. Update Book
5. Delete Book
6. Borrow Book
7. Return Book
8. View Active Loans
9. Check Overdue Books
0. Exit
```

The UI layer:
- Formats data for display
- Handles user input validation
- Provides clear error messages
- Ensures a consistent user experience

### Dependency Injection

The application uses Microsoft's dependency injection container to manage dependencies:

```csharp
var serviceProvider = new ServiceCollection()
    .AddSingleton<IBookRepository>(provider => 
        new JsonBookRepository(Path.Combine(dataFolder, "books.json")))
    .AddSingleton<IBookLoanRepository>(provider => 
        new JsonBookLoanRepository(Path.Combine(dataFolder, "loans.json")))
    .AddSingleton<INotificationRepository>(provider => 
        new JsonNotificationRepository(Path.Combine(dataFolder, "notifications.json")))
    .AddSingleton<ILibraryService, LibraryService>()
    .AddSingleton<NotificationService>()
    .AddSingleton<LibraryConsoleUI>()
    .BuildServiceProvider();
```

This approach:
- Decouples implementation from interfaces
- Makes testing easier
- Allows for future component replacement

## Innovative Feature: Overdue Notification System

The Overdue Notification System is an advanced feature that helps library administrators manage overdue books effectively:

### Key Components

1. **Overdue Detection**
   - Automatically identifies loans past their due date
   - Calculates days overdue for prioritization

2. **Notification Generation**
   - Creates personalized email content for borrowers
   - Includes book details, due date, and days overdue

3. **Notification Tracking**
   - Records all notifications in the system
   - Allows for follow-up and verification

### Implementation

The feature is implemented through:

1. **NotificationService**: Business logic component that:
   - Retrieves overdue loans with book details
   - Generates notification content
   - Records notification history

2. **CheckOverdueBooks UI Method**: Presentation component that:
   - Displays overdue books in a formatted table
   - Allows administrators to trigger notifications
   - Shows preview of notification content


### Error Handling

The application implements comprehensive error handling:

1. **Input Validation**
   - Validates user input for correctness
   - Provides clear error messages

2. **Business Rule Validation**
   - Enforces business constraints
   - Prevents operations that would violate integrity

3. **File I/O Exception Handling**
   - Manages file access errors
   - Ensures data integrity


