using Bank.Loan.Contracts.Events;
using MassTransit;

namespace Bank.Loan.Bff.Services
{
    public class LoanSubmittedConsumer(
        ILogger<LoanSubmittedConsumer> logger
          ) : IConsumer<LoanSubmittedIntegrationEvent>
    {
        public Task Consume(ConsumeContext<LoanSubmittedIntegrationEvent> context)
        {
            var message = context.Message;

            logger.LogInformation("Loan application received - LoanId: {LoanId}, CustomerId: {CustomerId}, Amount: {Amount}",
                message.LoanId, message.CustomerId, message.Amount);

            try
            {
                logger.LogInformation("Loan application processed - LoanId: {LoanId}", message.LoanId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing loan {LoanId}", message.LoanId);
                throw;
            }
            return Task.CompletedTask;
        }
    }
}
