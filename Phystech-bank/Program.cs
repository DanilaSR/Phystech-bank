using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using FinancialTracking.Application.Commands;
using FinancialTracking.Application.Commands.Decorators;
using FinancialTracking.Application.Facades;
using FinancialTracking.Application.Import;
using FinancialTracking.Application.Export;  // Добавьте эту директиву
using FinancialTracking.Domain.Entities;
using FinancialTracking.Domain.Factories;
using FinancialTracking.Application.Interfaces;
using FinancialTracking.Infrastructure.Repositories;
using FinancialTracking.Infrastructure.DependencyInjection;

namespace FinancialTracking.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddFinancialTrackingServices();
            
            var serviceProvider = services.BuildServiceProvider();

            await RunApplication(serviceProvider);
        }

        static async Task RunApplication(IServiceProvider serviceProvider)
        {
            var factory = serviceProvider.GetRequiredService<IDomainFactory>();
            var accountRepo = serviceProvider.GetRequiredService<IRepository<BankAccount>>();
            var categoryRepo = serviceProvider.GetRequiredService<IRepository<Category>>();
            var operationRepo = serviceProvider.GetRequiredService<IRepository<Operation>>();
            var analytics = serviceProvider.GetRequiredService<IAnalyticsFacade>();
            var logger = serviceProvider.GetRequiredService<ILogger>();
            var dataTransfer = serviceProvider.GetRequiredService<IDataTransferFacade>();

            Console.WriteLine("=== Финансовый учет Физтех-Банка ===");

            while (true)
            {
                Console.WriteLine("\nВыберите действие:");
                Console.WriteLine("1. Создать счет");
                Console.WriteLine("2. Создать категорию");
                Console.WriteLine("3. Добавить операцию");
                Console.WriteLine("4. Показать аналитику");
                Console.WriteLine("5. Экспорт данных");
                Console.WriteLine("6. Импорт данных");
                Console.WriteLine("7. Выход");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await CreateAccount(accountRepo, factory, logger);
                        break;
                    case "2":
                        await CreateCategory(categoryRepo, factory, logger);
                        break;
                    case "3":
                        await CreateOperation(accountRepo, categoryRepo, operationRepo, factory, logger);
                        break;
                    case "4":
                        ShowAnalytics(analytics);
                        break;
                    case "5":
                        await ExportData(dataTransfer, accountRepo, categoryRepo, operationRepo);
                        break;
                    case "6":
                        await ImportData(dataTransfer, accountRepo, categoryRepo, operationRepo);
                        break;
                    case "7":
                        return;
                }
            }
        }

        static async Task CreateAccount(IRepository<BankAccount> repo, IDomainFactory factory, ILogger logger)
        {
            Console.Write("Введите название счета: ");
            var name = Console.ReadLine();
            
            Console.Write("Введите начальный баланс: ");
            var balance = decimal.Parse(Console.ReadLine());

            var command = new CreateAccountCommand(repo, factory, name, balance);
            var decoratedCommand = new LoggingCommandDecorator<BankAccount>(command, logger);
            
            try
            {
                var account = decoratedCommand.Execute();
                Console.WriteLine($"Счет создан: {account.Name} (ID: {account.Id})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static async Task CreateCategory(IRepository<Category> repo, IDomainFactory factory, ILogger logger)
        {
            Console.Write("Введите название категории: ");
            var name = Console.ReadLine();
            
            Console.Write("Тип (1 - Доход, 2 - Расход): ");
            var typeChoice = Console.ReadLine();
            var type = typeChoice == "1" ? CategoryType.Income : CategoryType.Expense;

            var category = factory.CreateCategory(name, type);
            repo.Add(category);
            
            Console.WriteLine($"Категория создана: {category.Name} ({category.Type})");
        }

        static async Task CreateOperation(IRepository<BankAccount> accountRepo, 
                                        IRepository<Category> categoryRepo,
                                        IRepository<Operation> operationRepo,
                                        IDomainFactory factory,
                                        ILogger logger)
        {
            Console.Write("Тип операции (1 - Доход, 2 - Расход): ");
            var typeChoice = Console.ReadLine();
            var type = typeChoice == "1" ? CategoryType.Income : CategoryType.Expense;
            
            Console.Write("Сумма: ");
            var amount = decimal.Parse(Console.ReadLine());
            
            Console.Write("ID счета: ");
            var accountId = Guid.Parse(Console.ReadLine());
            
            Console.Write("ID категории: ");
            var categoryId = Guid.Parse(Console.ReadLine());

            Console.Write("Описание (необязательно): ");
            var description = Console.ReadLine();

            var operation = factory.CreateOperation(type, accountId, amount, categoryId, DateTime.Now, description);
            operationRepo.Add(operation);
            
            var account = accountRepo.GetById(accountId);
            if (account != null)
            {
                account.UpdateBalance(type == CategoryType.Income ? amount : -amount);
                accountRepo.Update(account);
            }
            
            Console.WriteLine($"Операция добавлена: {operation.Amount} ({operation.Type})");
        }

        static void ShowAnalytics(IAnalyticsFacade analytics)
        {
            var startDate = DateTime.Now.AddMonths(-1);
            var endDate = DateTime.Now;
            
            var difference = analytics.CalculateBalanceDifference(startDate, endDate);
            var grouped = analytics.GroupByCategories(startDate, endDate);
            
            Console.WriteLine($"\nАналитика за период с {startDate:dd.MM.yyyy} по {endDate:dd.MM.yyyy}:");
            Console.WriteLine($"Разница доходов и расходов: {difference:C}");
            Console.WriteLine("\nГруппировка по категориям:");
            
            foreach (var group in grouped)
            {
                Console.WriteLine($"  {group.Key}: {group.Value:C}");
            }
        }

        static async Task ExportData(IDataTransferFacade dataTransfer,
                                   IRepository<BankAccount> accountRepo,
                                   IRepository<Category> categoryRepo,
                                   IRepository<Operation> operationRepo)
        {
            Console.WriteLine("Выберите формат экспорта:");
            Console.WriteLine("1. JSON");
            Console.WriteLine("2. CSV");
            
            var formatChoice = Console.ReadLine();
            
            var accounts = accountRepo.GetAll();
            var categories = categoryRepo.GetAll();
            var operations = operationRepo.GetAll();
            
            string exportData;
            
            switch (formatChoice)
            {
                case "1":
                    exportData = dataTransfer.ExportToJson(accounts, categories, operations);
                    Console.WriteLine("\n=== JSON Export ===");
                    Console.WriteLine(exportData);
                    break;
                case "2":
                    exportData = dataTransfer.ExportToCsv(accounts, categories, operations);
                    Console.WriteLine("\n=== CSV Export ===");
                    Console.WriteLine(exportData);
                    break;
                default:
                    Console.WriteLine("Неверный выбор формата");
                    return;
            }
            
            Console.Write("\nСохранить в файл? (y/n): ");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                Console.Write("Введите путь к файлу: ");
                var filePath = Console.ReadLine();
                
                try
                {
                    dataTransfer.ExportToFile(filePath, accounts, categories, operations);
                    Console.WriteLine($"Данные успешно экспортированы в {filePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при экспорте: {ex.Message}");
                }
            }
        }

        static async Task ImportData(IDataTransferFacade dataTransfer,
                                   IRepository<BankAccount> accountRepo,
                                   IRepository<Category> categoryRepo,
                                   IRepository<Operation> operationRepo)
        {
            Console.Write("Введите путь к файлу для импорта: ");
            var filePath = Console.ReadLine();
            
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл не найден");
                return;
            }
            
            try
            {
                ImportResult result;
                var extension = Path.GetExtension(filePath).ToLower();
                
                switch (extension)
                {
                    case ".json":
                        result = dataTransfer.ImportFromJson(filePath);
                        break;
                    case ".csv":
                        result = dataTransfer.ImportFromCsv(filePath);
                        break;
                    default:
                        Console.WriteLine($"Неподдерживаемый формат файла: {extension}");
                        return;
                }
                
                if (result.Success)
                {
                    foreach (var account in result.Accounts)
                        accountRepo.Add(account);
                    
                    foreach (var category in result.Categories)
                        categoryRepo.Add(category);
                    
                    foreach (var operation in result.Operations)
                        operationRepo.Add(operation);
                    
                    Console.WriteLine(result.Message);
                }
                else
                {
                    Console.WriteLine($"Ошибка импорта: {result.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при импорте: {ex.Message}");
            }
        }
    }
}