using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtlasBuho.Data.Migrations
{
    /// <inheritdoc />
    public partial class LexicalEquivalenceModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LexicalEquivalences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    SourceLexemeId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    TargetLanguage = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TargetText = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsCanonical = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    VerificationStatus = table.Column<int>(type: "int", nullable: false),
                    SourceId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    CatalogVersionId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    RowHash = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LexicalEquivalences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LexicalEquivalences_CatalogVersions_CatalogVersionId",
                        column: x => x.CatalogVersionId,
                        principalTable: "CatalogVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LexicalEquivalences_Lexemes_SourceLexemeId",
                        column: x => x.SourceLexemeId,
                        principalTable: "Lexemes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LexicalEquivalences_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_LexicalEquivalences_CatalogVersionId",
                table: "LexicalEquivalences",
                column: "CatalogVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_LexicalEquivalences_Lookup",
                table: "LexicalEquivalences",
                columns: new[] { "SourceLexemeId", "TargetLanguage", "CatalogVersionId" });

            migrationBuilder.CreateIndex(
                name: "IX_LexicalEquivalences_RowHash",
                table: "LexicalEquivalences",
                column: "RowHash");

            migrationBuilder.CreateIndex(
                name: "IX_LexicalEquivalences_SourceId",
                table: "LexicalEquivalences",
                column: "SourceId");

            // ADR 0001 I1 — hard DB-level invariant: at most one canonical row per
            // (SourceLexemeId, TargetLanguage, CatalogVersionId). MySQL has no filtered
            // indexes; this generated column is non-null only for canonical rows, so the
            // unique index enforces I1 at persistence.
            migrationBuilder.Sql(
                "ALTER TABLE LexicalEquivalences " +
                "ADD COLUMN CanonicalKey CHAR(64) AS (" +
                "CASE WHEN IsCanonical=1 THEN CONCAT(HEX(SourceLexemeId), '|', TargetLanguage, '|', HEX(CatalogVersionId)) ELSE NULL END" +
                ") STORED NULL;");
            migrationBuilder.Sql(
                "CREATE UNIQUE INDEX UX_LexicalEquivalence_CanonicalPerTarget ON LexicalEquivalences(CanonicalKey);");

            // ADR 0001 I3 — trigger physical enforcement: completed catalog versions are
            // immutable, and their LexicalEquivalence evidence rows cannot be mutated.
            migrationBuilder.Sql(
                "DELIMITER // " +
                "CREATE TRIGGER trg_catalogversions_prevent_update_completed " +
                "BEFORE UPDATE ON CatalogVersions FOR EACH ROW " +
                "BEGIN IF OLD.ImportStatus = 'Completed' THEN " +
                "SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'CatalogVersion is immutable once Completed'; " +
                "END IF; END //" +
                "CREATE TRIGGER trg_catalogversions_prevent_delete_completed " +
                "BEFORE DELETE ON CatalogVersions FOR EACH ROW " +
                "BEGIN IF OLD.ImportStatus = 'Completed' THEN " +
                "SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'CatalogVersion is immutable once Completed'; " +
                "END IF; END //" +
                "CREATE TRIGGER trg_lexeq_block_mutation_completed " +
                "BEFORE UPDATE ON LexicalEquivalences FOR EACH ROW " +
                "BEGIN IF EXISTS (SELECT 1 FROM CatalogVersions cv WHERE cv.Id = OLD.CatalogVersionId AND cv.ImportStatus = 'Completed') THEN " +
                "SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'LexicalEquivalence evidence is immutable once its CatalogVersion is Completed'; " +
                "END IF; END //" +
                "DELIMITER ;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LexicalEquivalences");
        }
    }
}
