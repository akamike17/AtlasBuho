using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtlasBuho.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAiReviewTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AiTranslationReviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    InputText = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SourceLanguageRequested = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TargetLanguageRequested = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DetectedLanguage = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DetectedLanguageConfidence = table.Column<double>(type: "double", precision: 5, scale: 4, nullable: true),
                    AtlasBuhoV1Result = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReviewStatus = table.Column<int>(type: "int", nullable: false),
                    ProposedCorrection = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Explanation = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EvidenceNotes = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProviderName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ModelName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PromptVersion = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ResponseSchemaVersion = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValue: "v1")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LatencyMs = table.Column<int>(type: "int", nullable: true),
                    Success = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ErrorCode = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CorrelationId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    Lifecycle = table.Column<int>(type: "int", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    HumanReviewNotes = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PromotedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    V2CandidateId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiTranslationReviews", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "V2Candidates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    AiReviewId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CanonicalForm = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SpanishMeaning = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TargetLanguage = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TargetText = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SourceVariant = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OriginalConfidence = table.Column<double>(type: "double", precision: 5, scale: 4, nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PromotionEvidenceId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    IsPromoted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    PromotedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_V2Candidates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_V2Candidates_AiTranslationReviews_AiReviewId",
                        column: x => x.AiReviewId,
                        principalTable: "AiTranslationReviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AiTranslationReviews_CorrelationId",
                table: "AiTranslationReviews",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_AiTranslationReviews_CreatedAt",
                table: "AiTranslationReviews",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AiTranslationReviews_Lifecycle",
                table: "AiTranslationReviews",
                column: "Lifecycle");

            migrationBuilder.CreateIndex(
                name: "IX_AiTranslationReviews_ProviderName",
                table: "AiTranslationReviews",
                column: "ProviderName");

            migrationBuilder.CreateIndex(
                name: "IX_AiTranslationReviews_ReviewStatus",
                table: "AiTranslationReviews",
                column: "ReviewStatus");

            migrationBuilder.CreateIndex(
                name: "IX_V2Candidates_AiReviewId",
                table: "V2Candidates",
                column: "AiReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_V2Candidates_CreatedAt",
                table: "V2Candidates",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_V2Candidates_IsPromoted",
                table: "V2Candidates",
                column: "IsPromoted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "V2Candidates");

            migrationBuilder.DropTable(
                name: "AiTranslationReviews");
        }
    }
}
