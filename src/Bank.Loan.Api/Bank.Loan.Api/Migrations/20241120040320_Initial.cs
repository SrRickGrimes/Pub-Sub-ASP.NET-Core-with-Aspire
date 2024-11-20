using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bank.Loan.Api.Migrations;

/// <inheritdoc />
public partial class Initial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "LoanEvents",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                LoanId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                EventType = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                Version = table.Column<int>(type: "int", nullable: false),
                Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                NewStatus = table.Column<int>(type: "int", nullable: true),
                Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CustomerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LoanEvents", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Loans",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                CustomerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                InterestRate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                MonthlyPayment = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Loans", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "LoanEvents");

        migrationBuilder.DropTable(
            name: "Loans");
    }
}
