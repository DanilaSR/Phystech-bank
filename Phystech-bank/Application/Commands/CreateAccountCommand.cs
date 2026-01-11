using System;
using FinancialTracking.Domain.Entities;
using FinancialTracking.Domain.Factories;
using FinancialTracking.Application.Interfaces;

namespace FinancialTracking.Application.Commands
{
    public class CreateAccountCommand : ICommand<BankAccount>
    {
        private readonly IRepository<BankAccount> _repository;
        private readonly IDomainFactory _factory;
        private readonly string _name;
        private readonly decimal _initialBalance;

        public CreateAccountCommand(IRepository<BankAccount> repository, 
                                  IDomainFactory factory, 
                                  string name, 
                                  decimal initialBalance)
        {
            _repository = repository;
            _factory = factory;
            _name = name;
            _initialBalance = initialBalance;
        }

        public BankAccount Execute()
        {
            var account = _factory.CreateAccount(_name, _initialBalance);
            _repository.Add(account);
            return account;
        }
    }
}