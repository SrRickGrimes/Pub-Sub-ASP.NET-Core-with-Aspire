using Bank.Loan.Api.Controllers;

namespace Bank.Loan.Api.Dtos;

public record LoanResponseDto
{
    public required string Id { get; init; }
    public required string CustomerId { get; init; }
    public decimal Amount { get; init; }
    public required string Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public IEnumerable<LoanEventDto> Events { get; init; } = [];
}
