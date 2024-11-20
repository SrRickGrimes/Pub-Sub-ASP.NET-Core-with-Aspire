namespace Bank.Loan.Api.Entities.Events;

public class LoanStatusChangedEvent : LoanEvent
{
    public LoanStatus NewStatus { get; set; }
    public string? Reason { get; set; }
}
