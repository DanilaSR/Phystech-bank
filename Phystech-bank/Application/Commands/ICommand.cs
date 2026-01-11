namespace FinancialTracking.Application.Commands
{
    public interface ICommand
    {
        void Execute();
    }

    public interface ICommand<TResult>
    {
        TResult Execute();
    }
}