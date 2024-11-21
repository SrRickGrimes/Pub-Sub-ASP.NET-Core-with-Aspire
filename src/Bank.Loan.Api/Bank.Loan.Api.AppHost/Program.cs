var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql");
var sqldb = sql.AddDatabase("sqldb");
var rabbitmq = builder.AddRabbitMQ("messaging");

builder.AddProject<Projects.Bank_Loan_Api>("bank-loan-api")
    .WithReference(sqldb)
    .WithReference(rabbitmq);

builder.AddProject<Projects.Bank_Loan_Bff>("bank-loan-bff")
    .WithReference(rabbitmq);

builder.Build().Run();
