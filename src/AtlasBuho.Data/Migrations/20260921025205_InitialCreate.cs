using System;
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
            migrationBuilder.CreateTable(
                name: "LanguageFamilies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    NameEnglish = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageFamilies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Country = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: true),
                    Level = table.Column<int>(type: "INTEGER", nullable: false),
                    Institution = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Url = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Doi = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Isbn = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Issn = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    PublicationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Authors = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Editors = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Publisher = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Location = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Language = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LanguageGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LanguageFamilyId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    NameEnglish = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "LanguageVariants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LanguageGroupId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Autodenomination = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Iso639_3Code = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    InaliCode = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    WritingSystem = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Orthography = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "Communities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LanguageVariantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RegionId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Autodenomination = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    State = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Municipality = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Locality = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Latitude = table.Column<double>(type: "decimal(10,8)", nullable: true),
                    Longitude = table.Column<double>(type: "decimal(11,8)", nullable: true),
                    Population = table.Column<int>(type: "INTEGER", nullable: true),
                    SpeakerCount = table.Column<int>(type: "INTEGER", nullable: true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "DialectRelationships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SourceLanguageVariantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TargetLanguageVariantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RelationshipType = table.Column<int>(type: "INTEGER", nullable: false),
                    IntelligibilityScore = table.Column<double>(type: "decimal(3,2)", nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    VerificationStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    Confidence = table.Column<double>(type: "decimal(3,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "GrammarRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LanguageVariantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Subcategory = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: false),
                    Pattern = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Examples = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    VerificationStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    Confidence = table.Column<double>(type: "decimal(3,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "Lexemes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LanguageVariantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CanonicalForm = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    AlternativeForms = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Autodenomination = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SpanishMeaning = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    PartOfSpeech = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PronunciationIpa = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    PronunciationReadable = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SemanticDomain = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Register = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    RegionalNotes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Etymology = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    VerificationStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    Confidence = table.Column<double>(type: "decimal(3,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "RiskAssessments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LanguageVariantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RiskLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    Source = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Year = table.Column<int>(type: "INTEGER", nullable: true),
                    Methodology = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Population = table.Column<int>(type: "INTEGER", nullable: true),
                    Criterion = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    IsCurrent = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "Translations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SourceLanguageVariantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TargetLanguageVariantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SourceText = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: false),
                    TargetText = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: false),
                    Context = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Confidence = table.Column<double>(type: "decimal(3,2)", nullable: false),
                    VerificationStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    ModelUsed = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ModelVersion = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "WritingSystems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LanguageVariantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Script = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    OrthographyRules = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsOfficial = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "CulturalNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LanguageVariantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CommunityId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Content = table.Column<string>(type: "TEXT", maxLength: 10000, nullable: false),
                    Source = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Author = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    DateRecorded = table.Column<DateTime>(type: "TEXT", nullable: true),
                    VerificationStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    Confidence = table.Column<double>(type: "decimal(3,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "SpeakerProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LanguageVariantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CommunityId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Pseudonym = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Gender = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    EducationLevel = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Occupation = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "Examples",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LexemeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Text = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: false),
                    SpanishTranslation = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: true),
                    EnglishTranslation = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: true),
                    Context = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    VerificationStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    Confidence = table.Column<double>(type: "decimal(3,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "Meanings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LexemeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SpanishMeaning = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    EnglishMeaning = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    PartOfSpeech = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SemanticDomain = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Register = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    RegionalNotes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    VerificationStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    Confidence = table.Column<double>(type: "decimal(3,2)", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "Phrases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LanguageVariantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Text = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: false),
                    SpanishTranslation = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: true),
                    EnglishTranslation = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: true),
                    Context = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    GrammarNotes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    VerificationStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    Confidence = table.Column<double>(type: "decimal(3,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LexemeId = table.Column<Guid>(type: "TEXT", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_Phrases_Lexemes_LexemeId",
                        column: x => x.LexemeId,
                        principalTable: "Lexemes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orthographies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LanguageVariantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    WritingSystemId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Grapheme = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    IpaEquivalent = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    PositionalRules = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Allophones = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    VerificationStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    Confidence = table.Column<double>(type: "decimal(3,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "VideoRecordings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SpeakerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    LanguageVariantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CommunityId = table.Column<Guid>(type: "TEXT", nullable: true),
                    LexemeId = table.Column<Guid>(type: "TEXT", nullable: true),
                    PhraseId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ExampleId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CulturalNoteId = table.Column<Guid>(type: "TEXT", nullable: true),
                    FilePath = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    FileHash = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    FileSizeBytes = table.Column<long>(type: "INTEGER", nullable: true),
                    MimeType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DurationSeconds = table.Column<double>(type: "decimal(10,2)", nullable: true),
                    Width = table.Column<int>(type: "INTEGER", nullable: true),
                    Height = table.Column<int>(type: "INTEGER", nullable: true),
                    FrameRate = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    RecordedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Context = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Transcription = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: true),
                    Translation = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: true),
                    Annotation = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ConsentStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    ConsentSource = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    UsagePermission = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    AttributionRequirement = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    RemovalRequested = table.Column<bool>(type: "INTEGER", nullable: false),
                    CommunityRestriction = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "AudioRecordings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SpeakerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    LanguageVariantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CommunityId = table.Column<Guid>(type: "TEXT", nullable: true),
                    LexemeId = table.Column<Guid>(type: "TEXT", nullable: true),
                    PhraseId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ExampleId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CulturalNoteId = table.Column<Guid>(type: "TEXT", nullable: true),
                    FilePath = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    FileHash = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    FileSizeBytes = table.Column<long>(type: "INTEGER", nullable: true),
                    MimeType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DurationSeconds = table.Column<double>(type: "decimal(10,2)", nullable: true),
                    SampleRate = table.Column<int>(type: "INTEGER", nullable: true),
                    Channels = table.Column<int>(type: "INTEGER", nullable: true),
                    RecordedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Context = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Transcription = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: true),
                    Translation = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: true),
                    Annotation = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ConsentStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    ConsentSource = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    UsagePermission = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    AttributionRequirement = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    RemovalRequested = table.Column<bool>(type: "INTEGER", nullable: false),
                    CommunityRestriction = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "Evidence",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SourceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SourceId1 = table.Column<Guid>(type: "TEXT", nullable: true),
                    EntityType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    EntityId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Quote = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: true),
                    PageReference = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    SectionReference = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Url = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Confidence = table.Column<double>(type: "decimal(3,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                        name: "FK_Evidence_Sources_SourceId1",
                        column: x => x.SourceId1,
                        principalTable: "Sources",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Evidence_Translations_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Translations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhraseLexemes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PhraseId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LexemeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Position = table.Column<int>(type: "INTEGER", nullable: false),
                    GrammarRole = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LexemeId1 = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhraseLexemes", x => x.Id);
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
                });

            migrationBuilder.CreateTable(
                name: "ConsentRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SpeakerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AudioRecordingId = table.Column<Guid>(type: "TEXT", nullable: true),
                    VideoRecordingId = table.Column<Guid>(type: "TEXT", nullable: true),
                    LexemeId = table.Column<Guid>(type: "TEXT", nullable: true),
                    PhraseId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    GrantedBy = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    GrantedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Scope = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Restrictions = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    IsRevoked = table.Column<bool>(type: "INTEGER", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RevokedBy = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "Pronunciations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LexemeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Ipa = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Readable = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    AudioUrl = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    AudioRecordingId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SpeakerId = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    VerificationStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    Confidence = table.Column<double>(type: "decimal(3,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                });

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
                name: "IX_Evidence_SourceId1",
                table: "Evidence",
                column: "SourceId1");

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
                name: "IX_PhraseLexemes_Phrase_Position",
                table: "PhraseLexemes",
                columns: new[] { "PhraseId", "Position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhraseLexemes_PhraseId",
                table: "PhraseLexemes",
                column: "PhraseId");

            migrationBuilder.CreateIndex(
                name: "IX_Phrases_LexemeId",
                table: "Phrases",
                column: "LexemeId");

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
                name: "IX_Pronunciations_LexemeId",
                table: "Pronunciations",
                column: "LexemeId");

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
                name: "IX_Speakers_CommunityId",
                table: "SpeakerProfiles",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_Speakers_VariantId",
                table: "SpeakerProfiles",
                column: "LanguageVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Translations_Source_Target_Text",
                table: "Translations",
                columns: new[] { "SourceLanguageVariantId", "TargetLanguageVariantId", "SourceText" });

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
                name: "ConsentRecords");

            migrationBuilder.DropTable(
                name: "Evidence");

            migrationBuilder.DropTable(
                name: "Orthographies");

            migrationBuilder.DropTable(
                name: "PhraseLexemes");

            migrationBuilder.DropTable(
                name: "Pronunciations");

            migrationBuilder.DropTable(
                name: "RiskAssessments");

            migrationBuilder.DropTable(
                name: "VideoRecordings");

            migrationBuilder.DropTable(
                name: "DialectRelationships");

            migrationBuilder.DropTable(
                name: "GrammarRules");

            migrationBuilder.DropTable(
                name: "Meanings");

            migrationBuilder.DropTable(
                name: "Sources");

            migrationBuilder.DropTable(
                name: "Translations");

            migrationBuilder.DropTable(
                name: "WritingSystems");

            migrationBuilder.DropTable(
                name: "AudioRecordings");

            migrationBuilder.DropTable(
                name: "CulturalNotes");

            migrationBuilder.DropTable(
                name: "Examples");

            migrationBuilder.DropTable(
                name: "Phrases");

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
