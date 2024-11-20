using Bank.Loan.Api.Entities;

namespace Bank.Loan.Api.Interfaces;

public interface ILoanEventPublisher
{
    Task PublishLoanSubmittedAsync(LoanEntity loan, int terms);
}
