using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RE360.API.Migrations
{
    /// <inheritdoc />
    public partial class modifiedclientchange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalDisclosures",
                table: "Execution");

            migrationBuilder.RenameColumn(
                name: "OfThePurchasePrice",
                table: "PriorAgencyMarketing",
                newName: "Thirdly");

            migrationBuilder.RenameColumn(
                name: "InitialFee",
                table: "PriorAgencyMarketing",
                newName: "SecondlyTwo");

            migrationBuilder.RenameColumn(
                name: "CommissionOnInitial",
                table: "PriorAgencyMarketing",
                newName: "Secondly");

            migrationBuilder.RenameColumn(
                name: "CommissionOnBalance",
                table: "PriorAgencyMarketing",
                newName: "OnTheFirst");

            migrationBuilder.RenameColumn(
                name: "AgencyTypeRemark",
                table: "MethodOfSale",
                newName: "TenderVenue");

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

            migrationBuilder.AddColumn<string>(
                name: "AgencyOtherTypeRemark",
                table: "MethodOfSale",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAuctionOnSite",
                table: "MethodOfSale",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AdditionalDisclosures",
                table: "Estimates",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstimatedCommissionIncGST",
                table: "PriorAgencyMarketing");

            migrationBuilder.DropColumn(
                name: "FirstlyFee",
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
                name: "AgencyOtherTypeRemark",
                table: "MethodOfSale");

            migrationBuilder.DropColumn(
                name: "IsAuctionOnSite",
                table: "MethodOfSale");

            migrationBuilder.DropColumn(
                name: "AdditionalDisclosures",
                table: "Estimates");

            migrationBuilder.RenameColumn(
                name: "Thirdly",
                table: "PriorAgencyMarketing",
                newName: "OfThePurchasePrice");

            migrationBuilder.RenameColumn(
                name: "SecondlyTwo",
                table: "PriorAgencyMarketing",
                newName: "InitialFee");

            migrationBuilder.RenameColumn(
                name: "Secondly",
                table: "PriorAgencyMarketing",
                newName: "CommissionOnInitial");

            migrationBuilder.RenameColumn(
                name: "OnTheFirst",
                table: "PriorAgencyMarketing",
                newName: "CommissionOnBalance");

            migrationBuilder.RenameColumn(
                name: "TenderVenue",
                table: "MethodOfSale",
                newName: "AgencyTypeRemark");

            migrationBuilder.AddColumn<string>(
                name: "AdditionalDisclosures",
                table: "Execution",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
