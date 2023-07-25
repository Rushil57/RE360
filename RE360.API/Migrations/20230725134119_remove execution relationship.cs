using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RE360.API.Migrations
{
    /// <inheritdoc />
    public partial class removeexecutionrelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SignaturesOfClient_Execution_ExecutionId",
                table: "SignaturesOfClient");

            migrationBuilder.RenameColumn(
                name: "ExecutionId",
                table: "SignaturesOfClient",
                newName: "ExecutionID");

            migrationBuilder.RenameIndex(
                name: "IX_SignaturesOfClient_ExecutionId",
                table: "SignaturesOfClient",
                newName: "IX_SignaturesOfClient_ExecutionID");

            migrationBuilder.AlterColumn<int>(
                name: "ExecutionID",
                table: "SignaturesOfClient",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "PID",
                table: "SignaturesOfClient",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_SignaturesOfClient_Execution_ExecutionID",
                table: "SignaturesOfClient",
                column: "ExecutionID",
                principalTable: "Execution",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SignaturesOfClient_Execution_ExecutionID",
                table: "SignaturesOfClient");

            migrationBuilder.DropColumn(
                name: "PID",
                table: "SignaturesOfClient");

            migrationBuilder.RenameColumn(
                name: "ExecutionID",
                table: "SignaturesOfClient",
                newName: "ExecutionId");

            migrationBuilder.RenameIndex(
                name: "IX_SignaturesOfClient_ExecutionID",
                table: "SignaturesOfClient",
                newName: "IX_SignaturesOfClient_ExecutionId");

            migrationBuilder.AlterColumn<int>(
                name: "ExecutionId",
                table: "SignaturesOfClient",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SignaturesOfClient_Execution_ExecutionId",
                table: "SignaturesOfClient",
                column: "ExecutionId",
                principalTable: "Execution",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
