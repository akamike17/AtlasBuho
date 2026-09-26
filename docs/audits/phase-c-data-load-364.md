# Fase C — Datos — Load report 364/364

## Resumen
Carga canónica del catálogo INALI 2008 en base `atlasbuho_fasec` (no la sellada `atlasbuho`).
Pipeline ejecutado: PDF canónico → extractor → JSON canon → Entity Framework → MySQL.

## Inputs
- PDF oficial: catalogs/inali/catalogo_lenguas_indigenas.pdf
- SHA256: e38e667d024784bc84cc350f44918c1d22d998284d02e38dcf92858257947311
- JSON canónico: catalogs/extracted/inali_pdf_canon_full.json
- Discrepancy ledger: catalogs/extracted/discrepancy_ledger.json

## Migration y estado previo
- Migration última aplicada: 20260926053354_FaseB-CanonModel
- Base objetivo: atlasbuho_fasec (fresca, sin datos previos)

## Pipeline (Fase C)
1. PDF extraction — python + pdfplumber → 364 rows JSON literal.
2. Reconciliación — assertion tests 7/7 PASS (catalogs/extracted/test_inali_pdf_extraction.py).
3. Migration data — dotnet run --project tests/AtlasBuho.ImportRunner -- --load-canon.
4. BD population — atlasbuho_fasec (no la sellada).
5. 364/364 verified — dotnet run --project tests/AtlasBuho.ImportRunner -- --verify-canon + SQL snippets.

## Resultados verificados
| Métrica                          | Valor |
|----------------------------------|-------|
| LanguageFamilies                 | 11    |
| LanguageGroups                   | 68    |
| LanguageVariants                 | 364   |
| CanonVariantes                   | 364   |
| Sources                          | 364   |
| LanguageVariants distinct Name   | 364   |
| Variantes sin Canon              | 0     |
| Canon sin Variante               | 0     |
| Canon con Fuente                 | 364   |
| Canon con Página                 | 364   |

## Comando exacto (reproducible)
```bash
mysql -h 127.0.0.1 -u Admin -p<password> -e "CREATE DATABASE atlasbuho_fasec CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;"
export ConnectionStrings__MySQL="Server=127.0.0.1;Port=3306;Database=atlasbuho_fasec;User=Admin;Password=...;AllowLoadLocalInfile=true"
dotnet ef database update --project src/AtlasBuho.Data --startup-project tests/AtlasBuho.ImportRunner
dotnet run --project tests/AtlasBuho.ImportRunner -- --load-canon
dotnet run --project tests/AtlasBuho.ImportRunner -- --verify-canon
```

## Reconciliation one-row-one-variant
- Cada fila del JSON canónico → exactamente 1 LanguageVariant + 1 VarianteCanon + 1 Source.
- Resultado: 364/364 matched. Ningún huerfano.

## Salvedades explícitas
- Autodenominación vacía: 1 (otomí del oeste del Valle del Mezquital p.150) — vacío en fuente, marcado; NO rellenado.
- 47 variantes en PDF sin match en appendix4 (cross-check en discrepancy_ledger.json — no corregidas silenciosamente).
- 300 variantes en PDF sin match en HTML INALI (el HTML es parcial 235; cross-check reportado).
- Geografía original permanece como string literal en LanguageVariant.Source = JSON path; la normalización a PaisCanon/EstadoCanon/MunicipioCanon/LocalidadCanon es trabajo posterior no autorizado aún.

## Estado
FASE C — CLOSE (sujeto a tu aprobación). La base `atlasbuho` original (B10)
permanece intacta y NO fue tocada por esta carga.
