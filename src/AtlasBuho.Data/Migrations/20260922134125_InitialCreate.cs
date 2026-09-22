using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtlasBuho.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CatalogSources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BaseUrl = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogSources", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LanguageFamilies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NameEnglish = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Source = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageFamilies", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Country = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Source = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SourceDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Title = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Url = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HashSha256 = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ContentType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    RetrievedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TotalPages = table.Column<int>(type: "int", nullable: false),
                    CatalogRange = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ParserVersion = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceDocuments", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Sources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Institution = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Url = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Doi = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Isbn = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Issn = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PublicationDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Authors = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Editors = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Publisher = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Location = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Language = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sources", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CatalogVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    CatalogSourceId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    VersionNumber = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SourceDocumentHash = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RetrievedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ImportedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FamilyCount = table.Column<int>(type: "int", nullable: false),
                    GroupCount = table.Column<int>(type: "int", nullable: false),
                    VariantCount = table.Column<int>(type: "int", nullable: false),
                    AutodenominationCount = table.Column<int>(type: "int", nullable: false),
                    ParserVersion = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ImportStatus = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ErrorMessage = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogVersions_CatalogSources_CatalogSourceId",
                        column: x => x.CatalogSourceId,
                        principalTable: "CatalogSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LanguageGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    LanguageFamilyId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NameEnglish = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Source = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LanguageGroups_LanguageFamilies_LanguageFamilyId",
                        column: x => x.LanguageFamilyId,
                        principalTable: "LanguageFamilies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SourcePages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    SourceDocumentId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    PageNumber = table.Column<int>(type: "int", nullable: false),
                    SectionReference = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RawTextHash = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExtractedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourcePages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SourcePages_SourceDocuments_SourceDocumentId",
                        column: x => x.SourceDocumentId,
                        principalTable: "SourceDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CatalogRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    CatalogVersionId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    EntityType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdentityKey = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InaliCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Iso639_3Code = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Autodenomination = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GeoReference = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SourceDocumentId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    SourceHash = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SourceUrl = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SourcePage = table.Column<int>(type: "int", nullable: false),
                    SourceSection = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExtractionDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ParserVersion = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ParentRecordId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogRecords_CatalogRecords_ParentRecordId",
                        column: x => x.ParentRecordId,
                        principalTable: "CatalogRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CatalogRecords_CatalogVersions_CatalogVersionId",
                        column: x => x.CatalogVersionId,
                        principalTable: "CatalogVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LanguageVariants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    LanguageGroupId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Autodenomination = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Iso639_3Code = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InaliCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    WritingSystem = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Orthography = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Source = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageVariants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LanguageVariants_LanguageGroups_LanguageGroupId",
                        column: x => x.LanguageGroupId,
                        principalTable: "LanguageGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Communities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    LanguageVariantId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    RegionId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Autodenomination = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    State = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Municipality = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Locality = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Latitude = table.Column<decimal>(type: "decimal(10,8)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(11,8)", nullable: true),
                    Population = table.Column<int>(type: "int", nullable: true),
                    SpeakerCount = table.Column<int>(type: "int", nullable: true),
                    Source = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Communities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Communities_LanguageVariants_LanguageVariantId",
                        column: x => x.LanguageVariantId,
                        principalTable: "LanguageVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Communities_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DialectRelationships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    SourceLanguageVariantId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    TargetLanguageVariantId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    RelationshipType = table.Column<int>(type: "int", nullable: false),
                    IntelligibilityScore = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Source = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VerificationStatus = table.Column<int>(type: "int", nullable: false),
                    Confidence = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DialectRelationships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DialectRelationships_LanguageVariants_SourceLanguageVariantId",
                        column: x => x.SourceLanguageVariantId,
                        principalTable: "LanguageVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DialectRelationships_LanguageVariants_TargetLanguageVariantId",
                        column: x => x.TargetLanguageVariantId,
                        principalTable: "LanguageVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GrammarRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    LanguageVariantId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Category = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Subcategory = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Pattern = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Examples = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Source = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VerificationStatus = table.Column<int>(type: "int", nullable: false),
                    Confidence = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrammarRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GrammarRules_LanguageVariants_LanguageVariantId",
                        column: x => x.LanguageVariantId,
                        principalTable: "LanguageVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LanguageVariantAutodenominations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    LanguageVariantId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Autodenomination = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SpanishName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Agrupacion = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Familia = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SourcePage = table.Column<int>(type: "int", nullable: false),
                    SourceSection = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SourceDocumentId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    SourceHash = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExtractedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ParserVersion = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageVariantAutodenominations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LanguageVariantAutodenominations_LanguageVariants_LanguageVa~",
                        column: x => x.LanguageVariantId,
                        principalTable: "LanguageVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Lexemes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    LanguageVariantId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    CanonicalForm = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AlternativeForms = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Autodenomination = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SpanishMeaning = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PartOfSpeech = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PronunciationIpa = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PronunciationReadable = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SemanticDomain = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Register = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RegionalNotes = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Etymology = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Source = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VerificationStatus = table.Column<int>(type: "int", nullable: false),
                    Confidence = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lexemes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lexemes_LanguageVariants_LanguageVariantId",
                        column: x => x.LanguageVariantId,
                        principalTable: "LanguageVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Phrases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    LanguageVariantId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Text = table.Column<string>(type: "TEXT", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SpanishTranslation = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EnglishTranslation = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Context = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GrammarNotes = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Source = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VerificationStatus = table.Column<int>(type: "int", nullable: false),
                    Confidence = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phrases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Phrases_LanguageVariants_LanguageVariantId",
                        column: x => x.LanguageVariantId,
                        principalTable: "LanguageVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RiskAssessments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    LanguageVariantId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    RiskLevel = table.Column<int>(type: "int", nullable: false),
                    Source = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Year = table.Column<int>(type: "int", nullable: true),
                    Methodology = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Population = table.Column<int>(type: "int", nullable: true),
                    Criterion = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsCurrent = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiskAssessments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RiskAssessments_LanguageVariants_LanguageVariantId",
                        column: x => x.LanguageVariantId,
                        principalTable: "LanguageVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Translations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    SourceLanguageVariantId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    TargetLanguageVariantId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    SourceText = table.Column<string>(type: "TEXT", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TargetText = table.Column<string>(type: "TEXT", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Context = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Source = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Confidence = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    VerificationStatus = table.Column<int>(type: "int", nullable: false),
                    ModelUsed = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ModelVersion = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Translations_LanguageVariants_SourceLanguageVariantId",
                        column: x => x.SourceLanguageVariantId,
                        principalTable: "LanguageVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Translations_LanguageVariants_TargetLanguageVariantId",
                        column: x => x.TargetLanguageVariantId,
                        principalTable: "LanguageVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WritingSystems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    LanguageVariantId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Script = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OrthographyRules = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Source = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsOfficial = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WritingSystems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WritingSystems_LanguageVariants_LanguageVariantId",
                        column: x => x.LanguageVariantId,
                        principalTable: "LanguageVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CulturalNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    LanguageVariantId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    CommunityId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Content = table.Column<string>(type: "TEXT", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Source = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Author = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateRecorded = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    VerificationStatus = table.Column<int>(type: "int", nullable: false),
                    Confidence = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CulturalNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CulturalNotes_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CulturalNotes_LanguageVariants_LanguageVariantId",
                        column: x => x.LanguageVariantId,
                        principalTable: "LanguageVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SpeakerProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    LanguageVariantId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    CommunityId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Pseudonym = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BirthDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Gender = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EducationLevel = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Occupation = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Source = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpeakerProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpeakerProfiles_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SpeakerProfiles_LanguageVariants_LanguageVariantId",
                        column: x => x.LanguageVariantId,
                        principalTable: "LanguageVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Examples",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    LexemeId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Text = table.Column<string>(type: "TEXT", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SpanishTranslation = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EnglishTranslation = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Context = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Source = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VerificationStatus = table.Column<int>(type: "int", nullable: false),
                    Confidence = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Examples", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Examples_Lexemes_LexemeId",
                        column: x => x.LexemeId,
                        principalTable: "Lexemes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Meanings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    LexemeId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    SpanishMeaning = table.Column<string>(type: "TEXT", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EnglishMeaning = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PartOfSpeech = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SemanticDomain = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Register = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RegionalNotes = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Source = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VerificationStatus = table.Column<int>(type: "int", nullable: false),
                    Confidence = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meanings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Meanings_Lexemes_LexemeId",
                        column: x => x.LexemeId,
                        principalTable: "Lexemes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PhraseLexemes",
                columns: table => new
                {
                    PhraseId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    LexemeId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    GrammarRole = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PhraseId1 = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    LexemeId1 = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhraseLexemes", x => new { x.PhraseId, x.LexemeId, x.Position });
                    table.ForeignKey(
                        name: "FK_PhraseLexemes_Lexemes_LexemeId",
                        column: x => x.LexemeId,
                        principalTable: "Lexemes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhraseLexemes_Lexemes_LexemeId1",
                        column: x => x.LexemeId1,
                        principalTable: "Lexemes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PhraseLexemes_Phrases_PhraseId",
                        column: x => x.PhraseId,
                        principalTable: "Phrases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhraseLexemes_Phrases_PhraseId1",
                        column: x => x.PhraseId1,
                        principalTable: "Phrases",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Orthographies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    LanguageVariantId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    WritingSystemId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    Grapheme = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpaEquivalent = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PositionalRules = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Allophones = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Source = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VerificationStatus = table.Column<int>(type: "int", nullable: false),
                    Confidence = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orthographies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orthographies_LanguageVariants_LanguageVariantId",
                        column: x => x.LanguageVariantId,
                        principalTable: "LanguageVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orthographies_WritingSystems_WritingSystemId",
                        column: x => x.WritingSystemId,
                        principalTable: "WritingSystems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VideoRecordings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    SpeakerId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    LanguageVariantId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    CommunityId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    LexemeId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    PhraseId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ExampleId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    CulturalNoteId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    FilePath = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileName = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileHash = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    MimeType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DurationSeconds = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Width = table.Column<int>(type: "int", nullable: true),
                    Height = table.Column<int>(type: "int", nullable: true),
                    FrameRate = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RecordedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Context = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Transcription = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Translation = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Annotation = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Source = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConsentStatus = table.Column<int>(type: "int", nullable: false),
                    ConsentSource = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UsagePermission = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AttributionRequirement = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemovalRequested = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CommunityRestriction = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoRecordings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoRecordings_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VideoRecordings_CulturalNotes_CulturalNoteId",
                        column: x => x.CulturalNoteId,
                        principalTable: "CulturalNotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideoRecordings_LanguageVariants_LanguageVariantId",
                        column: x => x.LanguageVariantId,
                        principalTable: "LanguageVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideoRecordings_SpeakerProfiles_SpeakerId",
                        column: x => x.SpeakerId,
                        principalTable: "SpeakerProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AudioRecordings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    SpeakerId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    LanguageVariantId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    CommunityId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    LexemeId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    PhraseId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    ExampleId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    CulturalNoteId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    FilePath = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileName = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileHash = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    MimeType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DurationSeconds = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    SampleRate = table.Column<int>(type: "int", nullable: true),
                    Channels = table.Column<int>(type: "int", nullable: true),
                    RecordedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Context = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Transcription = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Translation = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Annotation = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Source = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConsentStatus = table.Column<int>(type: "int", nullable: false),
                    ConsentSource = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UsagePermission = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AttributionRequirement = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemovalRequested = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CommunityRestriction = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AudioRecordings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AudioRecordings_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AudioRecordings_CulturalNotes_CulturalNoteId",
                        column: x => x.CulturalNoteId,
                        principalTable: "CulturalNotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AudioRecordings_Examples_ExampleId",
                        column: x => x.ExampleId,
                        principalTable: "Examples",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AudioRecordings_LanguageVariants_LanguageVariantId",
                        column: x => x.LanguageVariantId,
                        principalTable: "LanguageVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AudioRecordings_Lexemes_LexemeId",
                        column: x => x.LexemeId,
                        principalTable: "Lexemes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AudioRecordings_Phrases_PhraseId",
                        column: x => x.PhraseId,
                        principalTable: "Phrases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AudioRecordings_SpeakerProfiles_SpeakerId",
                        column: x => x.SpeakerId,
                        principalTable: "SpeakerProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Evidence",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    SourceId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    EntityType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EntityId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    Quote = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PageReference = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SectionReference = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Url = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Confidence = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evidence", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evidence_CulturalNotes_EntityId",
                        column: x => x.EntityId,
                        principalTable: "CulturalNotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Evidence_DialectRelationships_EntityId",
                        column: x => x.EntityId,
                        principalTable: "DialectRelationships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Evidence_Examples_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Examples",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Evidence_GrammarRules_EntityId",
                        column: x => x.EntityId,
                        principalTable: "GrammarRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Evidence_Lexemes_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Lexemes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Evidence_Meanings_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Meanings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Evidence_Phrases_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Phrases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Evidence_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Evidence_Translations_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Translations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ConsentRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    SpeakerId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    AudioRecordingId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    VideoRecordingId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    LexemeId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    PhraseId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    GrantedBy = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GrantedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Scope = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Restrictions = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsRevoked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    RevokedBy = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsentRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsentRecords_AudioRecordings_AudioRecordingId",
                        column: x => x.AudioRecordingId,
                        principalTable: "AudioRecordings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsentRecords_Lexemes_LexemeId",
                        column: x => x.LexemeId,
                        principalTable: "Lexemes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsentRecords_Phrases_PhraseId",
                        column: x => x.PhraseId,
                        principalTable: "Phrases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsentRecords_SpeakerProfiles_SpeakerId",
                        column: x => x.SpeakerId,
                        principalTable: "SpeakerProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsentRecords_VideoRecordings_VideoRecordingId",
                        column: x => x.VideoRecordingId,
                        principalTable: "VideoRecordings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Pronunciations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    LexemeId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Ipa = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Readable = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AudioUrl = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AudioRecordingId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    SpeakerId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    Source = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VerificationStatus = table.Column<int>(type: "int", nullable: false),
                    Confidence = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pronunciations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pronunciations_AudioRecordings_AudioRecordingId",
                        column: x => x.AudioRecordingId,
                        principalTable: "AudioRecordings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Pronunciations_Lexemes_LexemeId",
                        column: x => x.LexemeId,
                        principalTable: "Lexemes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pronunciations_SpeakerProfiles_SpeakerId",
                        column: x => x.SpeakerId,
                        principalTable: "SpeakerProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "W1HContexts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    SpeakerId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    CommunityId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    ResearcherId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    Role = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LexemeId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    PhraseId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    GrammarRuleId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    CulturalNoteId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    EntityType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RegionId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    CommunityLocationId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    Latitude = table.Column<double>(type: "double", precision: 9, scale: 6, nullable: true),
                    Longitude = table.Column<double>(type: "double", precision: 9, scale: 6, nullable: true),
                    LocationDescription = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LocationType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    PeriodEnd = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Era = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Season = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TimeOfDay = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Purpose = table.Column<int>(type: "int", nullable: false),
                    ResearchGoal = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PreservationAction = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CommunityRequest = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Methodology = table.Column<int>(type: "int", nullable: false),
                    Technique = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Protocol = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Equipment = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Software = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AudioRecordingId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    VideoRecordingId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    SourceId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    IsComplete = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_W1HContexts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_W1HContexts_AudioRecordings",
                        column: x => x.AudioRecordingId,
                        principalTable: "AudioRecordings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_W1HContexts_Communities",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_W1HContexts_CulturalNotes",
                        column: x => x.CulturalNoteId,
                        principalTable: "CulturalNotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_W1HContexts_GrammarRules",
                        column: x => x.GrammarRuleId,
                        principalTable: "GrammarRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_W1HContexts_Lexemes",
                        column: x => x.LexemeId,
                        principalTable: "Lexemes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_W1HContexts_Phrases",
                        column: x => x.PhraseId,
                        principalTable: "Phrases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_W1HContexts_Regions",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_W1HContexts_Sources",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_W1HContexts_Speakers",
                        column: x => x.SpeakerId,
                        principalTable: "SpeakerProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_W1HContexts_VideoRecordings",
                        column: x => x.VideoRecordingId,
                        principalTable: "VideoRecordings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AudioRecordings_CommunityId",
                table: "AudioRecordings",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_AudioRecordings_ConsentStatus",
                table: "AudioRecordings",
                column: "ConsentStatus");

            migrationBuilder.CreateIndex(
                name: "IX_AudioRecordings_CulturalNoteId",
                table: "AudioRecordings",
                column: "CulturalNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_AudioRecordings_ExampleId",
                table: "AudioRecordings",
                column: "ExampleId");

            migrationBuilder.CreateIndex(
                name: "IX_AudioRecordings_FilePath",
                table: "AudioRecordings",
                column: "FilePath")
                .Annotation("MySql:IndexPrefixLength", new[] { 700 });

            migrationBuilder.CreateIndex(
                name: "IX_AudioRecordings_LexemeId",
                table: "AudioRecordings",
                column: "LexemeId");

            migrationBuilder.CreateIndex(
                name: "IX_AudioRecordings_PhraseId",
                table: "AudioRecordings",
                column: "PhraseId");

            migrationBuilder.CreateIndex(
                name: "IX_AudioRecordings_RemovalRequested",
                table: "AudioRecordings",
                column: "RemovalRequested");

            migrationBuilder.CreateIndex(
                name: "IX_AudioRecordings_SpeakerId",
                table: "AudioRecordings",
                column: "SpeakerId");

            migrationBuilder.CreateIndex(
                name: "IX_AudioRecordings_VariantId",
                table: "AudioRecordings",
                column: "LanguageVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogRecords_IdentityKey",
                table: "CatalogRecords",
                column: "IdentityKey");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogRecords_ParentRecordId",
                table: "CatalogRecords",
                column: "ParentRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogRecords_SourceDocument",
                table: "CatalogRecords",
                column: "SourceDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogRecords_SourcePage",
                table: "CatalogRecords",
                column: "SourcePage");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogRecords_SourceUrl",
                table: "CatalogRecords",
                column: "SourceUrl")
                .Annotation("MySql:IndexPrefixLength", new[] { 700 });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogRecords_Version_Type_Identity",
                table: "CatalogRecords",
                columns: new[] { "CatalogVersionId", "EntityType", "IdentityKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CatalogSources_BaseUrl",
                table: "CatalogSources",
                column: "BaseUrl")
                .Annotation("MySql:IndexPrefixLength", new[] { 700 });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogSources_Name",
                table: "CatalogSources",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CatalogVersions_Source_Version",
                table: "CatalogVersions",
                columns: new[] { "CatalogSourceId", "VersionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CatalogVersions_SourceHash",
                table: "CatalogVersions",
                column: "SourceDocumentHash");

            migrationBuilder.CreateIndex(
                name: "IX_Communities_RegionId",
                table: "Communities",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Communities_State",
                table: "Communities",
                column: "State");

            migrationBuilder.CreateIndex(
                name: "IX_Communities_VariantId",
                table: "Communities",
                column: "LanguageVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsentRecords_AudioRecordingId",
                table: "ConsentRecords",
                column: "AudioRecordingId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsentRecords_IsRevoked",
                table: "ConsentRecords",
                column: "IsRevoked");

            migrationBuilder.CreateIndex(
                name: "IX_ConsentRecords_LexemeId",
                table: "ConsentRecords",
                column: "LexemeId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsentRecords_PhraseId",
                table: "ConsentRecords",
                column: "PhraseId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsentRecords_SpeakerId",
                table: "ConsentRecords",
                column: "SpeakerId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsentRecords_VideoRecordingId",
                table: "ConsentRecords",
                column: "VideoRecordingId");

            migrationBuilder.CreateIndex(
                name: "IX_CulturalNotes_CommunityId",
                table: "CulturalNotes",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_CulturalNotes_Type",
                table: "CulturalNotes",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_CulturalNotes_VariantId",
                table: "CulturalNotes",
                column: "LanguageVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_DialectRelationships_Source_Target",
                table: "DialectRelationships",
                columns: new[] { "SourceLanguageVariantId", "TargetLanguageVariantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DialectRelationships_SourceVariantId",
                table: "DialectRelationships",
                column: "SourceLanguageVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_DialectRelationships_TargetVariantId",
                table: "DialectRelationships",
                column: "TargetLanguageVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Evidence_Entity",
                table: "Evidence",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_Evidence_EntityId",
                table: "Evidence",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Evidence_SourceId",
                table: "Evidence",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Evidence_Url",
                table: "Evidence",
                column: "Url")
                .Annotation("MySql:IndexPrefixLength", new[] { 700 });

            migrationBuilder.CreateIndex(
                name: "IX_Examples_LexemeId",
                table: "Examples",
                column: "LexemeId");

            migrationBuilder.CreateIndex(
                name: "IX_GrammarRules_Variant_Category",
                table: "GrammarRules",
                columns: new[] { "LanguageVariantId", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_GrammarRules_VariantId",
                table: "GrammarRules",
                column: "LanguageVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageFamilies_Name",
                table: "LanguageFamilies",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LanguageGroups_Family_Name",
                table: "LanguageGroups",
                columns: new[] { "LanguageFamilyId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LanguageGroups_FamilyId",
                table: "LanguageGroups",
                column: "LanguageFamilyId");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageVariantAutodenominations_SourceDocument",
                table: "LanguageVariantAutodenominations",
                column: "SourceDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageVariantAutodenominations_SourcePage",
                table: "LanguageVariantAutodenominations",
                column: "SourcePage");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageVariantAutodenominations_Variant_Autodenom",
                table: "LanguageVariantAutodenominations",
                columns: new[] { "LanguageVariantId", "Autodenomination" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LanguageVariants_Group_Name",
                table: "LanguageVariants",
                columns: new[] { "LanguageGroupId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LanguageVariants_GroupId",
                table: "LanguageVariants",
                column: "LanguageGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageVariants_InaliCode",
                table: "LanguageVariants",
                column: "InaliCode");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageVariants_IsoCode",
                table: "LanguageVariants",
                column: "Iso639_3Code");

            migrationBuilder.CreateIndex(
                name: "IX_Lexemes_SemanticDomain",
                table: "Lexemes",
                column: "SemanticDomain");

            migrationBuilder.CreateIndex(
                name: "IX_Lexemes_Variant_CanonicalForm",
                table: "Lexemes",
                columns: new[] { "LanguageVariantId", "CanonicalForm" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lexemes_VariantId",
                table: "Lexemes",
                column: "LanguageVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Lexemes_VerificationStatus",
                table: "Lexemes",
                column: "VerificationStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Meanings_Lexeme_Order",
                table: "Meanings",
                columns: new[] { "LexemeId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_Meanings_LexemeId",
                table: "Meanings",
                column: "LexemeId");

            migrationBuilder.CreateIndex(
                name: "IX_Orthographies_Variant_Grapheme",
                table: "Orthographies",
                columns: new[] { "LanguageVariantId", "Grapheme" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orthographies_VariantId",
                table: "Orthographies",
                column: "LanguageVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Orthographies_WritingSystemId",
                table: "Orthographies",
                column: "WritingSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_PhraseLexemes_LexemeId",
                table: "PhraseLexemes",
                column: "LexemeId");

            migrationBuilder.CreateIndex(
                name: "IX_PhraseLexemes_LexemeId1",
                table: "PhraseLexemes",
                column: "LexemeId1");

            migrationBuilder.CreateIndex(
                name: "IX_PhraseLexemes_PhraseId",
                table: "PhraseLexemes",
                column: "PhraseId");

            migrationBuilder.CreateIndex(
                name: "IX_PhraseLexemes_PhraseId1",
                table: "PhraseLexemes",
                column: "PhraseId1");

            migrationBuilder.CreateIndex(
                name: "IX_Phrases_VariantId",
                table: "Phrases",
                column: "LanguageVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Phrases_VerificationStatus",
                table: "Phrases",
                column: "VerificationStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Pronunciations_AudioRecordingId",
                table: "Pronunciations",
                column: "AudioRecordingId");

            migrationBuilder.CreateIndex(
                name: "IX_Pronunciations_AudioUrl",
                table: "Pronunciations",
                column: "AudioUrl")
                .Annotation("MySql:IndexPrefixLength", new[] { 700 });

            migrationBuilder.CreateIndex(
                name: "IX_Pronunciations_LexemeId",
                table: "Pronunciations",
                column: "LexemeId");

            migrationBuilder.CreateIndex(
                name: "IX_Pronunciations_SpeakerId",
                table: "Pronunciations",
                column: "SpeakerId");

            migrationBuilder.CreateIndex(
                name: "IX_Regions_Country",
                table: "Regions",
                column: "Country");

            migrationBuilder.CreateIndex(
                name: "IX_Regions_Name",
                table: "Regions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RiskAssessments_Level",
                table: "RiskAssessments",
                column: "RiskLevel");

            migrationBuilder.CreateIndex(
                name: "IX_RiskAssessments_Variant_Current",
                table: "RiskAssessments",
                columns: new[] { "LanguageVariantId", "IsCurrent" });

            migrationBuilder.CreateIndex(
                name: "IX_RiskAssessments_VariantId",
                table: "RiskAssessments",
                column: "LanguageVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_SourceDocuments_Hash",
                table: "SourceDocuments",
                column: "HashSha256",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SourceDocuments_Url",
                table: "SourceDocuments",
                column: "Url")
                .Annotation("MySql:IndexPrefixLength", new[] { 700 });

            migrationBuilder.CreateIndex(
                name: "IX_SourcePages_Document_Page",
                table: "SourcePages",
                columns: new[] { "SourceDocumentId", "PageNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sources_Institution",
                table: "Sources",
                column: "Institution");

            migrationBuilder.CreateIndex(
                name: "IX_Sources_Level",
                table: "Sources",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_Sources_Name",
                table: "Sources",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sources_Url",
                table: "Sources",
                column: "Url")
                .Annotation("MySql:IndexPrefixLength", new[] { 700 });

            migrationBuilder.CreateIndex(
                name: "IX_Speakers_CommunityId",
                table: "SpeakerProfiles",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_Speakers_VariantId",
                table: "SpeakerProfiles",
                column: "LanguageVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Translations_SourceVariantId",
                table: "Translations",
                column: "SourceLanguageVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Translations_TargetVariantId",
                table: "Translations",
                column: "TargetLanguageVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoRecordings_CommunityId",
                table: "VideoRecordings",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoRecordings_ConsentStatus",
                table: "VideoRecordings",
                column: "ConsentStatus");

            migrationBuilder.CreateIndex(
                name: "IX_VideoRecordings_CulturalNoteId",
                table: "VideoRecordings",
                column: "CulturalNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoRecordings_FilePath",
                table: "VideoRecordings",
                column: "FilePath")
                .Annotation("MySql:IndexPrefixLength", new[] { 700 });

            migrationBuilder.CreateIndex(
                name: "IX_VideoRecordings_RemovalRequested",
                table: "VideoRecordings",
                column: "RemovalRequested");

            migrationBuilder.CreateIndex(
                name: "IX_VideoRecordings_SpeakerId",
                table: "VideoRecordings",
                column: "SpeakerId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoRecordings_VariantId",
                table: "VideoRecordings",
                column: "LanguageVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_W1HContexts_AudioRecordingId",
                table: "W1HContexts",
                column: "AudioRecordingId");

            migrationBuilder.CreateIndex(
                name: "IX_W1HContexts_CommunityId",
                table: "W1HContexts",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_W1HContexts_CulturalNoteId",
                table: "W1HContexts",
                column: "CulturalNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_W1HContexts_DocumentedAt",
                table: "W1HContexts",
                column: "DocumentedAt");

            migrationBuilder.CreateIndex(
                name: "IX_W1HContexts_EntityReference",
                table: "W1HContexts",
                columns: new[] { "EntityType", "LexemeId", "PhraseId", "GrammarRuleId", "CulturalNoteId" });

            migrationBuilder.CreateIndex(
                name: "IX_W1HContexts_GrammarRuleId",
                table: "W1HContexts",
                column: "GrammarRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_W1HContexts_IsComplete",
                table: "W1HContexts",
                column: "IsComplete");

            migrationBuilder.CreateIndex(
                name: "IX_W1HContexts_LexemeId",
                table: "W1HContexts",
                column: "LexemeId");

            migrationBuilder.CreateIndex(
                name: "IX_W1HContexts_Methodology",
                table: "W1HContexts",
                column: "Methodology");

            migrationBuilder.CreateIndex(
                name: "IX_W1HContexts_PhraseId",
                table: "W1HContexts",
                column: "PhraseId");

            migrationBuilder.CreateIndex(
                name: "IX_W1HContexts_Purpose",
                table: "W1HContexts",
                column: "Purpose");

            migrationBuilder.CreateIndex(
                name: "IX_W1HContexts_RegionId",
                table: "W1HContexts",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_W1HContexts_SourceId",
                table: "W1HContexts",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_W1HContexts_SpeakerId",
                table: "W1HContexts",
                column: "SpeakerId");

            migrationBuilder.CreateIndex(
                name: "IX_W1HContexts_VideoRecordingId",
                table: "W1HContexts",
                column: "VideoRecordingId");

            migrationBuilder.CreateIndex(
                name: "IX_W1HContexts_WhoWhen",
                table: "W1HContexts",
                columns: new[] { "SpeakerId", "CommunityId", "DocumentedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_WritingSystems_Variant_Official",
                table: "WritingSystems",
                columns: new[] { "LanguageVariantId", "IsOfficial" });

            migrationBuilder.CreateIndex(
                name: "IX_WritingSystems_VariantId",
                table: "WritingSystems",
                column: "LanguageVariantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogRecords");

            migrationBuilder.DropTable(
                name: "ConsentRecords");

            migrationBuilder.DropTable(
                name: "Evidence");

            migrationBuilder.DropTable(
                name: "LanguageVariantAutodenominations");

            migrationBuilder.DropTable(
                name: "Orthographies");

            migrationBuilder.DropTable(
                name: "PhraseLexemes");

            migrationBuilder.DropTable(
                name: "Pronunciations");

            migrationBuilder.DropTable(
                name: "RiskAssessments");

            migrationBuilder.DropTable(
                name: "SourcePages");

            migrationBuilder.DropTable(
                name: "W1HContexts");

            migrationBuilder.DropTable(
                name: "CatalogVersions");

            migrationBuilder.DropTable(
                name: "DialectRelationships");

            migrationBuilder.DropTable(
                name: "Meanings");

            migrationBuilder.DropTable(
                name: "Translations");

            migrationBuilder.DropTable(
                name: "WritingSystems");

            migrationBuilder.DropTable(
                name: "SourceDocuments");

            migrationBuilder.DropTable(
                name: "AudioRecordings");

            migrationBuilder.DropTable(
                name: "GrammarRules");

            migrationBuilder.DropTable(
                name: "Sources");

            migrationBuilder.DropTable(
                name: "VideoRecordings");

            migrationBuilder.DropTable(
                name: "CatalogSources");

            migrationBuilder.DropTable(
                name: "Examples");

            migrationBuilder.DropTable(
                name: "Phrases");

            migrationBuilder.DropTable(
                name: "CulturalNotes");

            migrationBuilder.DropTable(
                name: "SpeakerProfiles");

            migrationBuilder.DropTable(
                name: "Lexemes");

            migrationBuilder.DropTable(
                name: "Communities");

            migrationBuilder.DropTable(
                name: "LanguageVariants");

            migrationBuilder.DropTable(
                name: "Regions");

            migrationBuilder.DropTable(
                name: "LanguageGroups");

            migrationBuilder.DropTable(
                name: "LanguageFamilies");
        }
    }
}
