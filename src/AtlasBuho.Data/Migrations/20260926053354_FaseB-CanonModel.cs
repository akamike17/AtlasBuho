using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtlasBuho.Data.Migrations
{
    /// <inheritdoc />
    public partial class FaseBCanonModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CanonAgrupaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    LanguageGroupId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    FuenteId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    EstadoVerificacion = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DatosAdicionalesJson = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanonAgrupaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanonAgrupaciones_LanguageGroups_LanguageGroupId",
                        column: x => x.LanguageGroupId,
                        principalTable: "LanguageGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CanonAgrupaciones_Sources_FuenteId",
                        column: x => x.FuenteId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CanonAuditorias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Entidad = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EntidadId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Operacion = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Usuario = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Fecha = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    AntesJson = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DespuesJson = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Motivo = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FuenteId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanonAuditorias", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CanonCorrespondenciasFuente",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    FuenteOrigenId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    EntidadOrigen = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EntidadOrigenId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    FuenteDestinoId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    EntidadDestino = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EntidadDestinoId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    TipoCorrespondencia = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FuenteEvidenciaId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    EstadoVerificacion = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observaciones = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanonCorrespondenciasFuente", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CanonFamilias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    LanguageFamilyId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    FuenteId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    EstadoVerificacion = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DatosAdicionalesJson = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanonFamilias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanonFamilias_LanguageFamilies_LanguageFamilyId",
                        column: x => x.LanguageFamilyId,
                        principalTable: "LanguageFamilies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CanonFamilias_Sources_FuenteId",
                        column: x => x.FuenteId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CanonFraseTraduccion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    FraseOrigenId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    FraseDestinoId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    FuenteId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    EstadoVerificacion = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanonFraseTraduccion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanonFraseTraduccion_Phrases_FraseDestinoId",
                        column: x => x.FraseDestinoId,
                        principalTable: "Phrases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CanonFraseTraduccion_Phrases_FraseOrigenId",
                        column: x => x.FraseOrigenId,
                        principalTable: "Phrases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CanonFraseTraduccion_Sources_FuenteId",
                        column: x => x.FuenteId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CanonImportRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    ImportBatchId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    NumeroRegistro = table.Column<int>(type: "int", nullable: false),
                    TextoOriginal = table.Column<string>(type: "TEXT", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DatosExtraidosJson = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Estado = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MotivoRechazo = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanonImportRecords", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CanonPaises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Nombre = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Codigo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EstadoVerificacion = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FuenteId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanonPaises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanonPaises_Sources_FuenteId",
                        column: x => x.FuenteId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CanonPoblaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Nombre = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NombreAlternativo = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Tipo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EstadoVerificacion = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FuenteId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Observaciones = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanonPoblaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanonPoblaciones_Sources_FuenteId",
                        column: x => x.FuenteId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CanonVariantes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    LanguageVariantId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    FuentePrincipalId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    PaginaFuentePrincipal = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EstadoVerificacion = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Observaciones = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanonVariantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanonVariantes_LanguageVariants_LanguageVariantId",
                        column: x => x.LanguageVariantId,
                        principalTable: "LanguageVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CanonVariantes_Sources_FuentePrincipalId",
                        column: x => x.FuentePrincipalId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CanonEvidenciaAgrupacion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    AgrupacionLinguisticaId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    FuenteId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Pagina = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Seccion = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TextoEvidencia = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ubicacion = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HashEvidencia = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaExtraccion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EstadoVerificacion = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observaciones = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanonEvidenciaAgrupacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanonEvidenciaAgrupacion_CanonAgrupaciones_AgrupacionLinguis~",
                        column: x => x.AgrupacionLinguisticaId,
                        principalTable: "CanonAgrupaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CanonEvidenciaAgrupacion_Sources_FuenteId",
                        column: x => x.FuenteId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CanonEvidenciaFamilia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    FamiliaLinguisticaId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    FuenteId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Pagina = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Seccion = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TextoEvidencia = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ubicacion = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HashEvidencia = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaExtraccion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EstadoVerificacion = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observaciones = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanonEvidenciaFamilia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanonEvidenciaFamilia_CanonFamilias_FamiliaLinguisticaId",
                        column: x => x.FamiliaLinguisticaId,
                        principalTable: "CanonFamilias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CanonEvidenciaFamilia_Sources_FuenteId",
                        column: x => x.FuenteId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CanonEstados",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    PaisId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Nombre = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Codigo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EstadoVerificacion = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FuenteId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanonEstados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanonEstados_CanonPaises_PaisId",
                        column: x => x.PaisId,
                        principalTable: "CanonPaises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CanonEstados_Sources_FuenteId",
                        column: x => x.FuenteId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CanonEvidenciaPoblacion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    PoblacionId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    FuenteId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Pagina = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Seccion = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TextoEvidencia = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ubicacion = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HashEvidencia = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaExtraccion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EstadoVerificacion = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observaciones = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanonEvidenciaPoblacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanonEvidenciaPoblacion_CanonPoblaciones_PoblacionId",
                        column: x => x.PoblacionId,
                        principalTable: "CanonPoblaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CanonEvidenciaPoblacion_Sources_FuenteId",
                        column: x => x.FuenteId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CanonCodigosLinguisticos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    VarianteLinguisticaId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Sistema = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Codigo = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FuenteId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    EstadoVerificacion = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Observaciones = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanonCodigosLinguisticos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanonCodigosLinguisticos_CanonVariantes_VarianteLinguisticaId",
                        column: x => x.VarianteLinguisticaId,
                        principalTable: "CanonVariantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CanonCodigosLinguisticos_Sources_FuenteId",
                        column: x => x.FuenteId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CanonEvidenciaVariante",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    VarianteLinguisticaId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    FuenteId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Pagina = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Seccion = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TextoEvidencia = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ubicacion = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HashEvidencia = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaExtraccion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EstadoVerificacion = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observaciones = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanonEvidenciaVariante", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanonEvidenciaVariante_CanonVariantes_VarianteLinguisticaId",
                        column: x => x.VarianteLinguisticaId,
                        principalTable: "CanonVariantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CanonEvidenciaVariante_Sources_FuenteId",
                        column: x => x.FuenteId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CanonReconciliationResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    CatalogRecordId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    FuenteId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    IdentificadorFuente = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NombreFuente = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VarianteLinguisticaId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    Resultado = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EvidenciaId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    Observaciones = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanonReconciliationResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanonReconciliationResults_CanonFamilias_EvidenciaId",
                        column: x => x.EvidenciaId,
                        principalTable: "CanonFamilias",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CanonReconciliationResults_CanonVariantes_VarianteLinguistic~",
                        column: x => x.VarianteLinguisticaId,
                        principalTable: "CanonVariantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CanonReconciliationResults_CatalogRecords_CatalogRecordId",
                        column: x => x.CatalogRecordId,
                        principalTable: "CatalogRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CanonReconciliationResults_Sources_FuenteId",
                        column: x => x.FuenteId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CanonVariantePoblacion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    VarianteLinguisticaId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    PoblacionId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Relacion = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EstadoVerificacion = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FuenteId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Observaciones = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanonVariantePoblacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanonVariantePoblacion_CanonPoblaciones_PoblacionId",
                        column: x => x.PoblacionId,
                        principalTable: "CanonPoblaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CanonVariantePoblacion_CanonVariantes_VarianteLinguisticaId",
                        column: x => x.VarianteLinguisticaId,
                        principalTable: "CanonVariantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CanonVariantePoblacion_Sources_FuenteId",
                        column: x => x.FuenteId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CanonMunicipios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    EstadoId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Nombre = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Codigo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EstadoVerificacion = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FuenteId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanonMunicipios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanonMunicipios_CanonEstados_EstadoId",
                        column: x => x.EstadoId,
                        principalTable: "CanonEstados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CanonMunicipios_Sources_FuenteId",
                        column: x => x.FuenteId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CanonEvidenciaCodigo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    CodigoLinguisticoId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    FuenteId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Pagina = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Seccion = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TextoEvidencia = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ubicacion = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HashEvidencia = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaExtraccion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EstadoVerificacion = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observaciones = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanonEvidenciaCodigo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanonEvidenciaCodigo_CanonCodigosLinguisticos_CodigoLinguist~",
                        column: x => x.CodigoLinguisticoId,
                        principalTable: "CanonCodigosLinguisticos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CanonEvidenciaCodigo_Sources_FuenteId",
                        column: x => x.FuenteId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CanonLocalidades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    MunicipioId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Nombre = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Codigo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Tipo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Latitud = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: true),
                    Longitud = table.Column<decimal>(type: "decimal(10,7)", precision: 10, scale: 7, nullable: true),
                    EstadoVerificacion = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FuenteId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanonLocalidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanonLocalidades_CanonMunicipios_MunicipioId",
                        column: x => x.MunicipioId,
                        principalTable: "CanonMunicipios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CanonLocalidades_Sources_FuenteId",
                        column: x => x.FuenteId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CanonEvidenciaLocalidad",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    LocalidadId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    FuenteId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Pagina = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Seccion = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TextoEvidencia = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ubicacion = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HashEvidencia = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaExtraccion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EstadoVerificacion = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observaciones = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanonEvidenciaLocalidad", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanonEvidenciaLocalidad_CanonLocalidades_LocalidadId",
                        column: x => x.LocalidadId,
                        principalTable: "CanonLocalidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CanonEvidenciaLocalidad_Sources_FuenteId",
                        column: x => x.FuenteId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CanonPoblacionLocalidad",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    PoblacionId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    LocalidadId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Relacion = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EstadoVerificacion = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FuenteId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Observaciones = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanonPoblacionLocalidad", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanonPoblacionLocalidad_CanonLocalidades_LocalidadId",
                        column: x => x.LocalidadId,
                        principalTable: "CanonLocalidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CanonPoblacionLocalidad_CanonPoblaciones_PoblacionId",
                        column: x => x.PoblacionId,
                        principalTable: "CanonPoblaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CanonPoblacionLocalidad_Sources_FuenteId",
                        column: x => x.FuenteId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CanonVarianteLocalidad",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    VarianteLinguisticaId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    LocalidadId = table.Column<Guid>(type: "CHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Relacion = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EstadoVerificacion = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FuenteId = table.Column<Guid>(type: "CHAR(36)", nullable: true, collation: "ascii_general_ci"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Observaciones = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanonVarianteLocalidad", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanonVarianteLocalidad_CanonLocalidades_LocalidadId",
                        column: x => x.LocalidadId,
                        principalTable: "CanonLocalidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CanonVarianteLocalidad_CanonVariantes_VarianteLinguisticaId",
                        column: x => x.VarianteLinguisticaId,
                        principalTable: "CanonVariantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CanonVarianteLocalidad_Sources_FuenteId",
                        column: x => x.FuenteId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CanonAgrupaciones_FuenteId",
                table: "CanonAgrupaciones",
                column: "FuenteId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonAgrupaciones_LanguageGroupId",
                table: "CanonAgrupaciones",
                column: "LanguageGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonCodigosLinguisticos_FuenteId",
                table: "CanonCodigosLinguisticos",
                column: "FuenteId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonCodigosLinguisticos_VarianteLinguisticaId_Sistema_Codigo",
                table: "CanonCodigosLinguisticos",
                columns: new[] { "VarianteLinguisticaId", "Sistema", "Codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CanonEstados_FuenteId",
                table: "CanonEstados",
                column: "FuenteId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonEstados_PaisId",
                table: "CanonEstados",
                column: "PaisId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonEvidenciaAgrupacion_AgrupacionLinguisticaId",
                table: "CanonEvidenciaAgrupacion",
                column: "AgrupacionLinguisticaId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonEvidenciaAgrupacion_FuenteId",
                table: "CanonEvidenciaAgrupacion",
                column: "FuenteId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonEvidenciaCodigo_CodigoLinguisticoId",
                table: "CanonEvidenciaCodigo",
                column: "CodigoLinguisticoId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonEvidenciaCodigo_FuenteId",
                table: "CanonEvidenciaCodigo",
                column: "FuenteId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonEvidenciaFamilia_FamiliaLinguisticaId",
                table: "CanonEvidenciaFamilia",
                column: "FamiliaLinguisticaId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonEvidenciaFamilia_FuenteId",
                table: "CanonEvidenciaFamilia",
                column: "FuenteId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonEvidenciaLocalidad_FuenteId",
                table: "CanonEvidenciaLocalidad",
                column: "FuenteId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonEvidenciaLocalidad_LocalidadId",
                table: "CanonEvidenciaLocalidad",
                column: "LocalidadId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonEvidenciaPoblacion_FuenteId",
                table: "CanonEvidenciaPoblacion",
                column: "FuenteId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonEvidenciaPoblacion_PoblacionId",
                table: "CanonEvidenciaPoblacion",
                column: "PoblacionId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonEvidenciaVariante_FuenteId",
                table: "CanonEvidenciaVariante",
                column: "FuenteId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonEvidenciaVariante_VarianteLinguisticaId",
                table: "CanonEvidenciaVariante",
                column: "VarianteLinguisticaId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonFamilias_FuenteId",
                table: "CanonFamilias",
                column: "FuenteId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonFamilias_LanguageFamilyId",
                table: "CanonFamilias",
                column: "LanguageFamilyId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonFraseTraduccion_FraseDestinoId",
                table: "CanonFraseTraduccion",
                column: "FraseDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonFraseTraduccion_FraseOrigenId",
                table: "CanonFraseTraduccion",
                column: "FraseOrigenId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonFraseTraduccion_FuenteId",
                table: "CanonFraseTraduccion",
                column: "FuenteId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonImportRecords_ImportBatchId_NumeroRegistro",
                table: "CanonImportRecords",
                columns: new[] { "ImportBatchId", "NumeroRegistro" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CanonLocalidades_FuenteId",
                table: "CanonLocalidades",
                column: "FuenteId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonLocalidades_MunicipioId",
                table: "CanonLocalidades",
                column: "MunicipioId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonMunicipios_EstadoId",
                table: "CanonMunicipios",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonMunicipios_FuenteId",
                table: "CanonMunicipios",
                column: "FuenteId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonPaises_FuenteId",
                table: "CanonPaises",
                column: "FuenteId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonPoblaciones_FuenteId",
                table: "CanonPoblaciones",
                column: "FuenteId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonPoblacionLocalidad_FuenteId",
                table: "CanonPoblacionLocalidad",
                column: "FuenteId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonPoblacionLocalidad_LocalidadId",
                table: "CanonPoblacionLocalidad",
                column: "LocalidadId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonPoblacionLocalidad_PoblacionId_LocalidadId",
                table: "CanonPoblacionLocalidad",
                columns: new[] { "PoblacionId", "LocalidadId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CanonReconciliationResults_CatalogRecordId_FuenteId",
                table: "CanonReconciliationResults",
                columns: new[] { "CatalogRecordId", "FuenteId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CanonReconciliationResults_EvidenciaId",
                table: "CanonReconciliationResults",
                column: "EvidenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonReconciliationResults_FuenteId",
                table: "CanonReconciliationResults",
                column: "FuenteId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonReconciliationResults_VarianteLinguisticaId",
                table: "CanonReconciliationResults",
                column: "VarianteLinguisticaId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonVarianteLocalidad_FuenteId",
                table: "CanonVarianteLocalidad",
                column: "FuenteId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonVarianteLocalidad_LocalidadId",
                table: "CanonVarianteLocalidad",
                column: "LocalidadId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonVarianteLocalidad_VarianteLinguisticaId_LocalidadId",
                table: "CanonVarianteLocalidad",
                columns: new[] { "VarianteLinguisticaId", "LocalidadId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CanonVariantePoblacion_FuenteId",
                table: "CanonVariantePoblacion",
                column: "FuenteId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonVariantePoblacion_PoblacionId",
                table: "CanonVariantePoblacion",
                column: "PoblacionId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonVariantePoblacion_VarianteLinguisticaId_PoblacionId",
                table: "CanonVariantePoblacion",
                columns: new[] { "VarianteLinguisticaId", "PoblacionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CanonVariantes_FuentePrincipalId",
                table: "CanonVariantes",
                column: "FuentePrincipalId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonVariantes_LanguageVariantId",
                table: "CanonVariantes",
                column: "LanguageVariantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CanonAuditorias");

            migrationBuilder.DropTable(
                name: "CanonCorrespondenciasFuente");

            migrationBuilder.DropTable(
                name: "CanonEvidenciaAgrupacion");

            migrationBuilder.DropTable(
                name: "CanonEvidenciaCodigo");

            migrationBuilder.DropTable(
                name: "CanonEvidenciaFamilia");

            migrationBuilder.DropTable(
                name: "CanonEvidenciaLocalidad");

            migrationBuilder.DropTable(
                name: "CanonEvidenciaPoblacion");

            migrationBuilder.DropTable(
                name: "CanonEvidenciaVariante");

            migrationBuilder.DropTable(
                name: "CanonFraseTraduccion");

            migrationBuilder.DropTable(
                name: "CanonImportRecords");

            migrationBuilder.DropTable(
                name: "CanonPoblacionLocalidad");

            migrationBuilder.DropTable(
                name: "CanonReconciliationResults");

            migrationBuilder.DropTable(
                name: "CanonVarianteLocalidad");

            migrationBuilder.DropTable(
                name: "CanonVariantePoblacion");

            migrationBuilder.DropTable(
                name: "CanonAgrupaciones");

            migrationBuilder.DropTable(
                name: "CanonCodigosLinguisticos");

            migrationBuilder.DropTable(
                name: "CanonFamilias");

            migrationBuilder.DropTable(
                name: "CanonLocalidades");

            migrationBuilder.DropTable(
                name: "CanonPoblaciones");

            migrationBuilder.DropTable(
                name: "CanonVariantes");

            migrationBuilder.DropTable(
                name: "CanonMunicipios");

            migrationBuilder.DropTable(
                name: "CanonEstados");

            migrationBuilder.DropTable(
                name: "CanonPaises");
        }
    }
}
