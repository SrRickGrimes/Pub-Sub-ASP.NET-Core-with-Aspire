namespace Bank.Loan.Api.Entities;

public class LoanEntity
{
    public required string Id { get; set; }

    public required string CustomerId { get; set; }

    public decimal Amount { get; set; }

    public decimal InterestRate { get; set; }

    public decimal MonthlyPayment { get; set; }

    public LoanStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
