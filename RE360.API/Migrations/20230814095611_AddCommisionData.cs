using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RE360.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCommisionData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstlyFee",
                table: "CalculationOfCommission");

            migrationBuilder.DropColumn(
                name: "OnTheFirst",
                table: "CalculationOfCommission");

            migrationBuilder.DropColumn(
                name: "Secondly",
                table: "CalculationOfCommission");

            migrationBuilder.DropColumn(
                name: "SecondlyTwo",
                table: "CalculationOfCommission");

            migrationBuilder.DropColumn(
                name: "Thirdly",
                table: "CalculationOfCommission");

            migrationBuilder.RenameColumn(
                name: "IsNonStandard",
                table: "CalculationOfCommission",
                newName: "IsInCaseOfLessHoldTerm");

            migrationBuilder.AddColumn<bool>(
                name: "IsCreateCustComTerm",
                table: "CalculationOfCommission",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "BaseAmount",
                table: "AspNetUsers",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagerEmail",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumCommission",
                table: "AspNetUsers",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OffinceName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SalePricePercentage",
                table: "AspNetUsers",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ClientCommissionDetails",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PID = table.Column<int>(type: "int", nullable: false),
                    Percent = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UpToAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Sequence = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientCommissionDetails", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CommissionDetails",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Percent = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UpToAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Sequence = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionDetails", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientCommissionDetails");

            migrationBuilder.DropTable(
                name: "CommissionDetails");

            migrationBuilder.DropColumn(
                name: "IsCreateCustComTerm",
                table: "CalculationOfCommission");

            migrationBuilder.DropColumn(
                name: "BaseAmount",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ManagerEmail",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MinimumCommission",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OffinceName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SalePricePercentage",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "IsInCaseOfLessHoldTerm",
                table: "CalculationOfCommission",
                newName: "IsNonStandard");

            migrationBuilder.AddColumn<decimal>(
                name: "FirstlyFee",
                table: "CalculationOfCommission",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OnTheFirst",
                table: "CalculationOfCommission",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Secondly",
                table: "CalculationOfCommission",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SecondlyTwo",
                table: "CalculationOfCommission",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Thirdly",
                table: "CalculationOfCommission",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
