using Bank.Loan.Api.Interfaces;
using Bank.Loan.Api.Persistence;
using Bank.Loan.Api.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.AddSqlServerDbContext<LoanDbContext>("sqldb");

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.UsingInMemory((context, configurator) =>
    {
        configurator.UseMessageRetry(
            r => r.Interval(3, TimeSpan.FromSeconds(5)));
        configurator.UseInMemoryOutbox(context);
        configurator.ConfigureEndpoints(context);
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
