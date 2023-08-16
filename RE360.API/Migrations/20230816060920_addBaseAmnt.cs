using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RE360.API.Migrations
{
    /// <inheritdoc />
    public partial class addBaseAmnt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BaseAmount",
                table: "CalculationOfCommission",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SalePricePercentage",
                table: "CalculationOfCommission",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SalePricePercentage",
                table: "AspNetUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseAmount",
                table: "CalculationOfCommission");

            migrationBuilder.DropColumn(
                name: "SalePricePercentage",
                table: "CalculationOfCommission");

            migrationBuilder.AlterColumn<decimal>(
                name: "SalePricePercentage",
                table: "AspNetUsers",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
