using System;
using System.IO;
using LibraryManagementSystem.Business.Services;
using LibraryManagementSystem.Data.Repositories;
using LibraryManagementSystem.Presentation;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryManagementSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            // Setup dependency injection
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

            // Run the application
            var ui = serviceProvider.GetRequiredService<LibraryConsoleUI>();
            ui.Run();
        }
    }
}
