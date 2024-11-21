using Bank.Loan.Bff.Services;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.AddRabbitMQClient(connectionName: "messaging");


// Configura MassTransit
builder.Services.AddMassTransit(x =>
{
    // Registra el consumidor
    x.AddConsumer<LoanSubmittedConsumer>(configure =>
    {
        // Configura las políticas de reintento específicas para este consumidor
        configure.UseMessageRetry(r =>
        {
            r.Intervals(100, 200, 500, 800, 1000);
        });
    });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("messaging"));

        cfg.ConfigureEndpoints(context);

        cfg.ReceiveEndpoint("loan-queue", e =>
        {
            e.ConfigureConsumer<LoanSubmittedConsumer>(context);
        });
    });
});

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

app.Run();
