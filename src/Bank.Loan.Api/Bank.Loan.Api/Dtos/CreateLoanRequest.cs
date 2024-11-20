namespace Bank.Loan.Api.Dtos;

public record CreateLoanRequest
{
    public required string CustomerId { get; init; }
    public decimal Amount { get; init; }
    public int Terms { get; init; } // month
}
