var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql");
var sqldb = sql.AddDatabase("sqldb");

builder.AddProject<Projects.Bank_Loan_Api>("bank-loan-api")
    .WithReference(sqldb);

builder.AddProject<Projects.Bank_Loan_Bff>("bank-loan-bff");

builder.Build().Run();
