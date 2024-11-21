using Bank.Loan.Api.Interfaces;
using Bank.Loan.Api.Persistence;
using Bank.Loan.Api.Services;
using Bank.Loan.Contracts.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.AddSqlServerDbContext<LoanDbContext>("sqldb");
builder.AddRabbitMQClient(connectionName: "messaging");

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("messaging"));
        cfg.Message<LoanSubmittedIntegrationEvent>(e =>
        {
            e.SetEntityName("loan-queue");
        });
        cfg.UseMessageRetry(r => r.Intervals(100, 200, 500, 800, 1000));
        cfg.UseJsonSerializer();
    });
});

builder.Services.AddScoped<ILoanEventPublisher, LoanEventPublisher>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<LoanDbContext>();
dbContext.Database.Migrate();

app.Run();
