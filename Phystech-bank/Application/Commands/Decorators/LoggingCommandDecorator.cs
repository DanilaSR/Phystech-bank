using System;
using FinancialTracking.Infrastructure.DependencyInjection;

namespace FinancialTracking.Application.Commands.Decorators
{
    public class LoggingCommandDecorator<T> : ICommand<T>
    {
        private readonly ICommand<T> _decorated;
        private readonly ILogger _logger;

        public LoggingCommandDecorator(ICommand<T> decorated, ILogger logger)
        {
            _decorated = decorated;
            _logger = logger;
        }

        public T Execute()
        {
            _logger.Log($"Starting execution of {_decorated.GetType().Name}");
            var startTime = DateTime.Now;

            try
            {
                var result = _decorated.Execute();
                var executionTime = DateTime.Now - startTime;
                _logger.Log($"Command executed successfully in {executionTime.TotalMilliseconds}ms");
                return result;
            }
            catch (Exception ex)
            {
                _logger.Log($"Command failed: {ex.Message}");
                throw;
            }
        }
    }

    public class TimingCommandDecorator<T> : ICommand<T>
    {
        private readonly ICommand<T> _decorated;

        public TimingCommandDecorator(ICommand<T> decorated)
        {
            _decorated = decorated;
        }

        public T Execute()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = _decorated.Execute();
            stopwatch.Stop();
            
            Console.WriteLine($"Execution time: {stopwatch.ElapsedMilliseconds}ms");
            return result;
        }
    }
}