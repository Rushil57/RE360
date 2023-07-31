using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RE360.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCalculationOfCommission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstimatedCommission",
                table: "PriorAgencyMarketing");

            migrationBuilder.DropColumn(
                name: "EstimatedCommissionIncGST",
                table: "PriorAgencyMarketing");

            migrationBuilder.DropColumn(
                name: "FirstlyFee",
                table: "PriorAgencyMarketing");

            migrationBuilder.DropColumn(
                name: "IsAppraisedValue",
                table: "PriorAgencyMarketing");

            migrationBuilder.DropColumn(
                name: "IsFlatCommission",
                table: "PriorAgencyMarketing");

            migrationBuilder.DropColumn(
                name: "IsIncGST",
                table: "PriorAgencyMarketing");

            migrationBuilder.DropColumn(
                name: "IsNonStandard",
                table: "PriorAgencyMarketing");

            migrationBuilder.DropColumn(
                name: "IsPercentageOfTheSalePrice",
                table: "PriorAgencyMarketing");

            migrationBuilder.DropColumn(
                name: "IsPlusGST",
                table: "PriorAgencyMarketing");

            migrationBuilder.DropColumn(
                name: "IsStandard",
                table: "PriorAgencyMarketing");

            migrationBuilder.DropColumn(
                name: "IsUnAppraisedClientAskingPrice",
                table: "PriorAgencyMarketing");

            migrationBuilder.DropColumn(
                name: "OnTheFirst",
                table: "PriorAgencyMarketing");

            migrationBuilder.DropColumn(
                name: "SalePrice",
                table: "PriorAgencyMarketing");

            migrationBuilder.DropColumn(
                name: "Secondly",
                table: "PriorAgencyMarketing");

            migrationBuilder.DropColumn(
                name: "SecondlyTwo",
                table: "PriorAgencyMarketing");

            migrationBuilder.DropColumn(
                name: "Thirdly",
                table: "PriorAgencyMarketing");

            migrationBuilder.DropColumn(
                name: "WithMinimumCommission",
                table: "PriorAgencyMarketing");

            migrationBuilder.CreateTable(
                name: "CalculationOfCommission",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PID = table.Column<int>(type: "int", nullable: false),
                    IsPlusGST = table.Column<bool>(type: "bit", nullable: false),
                    IsIncGST = table.Column<bool>(type: "bit", nullable: false),
                    IsStandard = table.Column<bool>(type: "bit", nullable: false),
                    IsNonStandard = table.Column<bool>(type: "bit", nullable: false),
                    FirstlyFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Secondly = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OnTheFirst = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Thirdly = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SecondlyTwo = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    WithMinimumCommission = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EstimatedCommission = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsPercentageOfTheSalePrice = table.Column<bool>(type: "bit", nullable: false),
                    IsFlatCommission = table.Column<bool>(type: "bit", nullable: false),
                    IsAppraisedValue = table.Column<bool>(type: "bit", nullable: false),
                    IsUnAppraisedClientAskingPrice = table.Column<bool>(type: "bit", nullable: false),
                    SalePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EstimatedCommissionIncGST = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalculationOfCommission", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalculationOfCommission");

            migrationBuilder.AddColumn<decimal>(
                name: "EstimatedCommission",
                table: "PriorAgencyMarketing",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EstimatedCommissionIncGST",
                table: "PriorAgencyMarketing",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FirstlyFee",
                table: "PriorAgencyMarketing",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAppraisedValue",
                table: "PriorAgencyMarketing",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFlatCommission",
                table: "PriorAgencyMarketing",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsIncGST",
                table: "PriorAgencyMarketing",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsNonStandard",
                table: "PriorAgencyMarketing",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPercentageOfTheSalePrice",
                table: "PriorAgencyMarketing",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPlusGST",
                table: "PriorAgencyMarketing",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsStandard",
                table: "PriorAgencyMarketing",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnAppraisedClientAskingPrice",
                table: "PriorAgencyMarketing",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "OnTheFirst",
                table: "PriorAgencyMarketing",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SalePrice",
                table: "PriorAgencyMarketing",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Secondly",
                table: "PriorAgencyMarketing",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SecondlyTwo",
                table: "PriorAgencyMarketing",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Thirdly",
                table: "PriorAgencyMarketing",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WithMinimumCommission",
                table: "PriorAgencyMarketing",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
