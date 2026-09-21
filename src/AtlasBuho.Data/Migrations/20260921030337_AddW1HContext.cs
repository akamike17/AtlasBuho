using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtlasBuho.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddW1HContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "W1HContexts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SpeakerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CommunityId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ResearcherId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Role = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    LexemeId = table.Column<Guid>(type: "TEXT", nullable: true),
                    PhraseId = table.Column<Guid>(type: "TEXT", nullable: true),
                    GrammarRuleId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CulturalNoteId = table.Column<Guid>(type: "TEXT", nullable: true),
                    EntityType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    RegionId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CommunityLocationId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Latitude = table.Column<double>(type: "REAL", precision: 9, scale: 6, nullable: true),
                    Longitude = table.Column<double>(type: "REAL", precision: 9, scale: 6, nullable: true),
                    LocationDescription = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    LocationType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DocumentedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PeriodEnd = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Era = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Season = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    TimeOfDay = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Purpose = table.Column<int>(type: "INTEGER", nullable: false),
                    ResearchGoal = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    PreservationAction = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    CommunityRequest = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Methodology = table.Column<int>(type: "INTEGER", nullable: false),
                    Technique = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Protocol = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Equipment = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Software = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    AudioRecordingId = table.Column<Guid>(type: "TEXT", nullable: true),
                    VideoRecordingId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SourceId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "UTC_TIMESTAMP()"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "UTC_TIMESTAMP()"),
                    IsComplete = table.Column<bool>(type: "INTEGER", nullable: false)
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
                });

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "W1HContexts");
        }
    }
}
