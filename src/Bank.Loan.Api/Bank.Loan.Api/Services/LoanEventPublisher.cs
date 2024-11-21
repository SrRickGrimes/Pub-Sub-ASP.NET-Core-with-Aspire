using Bank.Loan.Api.Entities;
using Bank.Loan.Api.Interfaces;
using Bank.Loan.Contracts.Events;
using MassTransit;

namespace Bank.Loan.Api.Services;

public class LoanEventPublisher(
    IBus bus,
    ILogger<LoanEventPublisher> logger) : ILoanEventPublisher
{
    public async Task PublishLoanSubmittedAsync(LoanEntity loan, int terms)
    {
        try
        {
            await bus.Publish(new LoanSubmittedIntegrationEvent
            {
                LoanId = loan.Id,
                CustomerId = loan.CustomerId,
                Amount = loan.Amount,
                Terms = terms,
                SubmittedAt = loan.CreatedAt
            });

            logger.LogInformation(
                "Published LoanSubmitted event for loan {LoanId}",
                loan.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Error publishing LoanSubmitted event for loan {LoanId}",
                loan.Id);
            throw;
        }
    }
}
