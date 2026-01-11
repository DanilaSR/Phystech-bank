using System;
using Microsoft.Extensions.DependencyInjection;
using FinancialTracking.Application.Interfaces;
using FinancialTracking.Application.Facades;
using FinancialTracking.Domain.Factories;
using FinancialTracking.Domain.Entities;
using FinancialTracking.Infrastructure.Repositories;

namespace FinancialTracking.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFinancialTrackingServices(this IServiceCollection services)
        {
            // Factories
            services.AddSingleton<IDomainFactory, DomainFactory>();

            // Repositories
            services.AddSingleton<IRepository<BankAccount>, InMemoryRepository<BankAccount>>();
            services.AddSingleton<IRepository<Category>, InMemoryRepository<Category>>();
            services.AddSingleton<IRepository<Operation>, InMemoryRepository<Operation>>();

            // Facades
            services.AddSingleton<ICategoryFacade, CategoryFacade>();
            services.AddSingleton<IAnalyticsFacade, AnalyticsFacade>();
            services.AddSingleton<IDataTransferFacade, DataTransferFacade>();  // Добавьте эту строку

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