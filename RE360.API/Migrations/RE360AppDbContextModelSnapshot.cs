﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RE360.API.Auth;

#nullable disable

namespace RE360.API.Migrations
{
    [DbContext(typeof(RE360AppDbContext))]
    partial class RE360AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("RE360.API.Auth.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Device")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FCMToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("ProfileUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("RefreshTokenExpiryTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("RE360.API.DBModels.ClientDetail", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Business")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CompanyTrustName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ContactPerson")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GSTNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Home")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsCompanyTrust")
                        .HasColumnType("bit");

                    b.Property<bool>("IsGSTRegistered")
                        .HasColumnType("bit");

                    b.Property<bool>("IsIndividual")
                        .HasColumnType("bit");

                    b.Property<bool>("IsSameAsListingAddress")
                        .HasColumnType("bit");

                    b.Property<string>("Mobile")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PID")
                        .HasColumnType("int");

                    b.Property<string>("Position")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PostCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SurName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("ClientDetail");
                });

            modelBuilder.Entity("RE360.API.DBModels.ContractDetail", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<decimal>("AgreedMarketSpend")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("AuthorityEndDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("AuthorityStartDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("PID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.ToTable("ContractDetail");
                });

            modelBuilder.Entity("RE360.API.DBModels.ContractRate", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<decimal?>("Council")
                        .HasColumnType("decimal(18,2)");

                    b.Property<bool>("IsPerAnnum")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPerQuarter")
                        .HasColumnType("bit");

                    b.Property<decimal?>("OtherValue")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("PID")
                        .HasColumnType("int");

                    b.Property<decimal?>("Water")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("ID");

                    b.ToTable("ContractRate");
                });

            modelBuilder.Entity("RE360.API.DBModels.Estimates", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("AdditionalDisclosures")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("AmountDiscountCommission")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("ExpensesToBeIncurred")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PID")
                        .HasColumnType("int");

                    b.Property<decimal?>("ProviderDiscountCommission")
                        .HasColumnType("decimal(18,2)");

                    b.Property<bool>("TickHereIfEstimate")
                        .HasColumnType("bit");

                    b.HasKey("ID");

                    b.ToTable("Estimates");
                });

            modelBuilder.Entity("RE360.API.DBModels.Execution", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("AgentToSignHere")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AgentToSignHereDate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("PID")
                        .HasColumnType("int");

                    b.Property<string>("SignedOnBehalfOfTheAgent")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("SignedOnBehalfOfTheAgentDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("SignedOnBehalfOfTheAgentTime")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Execution");
                });

            modelBuilder.Entity("RE360.API.DBModels.LegalDetail", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("AdditionalDetails")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DepositedPlan")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("ImprovementValue")
                        .HasColumnType("decimal(18,2)");

                    b.Property<bool>("IsHectare")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPropertyUnitTitle")
                        .HasColumnType("bit");

                    b.Property<bool>("IsSqm")
                        .HasColumnType("bit");

                    b.Property<string>("LandArea")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("LandValue")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("LotNo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PID")
                        .HasColumnType("int");

                    b.Property<decimal?>("RateableValue")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("RatingValuationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("RegisteredOwner")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TitleIdentifier")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TitleTypeID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.ToTable("LegalDetail");
                });

            modelBuilder.Entity("RE360.API.DBModels.ListingAddress", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("AgentID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("PostCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StreetName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StreetNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Suburb")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Unit")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("ListingAddress");
                });

            modelBuilder.Entity("RE360.API.DBModels.MethodOfSale", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("AgencyOtherTypeRemark")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("AgencyTypeID")
                        .HasColumnType("int");

                    b.Property<DateTime?>("AuctionDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("AuctionTime")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AuctionVenue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Auctioneer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DeadLineDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("DeadLineTime")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsAsIs")
                        .HasColumnType("bit");

                    b.Property<bool>("IsAuctionOnSite")
                        .HasColumnType("bit");

                    b.Property<bool>("IsAuctionUnlessSoldPrior")
                        .HasColumnType("bit");

                    b.Property<bool>("IsMortgageeSale")
                        .HasColumnType("bit");

                    b.Property<bool>("IsTenderUnlessSoldPrior")
                        .HasColumnType("bit");

                    b.Property<int>("MethodOfSaleID")
                        .HasColumnType("int");

                    b.Property<int>("PID")
                        .HasColumnType("int");

                    b.Property<decimal?>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("PriceRemark")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("TenderDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("TenderTime")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TenderVenue")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("MethodOfSale");
                });

            modelBuilder.Entity("RE360.API.DBModels.ParticularDetail", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<decimal?>("AprxFloorArea")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("AprxYearBuilt")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Bath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Bed")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Carports")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Dining")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Ensuites")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Garages")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsHomeLandPackage")
                        .HasColumnType("bit");

                    b.Property<bool>("IsNewConstruction")
                        .HasColumnType("bit");

                    b.Property<bool>("IsNonVerified")
                        .HasColumnType("bit");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("bit");

                    b.Property<string>("LivingRooms")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OpenParkingSpaces")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PID")
                        .HasColumnType("int");

                    b.Property<int?>("ParticularTypeID")
                        .HasColumnType("int");

                    b.Property<string>("StudyRooms")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Toilets")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Zoning")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("ParticularDetail");
                });

            modelBuilder.Entity("RE360.API.DBModels.PriorAgencyMarketing", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("AgencyExpiredDate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AgencyExpiredDate1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AgencyName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AgencyName1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("AgencySum")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("EstimatedCommission")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("EstimatedCommissionIncGST")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("FirstlyFee")
                        .HasColumnType("decimal(18,2)");

                    b.Property<bool>("IsAppraisedValue")
                        .HasColumnType("bit");

                    b.Property<bool>("IsFlatCommission")
                        .HasColumnType("bit");

                    b.Property<bool>("IsIncGST")
                        .HasColumnType("bit");

                    b.Property<bool>("IsNonStandard")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPercentageOfTheSalePrice")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPlusGST")
                        .HasColumnType("bit");

                    b.Property<bool>("IsStandard")
                        .HasColumnType("bit");

                    b.Property<bool>("IsUnAppraisedClientAskingPrice")
                        .HasColumnType("bit");

                    b.Property<decimal?>("OnTheFirst")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("PID")
                        .HasColumnType("int");

                    b.Property<decimal?>("SalePrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("Secondly")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("SecondlyTwo")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("Thirdly")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("WithMinimumCommission")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("ID");

                    b.ToTable("PriorAgencyMarketing");
                });

            modelBuilder.Entity("RE360.API.DBModels.PropertyAttribute", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PropertyAttributeTypeID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("PropertyAttributeTypeID");

                    b.ToTable("PropertyAttribute");
                });

            modelBuilder.Entity("RE360.API.DBModels.PropertyAttributeType", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("PropertyAttributeType");
                });

            modelBuilder.Entity("RE360.API.DBModels.PropertyInformation", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<decimal?>("Count")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("PID")
                        .HasColumnType("int");

                    b.Property<int>("PropAttrId")
                        .HasColumnType("int");

                    b.Property<string>("Remarks")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("PropertyInformation");
                });

            modelBuilder.Entity("RE360.API.DBModels.PropertyInformationDetail", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("AdditionalFeature")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Double")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ExcludedChattels")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InternalRemark")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PID")
                        .HasColumnType("int");

                    b.Property<string>("Single")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("PropertyInformationDetail");
                });

            modelBuilder.Entity("RE360.API.DBModels.SignaturesOfClient", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<int>("ClientId")
                        .HasColumnType("int");

                    b.Property<int>("ExecutionId")
                        .HasColumnType("int");

                    b.Property<string>("SignatureOfClientName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("ExecutionId");

                    b.ToTable("SignaturesOfClient");
                });

            modelBuilder.Entity("RE360.API.DBModels.SolicitorDetail", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EmailID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Firm")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IndividualActing")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PID")
                        .HasColumnType("int");

                    b.Property<string>("Phone")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("SolicitorDetail");
                });

            modelBuilder.Entity("RE360.API.DBModels.TenancyDetail", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<DateTime?>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsTananted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsToBeAdvised")
                        .HasColumnType("bit");

                    b.Property<bool>("IsVacant")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PID")
                        .HasColumnType("int");

                    b.Property<string>("Phone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TenancyDetails")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Time")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("TenancyDetail");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("RE360.API.Auth.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("RE360.API.Auth.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RE360.API.Auth.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("RE360.API.Auth.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RE360.API.DBModels.PropertyAttribute", b =>
                {
                    b.HasOne("RE360.API.DBModels.PropertyAttributeType", null)
                        .WithMany("PropertyAttribute")
                        .HasForeignKey("PropertyAttributeTypeID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RE360.API.DBModels.SignaturesOfClient", b =>
                {
                    b.HasOne("RE360.API.DBModels.Execution", null)
                        .WithMany("SignaturesOfClient")
                        .HasForeignKey("ExecutionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RE360.API.DBModels.Execution", b =>
                {
                    b.Navigation("SignaturesOfClient");
                });

            modelBuilder.Entity("RE360.API.DBModels.PropertyAttributeType", b =>
                {
                    b.Navigation("PropertyAttribute");
                });
#pragma warning restore 612, 618
        }
    }
}
