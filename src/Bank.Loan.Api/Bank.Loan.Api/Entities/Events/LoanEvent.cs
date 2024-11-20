namespace Bank.Loan.Api.Entities.Events;

public abstract class LoanEvent
{
    public required string Id { get; set; }
    public required string LoanId { get; set; }
    public required string EventType { get; set; }
    public DateTime Timestamp { get; set; }
    public int Version { get; set; }
    public string? Metadata { get; set; }
}
