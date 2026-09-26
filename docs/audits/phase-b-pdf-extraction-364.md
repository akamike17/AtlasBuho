# Auditoría Fase B · Extracción PDF INALI oficial → 364/364

**Fecha (UTC):** 2026-09-25
**Branch:** review/phase-1-inventario-linguistico
**Commit context:** sobre f127df2 (manifest + fuentes staged)
**Regla:** BD.MD §36. La carga 364 sigue **PROHIBIDA** hasta Fase B cerrada (Canon EF + migración + tests integración nunca corrieron aún).

## 1. Fuente canónica

- URL: https://site.inali.gob.mx/pdf/catalogo_lenguas_indigenas.pdf
- Archivo: `catalogs/inali/catalogo_lenguas_indigenas.pdf`
- SHA256: `e38e667d024784bc84cc350f44918c1d22d998284d02e38dcf92858257947311`
- Tamaño: 6 822 945 bytes, 382 páginas
- Manifest: `catalogs/extracted/_inali_pdf_manifest.json`

## 2. Extracción canónica

- Salida: `catalogs/extracted/inali_pdf_canon_full.json`
- Filas: 364
- Variantes únicas: 364
- Duplicadas: 0
- Familias: 11
- Agrupaciones: 68
- Unresolved agrupación/familia/variante/geo: 0

## 3. Cobertura por familia

| Familia | Variantes |
| --- | ---: |
| álgica | 1 |
| yuto-nahua | 59 |
| cochimí-yumana | 5 |
| seri | 1 |
| oto-mangue | 220 |
| maya | 43 |
| totonaco-tepehua | 10 |
| tarasca | 1 |
| mixe-zoque | 19 |
| chontal de Oaxaca | 3 |
| huave | 2 |
| **TOTAL** | **364** |

## 4. Cross-check ledger

`catalogs/extracted/discrepancy_ledger.json`:

| Comparación | match | solo PDF | solo otra |
|---|---|---:|---:|
| PDF vs appendix4_parsed.json | 317 | 47 | 30 |
| PDF vs INALI HTML parcial | 64 | 300 | 155 |

Appendix4 viene del viejo parser y cubre 347 `spanish_name`; el HTML es parcial (235 filas agrupadas, 219 únicas). Ninguna diferencia se resolvió silenciosamente: el ledger lista cada variante faltante / extra.

## 5. Tests de regresión (ejecutados)

```
PASS test_canonical_inventory_364
PASS test_no_unresolved_fields
PASS test_every_variant_has_provenance
PASS test_coverage_11_familias_68_agrupaciones
PASS test_manifest_sha256_recorded
PASS test_discrepancy_ledger_exists
```

## 6. Constraints respetados

- NADA se cargó a MySQL ni se creó ningún `DbSet`, EF Configuration o migración.
- Ninguna corrección automática a nombres (regla §7 literal).
- Cada fila lleva `page` (página física del PDF, base 1).
- La correspondencia `variante → familia/agrupación` se derivó del propio cell español `<variante>`; no fue inferida desde la TOC por página (que hubiera roto 23 agrupaciones con página compartida).

## 7. Pendiente (NO cerrado)

- Parsing del campo `geo_reference` en tupla (Estado, Municipio, Localidad[]) — hoy es verbatim.
- Cross-check contra INEGI XLSX 2024 (no descargado todavía).
- PD-A11 quedó con opción B aprobada pero **sin implementar** en EF/Canon.
- PD-A19 semántica cerrada pero no aplicada a ninguna migración.
- Fase B = no cerrada.
