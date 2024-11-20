namespace Bank.Loan.Api.Dtos;

public record LoanEventDto
{
    public required string EventType { get; init; }
    public DateTime Timestamp { get; init; }
    public string? Metadata { get; init; }
}
