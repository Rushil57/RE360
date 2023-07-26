using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RE360.API.Migrations
{
    /// <inheritdoc />
    public partial class addaddressinsolicitorandclientaddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Time",
                table: "TenancyDetail");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "TenancyDetail",
                newName: "TenancyStartDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "TenancyEndDate",
                table: "TenancyDetail",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostCode",
                table: "SolicitorDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StreetName",
                table: "SolicitorDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StreetNumber",
                table: "SolicitorDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Suburb",
                table: "SolicitorDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "SolicitorDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StreetName",
                table: "ClientDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StreetNumber",
                table: "ClientDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Suburb",
                table: "ClientDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "ClientDetail",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenancyEndDate",
                table: "TenancyDetail");

            migrationBuilder.DropColumn(
                name: "PostCode",
                table: "SolicitorDetail");

            migrationBuilder.DropColumn(
                name: "StreetName",
                table: "SolicitorDetail");

            migrationBuilder.DropColumn(
                name: "StreetNumber",
                table: "SolicitorDetail");

            migrationBuilder.DropColumn(
                name: "Suburb",
                table: "SolicitorDetail");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "SolicitorDetail");

            migrationBuilder.DropColumn(
                name: "StreetName",
                table: "ClientDetail");

            migrationBuilder.DropColumn(
                name: "StreetNumber",
                table: "ClientDetail");

            migrationBuilder.DropColumn(
                name: "Suburb",
                table: "ClientDetail");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "ClientDetail");

            migrationBuilder.RenameColumn(
                name: "TenancyStartDate",
                table: "TenancyDetail",
                newName: "Date");

            migrationBuilder.AddColumn<string>(
                name: "Time",
                table: "TenancyDetail",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
