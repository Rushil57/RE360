using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RE360.API.Migrations
{
    /// <inheritdoc />
    public partial class addagentidproperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FCMToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Device = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientDetail",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PID = table.Column<int>(type: "int", nullable: false),
                    IsIndividual = table.Column<bool>(type: "bit", nullable: false),
                    IsCompanyTrust = table.Column<bool>(type: "bit", nullable: false),
                    CompanyTrustName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SurName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSameAsListingAddress = table.Column<bool>(type: "bit", nullable: false),
                    PostCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Home = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Business = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsGSTRegistered = table.Column<bool>(type: "bit", nullable: false),
                    GSTNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactPerson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientDetail", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ContractDetail",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PID = table.Column<int>(type: "int", nullable: false),
                    AuthorityStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AuthorityEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AgreedMarketSpend = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractDetail", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ContractRate",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PID = table.Column<int>(type: "int", nullable: false),
                    Water = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Council = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsPerAnnum = table.Column<bool>(type: "bit", nullable: false),
                    IsPerQuarter = table.Column<bool>(type: "bit", nullable: false),
                    OtherValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractRate", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Estimates",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PID = table.Column<int>(type: "int", nullable: false),
                    ExpensesToBeIncurred = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdditionalDisclosures = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProviderDiscountCommission = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AmountDiscountCommission = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TickHereIfEstimate = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estimates", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Execution",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PID = table.Column<int>(type: "int", nullable: false),
                    SignedOnBehalfOfTheAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SignedOnBehalfOfTheAgentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SignedOnBehalfOfTheAgentTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AgentToSignHere = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AgentToSignHereDate = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Execution", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "LegalDetail",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PID = table.Column<int>(type: "int", nullable: false),
                    TitleTypeID = table.Column<int>(type: "int", nullable: false),
                    LotNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepositedPlan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TitleIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPropertyUnitTitle = table.Column<bool>(type: "bit", nullable: false),
                    RegisteredOwner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdditionalDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LandValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ImprovementValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RateableValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RatingValuationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LandArea = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSqm = table.Column<bool>(type: "bit", nullable: false),
                    IsHectare = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegalDetail", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ListingAddress",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Suburb = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StreetNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StreetName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListingAddress", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MethodOfSale",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PID = table.Column<int>(type: "int", nullable: false),
                    AgencyTypeID = table.Column<int>(type: "int", nullable: false),
                    AgencyOtherTypeRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MethodOfSaleID = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PriceRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuctionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AuctionTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuctionVenue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Auctioneer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenderDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenderTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeadLineDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeadLineTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsMortgageeSale = table.Column<bool>(type: "bit", nullable: false),
                    IsAsIs = table.Column<bool>(type: "bit", nullable: false),
                    IsAuctionUnlessSoldPrior = table.Column<bool>(type: "bit", nullable: false),
                    IsTenderUnlessSoldPrior = table.Column<bool>(type: "bit", nullable: false),
                    IsAuctionOnSite = table.Column<bool>(type: "bit", nullable: false),
                    TenderVenue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MethodOfSale", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ParticularDetail",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PID = table.Column<int>(type: "int", nullable: false),
                    ParticularTypeID = table.Column<int>(type: "int", nullable: true),
                    Bed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ensuites = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Toilets = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LivingRooms = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StudyRooms = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dining = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Garages = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Carports = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpenParkingSpaces = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsHomeLandPackage = table.Column<bool>(type: "bit", nullable: false),
                    IsNewConstruction = table.Column<bool>(type: "bit", nullable: false),
                    AprxFloorArea = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    IsNonVerified = table.Column<bool>(type: "bit", nullable: false),
                    AprxYearBuilt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Zoning = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticularDetail", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PriorAgencyMarketing",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PID = table.Column<int>(type: "int", nullable: false),
                    AgencyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AgencyExpiredDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AgencyName1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AgencyExpiredDate1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AgencySum = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
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
                    table.PrimaryKey("PK_PriorAgencyMarketing", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PropertyAttributeType",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyAttributeType", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PropertyInformation",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PID = table.Column<int>(type: "int", nullable: false),
                    PropAttrId = table.Column<int>(type: "int", nullable: false),
                    Count = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyInformation", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PropertyInformationDetail",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PID = table.Column<int>(type: "int", nullable: false),
                    Double = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Single = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdditionalFeature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExcludedChattels = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InternalRemark = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyInformationDetail", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SolicitorDetail",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PID = table.Column<int>(type: "int", nullable: false),
                    Firm = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IndividualActing = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitorDetail", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TenancyDetail",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PID = table.Column<int>(type: "int", nullable: false),
                    IsVacant = table.Column<bool>(type: "bit", nullable: false),
                    IsTananted = table.Column<bool>(type: "bit", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Time = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenancyDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsToBeAdvised = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenancyDetail", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SignaturesOfClient",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExecutionId = table.Column<int>(type: "int", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    SignatureOfClientName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignaturesOfClient", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SignaturesOfClient_Execution_ExecutionId",
                        column: x => x.ExecutionId,
                        principalTable: "Execution",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropertyAttribute",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyAttributeTypeID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyAttribute", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PropertyAttribute_PropertyAttributeType_PropertyAttributeTypeID",
                        column: x => x.PropertyAttributeTypeID,
                        principalTable: "PropertyAttributeType",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyAttribute_PropertyAttributeTypeID",
                table: "PropertyAttribute",
                column: "PropertyAttributeTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_SignaturesOfClient_ExecutionId",
                table: "SignaturesOfClient",
                column: "ExecutionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "ClientDetail");

            migrationBuilder.DropTable(
                name: "ContractDetail");

            migrationBuilder.DropTable(
                name: "ContractRate");

            migrationBuilder.DropTable(
                name: "Estimates");

            migrationBuilder.DropTable(
                name: "LegalDetail");

            migrationBuilder.DropTable(
                name: "ListingAddress");

            migrationBuilder.DropTable(
                name: "MethodOfSale");

            migrationBuilder.DropTable(
                name: "ParticularDetail");

            migrationBuilder.DropTable(
                name: "PriorAgencyMarketing");

            migrationBuilder.DropTable(
                name: "PropertyAttribute");

            migrationBuilder.DropTable(
                name: "PropertyInformation");

            migrationBuilder.DropTable(
                name: "PropertyInformationDetail");

            migrationBuilder.DropTable(
                name: "SignaturesOfClient");

            migrationBuilder.DropTable(
                name: "SolicitorDetail");

            migrationBuilder.DropTable(
                name: "TenancyDetail");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "PropertyAttributeType");

            migrationBuilder.DropTable(
                name: "Execution");
        }
    }
}
