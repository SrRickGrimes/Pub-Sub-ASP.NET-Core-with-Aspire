using Bank.Loan.Api.Entities.Events;
using Microsoft.EntityFrameworkCore;
using Bank.Loan.Api.Entities;
using Bank.Loan.Api.Interfaces;
using System.Text.Json;

namespace Bank.Loan.Api.Persistence;

public class LoanDbContext(
    DbContextOptions<LoanDbContext> options,
    IServiceScopeFactory scopeFactory,
    ILogger<LoanDbContext> logger) : DbContext(options)
{
    public required DbSet<LoanEntity> Loans { get; set; }
    public required DbSet<LoanEvent> LoanEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LoanEntity>(entity =>
        {
            entity.ToTable("Loans");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.InterestRate).HasPrecision(5, 2);
            entity.Property(e => e.MonthlyPayment).HasPrecision(18, 2);
            entity.Property(e => e.Status).HasConversion<string>();
        });

        modelBuilder.Entity<LoanEvent>(entity =>
        {
            entity.ToTable("LoanEvents");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EventType).IsRequired();
            entity.Property(e => e.Timestamp).IsRequired();
            entity.Property(e => e.Version).IsRequired();

            entity.HasDiscriminator<string>("EventType")
                .HasValue<LoanSubmittedEvent>("LoanSubmitted")
                .HasValue<LoanStatusChangedEvent>("LoanStatusChanged");
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<LoanEvent>()
            .Where(e => e.State == EntityState.Added)
            .Select(e => e.Entity)
            .ToList();

        var result = await base.SaveChangesAsync(cancellationToken);

        using var scope = scopeFactory.CreateScope();
        var publisher = scope.ServiceProvider.GetRequiredService<ILoanEventPublisher>();

        foreach (var @event in entries)
        {
            if (@event is LoanSubmittedEvent submittedEvent)
            {
                try
                {
                    var loan = await Loans.FindAsync(submittedEvent.LoanId);
                    if (loan != null)
                    {
                        int? terms = null;
                        if (!string.IsNullOrEmpty(submittedEvent.Metadata))
                        {
                            try
                            {
                                var metadata = JsonSerializer.Deserialize<JsonElement>(submittedEvent.Metadata);
                                if (metadata.TryGetProperty("Terms", out JsonElement termsElement) &&
                                    termsElement.TryGetInt32(out int parsedTerms))
                                {
                                    terms = parsedTerms;
                                }
                            }
                            catch (JsonException ex)
                            {
                                logger.LogWarning(ex,
                                    "Could not parse metadata for loan {LoanId}. Metadata: {Metadata}",
                                    submittedEvent.LoanId,
                                    submittedEvent.Metadata);
                            }
                        }

                        await publisher.PublishLoanSubmittedAsync(loan, terms ?? 12); // default a 12 meses
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex,
                        "Error publishing event for loan {LoanId}",
                        submittedEvent.LoanId);
                    throw; // Importante re-throw para mantener la consistencia transaccional
                }
            }
        }

        return result;
    }
}
