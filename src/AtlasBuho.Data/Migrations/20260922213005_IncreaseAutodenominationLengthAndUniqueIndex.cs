using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtlasBuho.Data.Migrations
{
    /// <inheritdoc />
    public partial class IncreaseAutodenominationLengthAndUniqueIndex : Migration
    {
        /// <inheritdoc />
                protected override void Up(MigrationBuilder migrationBuilder)
                {
                    // First create the new unique index (includes SourcePage) so FK can use it
                    migrationBuilder.CreateIndex(
                        name: "IX_LanguageVariantAutodenominations_Variant_Autodenom_Page",
                        table: "LanguageVariantAutodenominations",
                        columns: new[] { "LanguageVariantId", "Autodenomination", "SourcePage" },
                        unique: true);

                    // Now drop the old unique index (FK can now use the new one)
                    migrationBuilder.DropIndex(
                        name: "IX_LanguageVariantAutodenominations_Variant_Autodenom",
                        table: "LanguageVariantAutodenominations");

                    migrationBuilder.AlterColumn<string>(
                        name: "SpanishName",
                        table: "LanguageVariantAutodenominations",
                        type: "varchar(500)",
                        maxLength: 500,
                        nullable: true,
                        oldClrType: typeof(string),
                        oldType: "varchar(200)",
                        oldMaxLength: 200,
                        oldNullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .OldAnnotation("MySql:CharSet", "utf8mb4");

                    migrationBuilder.AlterColumn<string>(
                        name: "Familia",
                        table: "LanguageVariantAutodenominations",
                        type: "varchar(500)",
                        maxLength: 500,
                        nullable: true,
                        oldClrType: typeof(string),
                        oldType: "varchar(200)",
                        oldMaxLength: 200,
                        oldNullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .OldAnnotation("MySql:CharSet", "utf8mb4");

                    migrationBuilder.AlterColumn<string>(
                        name: "Autodenomination",
                        table: "LanguageVariantAutodenominations",
                        type: "varchar(500)",
                        maxLength: 500,
                        nullable: false,
                        oldClrType: typeof(string),
                        oldType: "varchar(200)",
                        oldMaxLength: 200)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .OldAnnotation("MySql:CharSet", "utf8mb4");

                    migrationBuilder.AlterColumn<string>(
                        name: "Agrupacion",
                        table: "LanguageVariantAutodenominations",
                        type: "varchar(500)",
                        maxLength: 500,
                        nullable: true,
                        oldClrType: typeof(string),
                        oldType: "varchar(200)",
                        oldMaxLength: 200,
                        oldNullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .OldAnnotation("MySql:CharSet", "utf8mb4");
                }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LanguageVariantAutodenominations_Variant_Autodenom_Page",
                table: "LanguageVariantAutodenominations");

            migrationBuilder.AlterColumn<string>(
                name: "SpanishName",
                table: "LanguageVariantAutodenominations",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Familia",
                table: "LanguageVariantAutodenominations",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Autodenomination",
                table: "LanguageVariantAutodenominations",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Agrupacion",
                table: "LanguageVariantAutodenominations",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageVariantAutodenominations_Variant_Autodenom",
                table: "LanguageVariantAutodenominations",
                columns: new[] { "LanguageVariantId", "Autodenomination" },
                unique: true);
        }
    }
}
