using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RE360.API.Migrations
{
    /// <inheritdoc />
    public partial class addestimateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalDisclosures",
                table: "Estimates");

            migrationBuilder.CreateTable(
                name: "EstimatesDetail",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PID = table.Column<int>(type: "int", nullable: false),
                    AdditionalDisclosures = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstimatesDetail", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EstimatesDetail");

            migrationBuilder.AddColumn<string>(
                name: "AdditionalDisclosures",
                table: "Estimates",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
