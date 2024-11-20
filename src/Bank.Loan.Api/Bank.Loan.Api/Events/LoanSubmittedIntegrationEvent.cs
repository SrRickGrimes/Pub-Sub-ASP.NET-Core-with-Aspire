namespace Bank.Loan.Api.Events;

public record LoanSubmittedIntegrationEvent
{
    public required string LoanId { get; init; }
    public required string CustomerId { get; init; }
    public decimal Amount { get; init; }
    public int Terms { get; init; }
    public DateTime SubmittedAt { get; init; }
}
