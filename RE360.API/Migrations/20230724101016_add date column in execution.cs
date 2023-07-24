using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RE360.API.Migrations
{
    /// <inheritdoc />
    public partial class adddatecolumninexecution : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Execution",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Execution");
        }
    }
}
