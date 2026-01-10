using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using FinancialTracking.Application.Commands;
using FinancialTracking.Application.Commands.Decorators;
using FinancialTracking.Application.Facades;
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

            Console.WriteLine("=== Финансовый учет Физтех-Банка ===");

            while (true)
            {
                Console.WriteLine("\nВыберите действие:");
                Console.WriteLine("1. Создать счет");
                Console.WriteLine("2. Создать категорию");
                Console.WriteLine("3. Добавить операцию");
                Console.WriteLine("4. Показать аналитику");
                Console.WriteLine("5. Выход");

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

            // Исправленная строка - добавлен description
            var operation = factory.CreateOperation(type, accountId, amount, categoryId, DateTime.Now, description);
            operationRepo.Add(operation);
            
            // Update account balance
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
    }
}