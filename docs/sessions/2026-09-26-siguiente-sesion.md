ESTADO DEL PROYECTO — AtlasBuho — "SIGUIENTE SESION"
Fecha snapshot: 2026-09-26
Branch: review/phase-1-inventario-linguistico
Último commit: 21469cd (push OK)
==========================================================

1. DÓNDE QUEDAMOS
=================
- FASE A  BD Architecture / Traceability: CLOSED
    ADR 0002 (a45f1b1) + addendum (f7f79c4, 8a62965, 0009737)
    19 PD documentados. Sin implementación de persistencia.

- FASE B  Canon Model + Migration: CLOSED
    ADR 0003 (9934641)
    24 POCOs Canon en src/AtlasBuho.Domain/Entities/Design/
    24 EF Configurations en src/AtlasBuho.Data/Configurations/Design/
    24 DbSet añadidos a AtlasBuhoDbContext
    Migration 20260926053354_FaseB-CanonModel
    Tests integración 52/52, unitarios 68/68 (commit 54c97fb)

- FASE C  Datos (extracción PDF + carga + resconciliación): CLOSED
    PDF INALI oficial SHA-256: e38e667d024784bc84cc350f44918c1d22d998284d02e38dcf92858257947311
    Extracción: 364/364 variantes, 11 familias, 68 agrupaciones, 0 duplicadas, 0 unresolved
    Tests Python extractor: 7/7 PASS
    Discrepancy ledger escrito (PDF vs appendix4, PDF vs HTML INALI)
    BD objetivo: atlasbuho_fasec
    Cargado: 11 familias, 68 grupos, 364 variantes, 364 canon, 364 fuentes
    Reconciliación one-row-one-variant: VariantesSinCanon=0, CanonSinVariante=0
    364/364 VERIFIED en BD (commit 21469cd)

2. PUNTOS / ARCHIVOS CLAVE
==========================
- catalogs/extracted/inali_pdf_canon_full.json       364 rows (fuente canónica JSON)
- catalogs/extracted/discrepancy_ledger.json         cross-check PDF vs appendix4 vs HTML
- catalogs/extracted/fase_c_bd_proof.json            evidencia SQL final Fase C
- catalogs/extracted/_inali_pdf_manifest.json        manifest SHA-256
- catalogs/extracted/pages_html/                     68 páginas cross-check 235 variantes
- catalogs/extracted/inali_html_catalog.json         HTML INALI estructurado (cross-check)
- catalogs/inali/catalogo_lenguas_indigenas.pdf      PDF oficial (preservado en repo)
- docs/audits/phase-b-pdf-extraction-364.md          auditoría extracción 364
- docs/audits/phase-c-data-load-364.md               auditoría carga BD
- docs/adr/0002-fase-a-trazabilidad-bdmd.md          ADR Fase A
- docs/adr/0002-addendum-fase-a-ajuste-pd.md         addendum PD
- docs/adr/0003-fase-b-plan-migracion.md             ADR Fase B
- tests/AtlasBuho.Tests/test_inali_pdf_extraction.py 7/7 PASS
- tests/AtlasBuho.Tests.Integration/CanonSchemaTests.cs 7/7 PASS
- tests/AtlasBuho.ImportRunner/Program.cs            --load-canon / --verify-canon

3. PENDIENTES EXPLÍCITOS (NO HECHOS)
====================================
a) Normalización geográfica: la columna geo_reference sigue como string
   literal en LanguageVariant.Source (ruta JSON: catalogs). La mapeo a
   Pais/Estado/Municipio/Localidad Canon NO fue ejecutado. Requiere
   decisión explícita por la cantidad de municipios/localidades
   (miles) y reglas de dedupe.

b) ReconciliationResultCanon NUNCA fue poblado — sólo existe el esquema.
   Cada una de las 364 variantes necesita exactamente una fila con
   Resultado ∈ {MATCH, MISSING, DUPLICATE, CONFLICT, UNRESOLVED, NOT_APPLICABLE}.

c) Cross-checks adicionales pendientes: INEGI XLSX y catálogo datos.gob.mx
   fueron identificados pero NO descargados ni procesados.

d) Correcciones del discrepancy ledger: 47 rows sólo PDF, 30 sólo appendix4,
   300 sólo PDF, 155 sólo HTML — requieren decisión humana (no corregir
   silenciosamente, según tu regla).

e) 1 autodenominación vacía (otomí del oeste del Valle del Mezquital p.150)
   — pendiente decisión: aceptar vacío documental o asignar UNKNOWN explícito
   bajo PD-A19.

f) Base sellada `atlasbuho` (B10) permanece INTACTA. NO se ha aplicado
   ninguna migración Canon a producción. Sólo `atlasbuho_fasec`.

g) Tests unitarios/integration corren 100% en local con MySQL
   Admin/[REDACTED]. No hay CI configurado para reproducibilidad fuera.

4. COMANDOS CANÓNICOS (para retomar)
====================================
# Extracción PDF a JSON canónico (regenera catalogs/extracted/inali_pdf_canon_full.json)
cd C:/Users/Admin/source/repos/AtlasBuho
python tests/AtlasBuho.Tests/test_inali_pdf_extraction.py    # 7/7 PASS

# Aplicar migraciones (en BD fresca)
export ConnectionStrings__MySQL="Server=127.0.0.1;Port=3306;Database=atlasbuho_fasec;User=Admin;Password='<NO_SUBIR_A_GIT>';AllowLoadLocalInfile=true"
export DOTNET_ROLL_FORWARD=LatestMajor
dotnet ef database update --project src/AtlasBuho.Data --startup-project tests/AtlasBuho.ImportRunner

# Cargar canónico + verificar
dotnet run --project tests/AtlasBuho.ImportRunner -- --load-canon
dotnet run --project tests/AtlasBuho.ImportRunner -- --verify-canon

# Tests
dotnet test tests/AtlasBuho.Tests --configuration Release --no-restore        # 68/68
dotnet test tests/AtlasBuho.Tests.Integration --configuration Release --no-restore  # 52/52

5. CREDENCIALES (recordatorio — NO las subas a git)
===================================================
MySQL: usuario 'Admin', password proporcionado por el usuario en esta sesión (no
registrarla en archivos del repo).
Env vars: ConnectionStrings__MySQL, ATLASBUHO_TEST_DB_PASSWORD, DOTNET_ROLL_FORWARD=LatestMajor

6. DECISIONES CERRADAS (no volver a discutir salvo que lo pidas)
================================================================
- PD-A11: opción B (FKs reales en DbContext, no Evidence polimórfica)
- PD-A19: semántica cerrada (NULL/UNKNOWN/UNVERIFIED/DOCUMENTED/VERIFIED/RECONCILED)
- PD-A15: ReconciliationResultCanon separado con CatalogRecordId FK NOT NULL
- Fuente canónica: PDF INALI oficial (HTML e INEGI = cross-checks)
- No corregir discrepancias silenciosamente — ledger explícito

7. PRÓXIMOS PASOS SUGERIDOS (en orden)
======================================
1. Decidir si poblar ReconciliationResultCanon con estado NOT_APPLICABLE
   o MISSING según cross-checks.
2. Diseñar y aprobar el mapeo geo_reference -> Pais/Estado/Municipio/Localidad
   (reglas, dedupe, PronounciationSystem si aplica).
3. Descargar y procesar INEGI XLSX + datos.gob.mx CSV para enriquecer
   el ledger antes de la normalización geográfica final.
4. Definir política para autodenominaciones vacías.
5. Ejecutar CI (GitHub Actions) para reproducir Fase B/C fuera del entorno local.

SESIÓN CERRADA EN ESTE PUNTO.
==========================================================
