// Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs
using System;
using FinancialTracking.Application.Facades;
using FinancialTracking.Application.Interfaces;
using FinancialTracking.Domain.Entities;
using FinancialTracking.Domain.Factories;
using FinancialTracking.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialTracking.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFinancialTrackingServices(this IServiceCollection services)
        {
            // Factories
            _ = services.AddSingleton<IDomainFactory, DomainFactory>();

            // Repositories
            _ = services.AddSingleton<IRepository<BankAccount>, InMemoryRepository<BankAccount>>();
            _ = services.AddSingleton<IRepository<Category>, InMemoryRepository<Category>>();
            _ = services.AddSingleton<IRepository<Operation>, InMemoryRepository<Operation>>();

            // Facades
            _ = services.AddSingleton<ICategoryFacade, CategoryFacade>();
            _ = services.AddSingleton<IAnalyticsFacade, AnalyticsFacade>();

            // Logging
            services.AddSingleton<ILogger, ConsoleLogger>();

            return services;
        }
    }

    public interface ILogger
    {
        void Log(string message);
    }

    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine($"[LOG] {DateTime.Now:HH:mm:ss}: {message}");
        }
    }
}