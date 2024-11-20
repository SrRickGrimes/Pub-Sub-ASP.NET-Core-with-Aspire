namespace Bank.Loan.Api.Dtos;

public record CreateLoanResponse
{
    public required string LoanId { get; init; }
    public required string Status { get; init; }
    public DateTime SubmittedAt { get; init; }
}
