namespace Bank.Loan.Api.Entities.Events;

public class LoanSubmittedEvent : LoanEvent
{
    public required string CustomerId { get; set; }
    public decimal Amount { get; set; }
}
