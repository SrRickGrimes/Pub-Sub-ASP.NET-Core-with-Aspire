var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Bank_Loan_Api>("bank-loan-api");

builder.AddProject<Projects.Bank_Loan_Bff>("bank-loan-bff");

builder.Build().Run();
