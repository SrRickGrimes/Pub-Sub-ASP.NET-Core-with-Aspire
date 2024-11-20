using Bank.Loan.Api.Entities.Events;
using Bank.Loan.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Bank.Loan.Api.Persistence;
using Microsoft.EntityFrameworkCore;
using Bank.Loan.Api.Dtos;

namespace Bank.Loan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly ILogger<LoansController> _logger;
    private readonly LoanDbContext _context;

    public LoansController(
        ILogger<LoansController> logger,
        LoanDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<CreateLoanResponse>> CreateLoan(CreateLoanRequest request)
    {
        try
        {
            if (request.Amount <= 0)
                return BadRequest("Amount must be greater than 0");

            if (request.Terms < 6 || request.Terms > 60)
                return BadRequest("Terms must be between 6 and 60 months");

            var result = await _context.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
            {
                var loanId = $"LOAN-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8]}".ToUpper();

                var loan = new LoanEntity
                {
                    Id = loanId,
                    CustomerId = request.CustomerId,
                    Amount = request.Amount,
                    Status = LoanStatus.Submitted,
                    CreatedAt = DateTime.UtcNow
                };

                var submittedEvent = new LoanSubmittedEvent
                {
                    Id = Guid.NewGuid().ToString(),
                    LoanId = loan.Id,
                    CustomerId = request.CustomerId,
                    Amount = request.Amount,
                    EventType = "LoanSubmitted",
                    Timestamp = DateTime.UtcNow,
                    Version = 1,
                    Metadata = JsonSerializer.Serialize(new
                    {
                        request.Terms,
                        SubmittedVia = "WebApi"
                    })
                };

                using var transaction = await _context.Database.BeginTransactionAsync();
                await _context.Loans.AddAsync(loan);
                await _context.LoanEvents.AddAsync(submittedEvent);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new CreateLoanResponse
                {
                    LoanId = loan.Id,
                    Status = loan.Status.ToString(),
                    SubmittedAt = loan.CreatedAt
                };
            });

            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating loan for customer {CustomerId}", request.CustomerId);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LoanResponseDto>>> GetLoans()
    {
        try
        {
            var loans = await _context.Loans
                .AsNoTracking()
                .Select(loan => new LoanResponseDto
                {
                    Id = loan.Id,
                    CustomerId = loan.CustomerId,
                    Amount = loan.Amount,
                    Status = loan.Status.ToString(),
                    CreatedAt = loan.CreatedAt,
                    Events = _context.LoanEvents
                        .Where(e => e.LoanId == loan.Id)
                        .OrderBy(e => e.Timestamp)
                        .Select(e => new LoanEventDto
                        {
                            EventType = e.EventType,
                            Timestamp = e.Timestamp,
                            Metadata = e.Metadata
                        })
                        .ToList()
                })
                .ToListAsync();

            return Ok(loans);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting loans");
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LoanResponseDto>> GetLoan(string id)
    {
        try
        {
            var loan = await _context.Loans
                .AsNoTracking()
                .Select(loan => new LoanResponseDto
                {
                    Id = loan.Id,
                    CustomerId = loan.CustomerId,
                    Amount = loan.Amount,
                    Status = loan.Status.ToString(),
                    CreatedAt = loan.CreatedAt,
                    Events = _context.LoanEvents
                        .Where(e => e.LoanId == loan.Id)
                        .OrderBy(e => e.Timestamp)
                        .Select(e => new LoanEventDto
                        {
                            EventType = e.EventType,
                            Timestamp = e.Timestamp,
                            Metadata = e.Metadata
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync(l => l.Id == id);

            if (loan == null)
                return NotFound($"Loan with ID {id} not found");

            return Ok(loan);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting loan {LoanId}", id);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }
}