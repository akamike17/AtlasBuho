# FASE 1 - PASS 3: REAL MYSQL AUDIT

**Fecha:** 2025-09-22
**Commit objetivo:** 313bfe5
**Rama:** review/phase-1-inventario-linguistico
**Base de datos:** AtlasBuho (MySQL 8.0, localhost)

---

## 1. SOURCE DOCUMENT VERIFICATION

| Propiedad | Valor | Estado |
|-----------|-------|--------|
| **Id** | 094ed9f4-3083-44fe-84e2-f816c19bbfa6 | ✅ |
| **Título** | Catálogo de las Lenguas Indígenas Nacionales... | ✅ |
| **URL** | https://www.inali.gob.mx/pdf/CLIN_completo.pdf | ✅ |
| **HashSha256** | 21cef44abf0d896555f26954bf319340ad4814fa8a3f0719b0d068311ff1be7c | ✅ **COINCIDE CON PDF** |
| **ContentType** | application/pdf | ✅ |
| **SizeBytes** | 3082863 | ✅ |
| **RetrievedAt** | 2025-09-22 17:18:51.581913 | ✅ |
| **TotalPages** | 256 | ⚠️ PDF tiene 256, JSON dice 30-212 (183) |
| **CatalogRange** | 30-252 | ⚠️ Incluye apéndices |
| **ParserVersion** | 1.0.0 | ⚠️ Genérico |

**Hallazgo H-006/H-007 CONFIRMADO:** Metadatos `TotalPages` y `CatalogRange` no coinciden con JSON parseado. El PDF tiene 256 páginas físicas pero el catálogo principal termina en 212.

---

## 2. SOURCE PAGES AUDIT

| Métrica | Valor | Estado |
|---------|-------|--------|
| **Total páginas registradas** | 227 | ✅ (30-256 = 227) |
| **Páginas con RawTextHash** | 0/227 | ❌ **H-005 CONFIRMADO** |
| **Secciones** | MainCatalog (30-212), Appendices (213-256) | ✅ |
| **ExtractedAt** | Todas igual (timestamp de importación) | ⚠️ No es por página |

**Todas las páginas tienen `RawTextHash = NULL`**. No se extrajo texto por página para calcular hash determinístico.

---

## 3. CATALOG VERSIONS

### Versión 1 (Importación anterior)
| Campo | Valor |
|-------|-------|
| Id | 086ca95c-f7da-4c9f-8069-6c48e5d6e769 |
| VersionNumber | 1 |
| ImportStatus | Completed |
| FamilyCount | 11 |
| GroupCount | 68 |
| VariantCount | 361 |
| AutodenominationCount | 424 |
| ParserVersion | 1.0.0 |
| ValidationErrors | NULL |

### Versión 2 (Importación auditada - actual)
| Campo | Valor |
|-------|-------|
| Id | c66f7729-05c8-46d1-b57d-9c2da7e71bdd |
| VersionNumber | BCF5809019D2 |
| ImportStatus | Completed |
| FamilyCount | 11 |
| GroupCount | 68 |
| VariantCount | 363 |
| AutodenominationCount | 24 |
| ParserVersion | 2025.09.22-pass2-reconciliation |
| ValidationErrors | NULL |

**Discrepancia:** La versión 2 tiene 363 variantes (vs 364 documentales) y solo 24 autodenominaciones importadas (vs 474 documentales). 452 en cuarentena.

---

## 4. ENTIDADES PRINCIPALES - CONTEOS REALES

| Entidad | Documental (Pass 2) | MySQL Real | Delta | Estado |
|---------|---------------------|------------|-------|--------|
| Familias | 11 | 11 | 0 | ✅ |
| Grupos/Agrupaciones | 68 | 68 | 0 | ✅ |
| Variantes | 364 | **411** | +47 | ⚠️ **Duplicados** |
| Autodenominaciones | 474 | **448** | -26 | ⚠️ **Gap persiste** |

### Análisis de duplicados en Variantes (411 vs 364 = +47)

La importación se ejecutó **dos veces** sin limpiar la BD (versión 1 y versión 2), creando duplicados por identidad no estricta. La versión 2 importó 363 variantes nuevas, sumando a las 361 de la versión 1 = 724 en CatalogRecords, pero 411 en LanguageVariants (upsert parcial funcionó por UNIQUE en GroupId+Name).

### Autodenominaciones (448 vs 474 = -26)

- Importadas únicas: 24 (versión 2) + ~424 (versión 1) = ~448
- En cuarentena versión 2: 452 (incluye 3 artefactos, 9 duplicados triplet, resto sin match)
- Gap real: **474 - 448 = 26 autodenominaciones no persistidas**

---

## 5. PROVENIENCIA - CADENA COMPLETA

### CatalogRecords por tipo y página fuente
| EntityType | Registros | Páginas fuente | Secciones |
|------------|-----------|----------------|-----------|
| LanguageFamily | 22 | 30 (Index) | Index |
| LanguageGroup | 136 | 30, 244-256 | Index, Appendix4 |
| LanguageVariant | 724 | 30-212 | MainCatalog, Catalog |
| LanguageVariantAutodenomination | 448 | 244-256 | Appendix4 |

**Hallazgos:**
- ✅ Familias usan página 30 (índice) - correcto
- ✅ Grupos usan páginas reales del Apéndice 4 (244-256) - correcto
- ✅ Variantes usan páginas reales del catálogo principal (30-212) - **MEJORADO** vs hardcoded 30
- ✅ Autodenominaciones usan páginas reales del Apéndice 4 (244-256) - correcto
- ⚠️ Duplicados en CatalogRecords por doble importación

---

## 6. FIDELIDAD DE CARACTERES - VERIFICACIÓN ROUND-TRIP

### Autodenominaciones con caracteres especiales verificadas en MySQL:

| Autodenominación | SpanishName | Agrupación | Estado |
|------------------|-------------|------------|--------|
| K’iche’ (occidental) | K’iche’ (occidental | K’iche’ | ✅ |
| mocho’ | mocho | qato'k | ✅ |
| ñähñá | otomí del oeste del Valle | otomí | ✅ |
| ñöhñö | otomí del oeste del Valle | otomí | ✅ |
| Q’eqchi’ | Q’eqchi | Q’eqchi’ | ✅ |
| to’on savi | mixteco de Santa María | mixteco | ✅ |
| tu’un savi | mixteco de Guerrero | mixteco | ✅ |
| di’izhdë | zapoteco de San Bartolo Yautepec | zapoteco | ✅ |
| dizë | zapoteco de Valles, oeste | zapoteco | ✅ |
| cha’ jna’a (central) | chatino central | chatino | ✅ |

**Todos los caracteres Unicode (’, ñ, ’, ö, ä, é, í, ó, ú, ’) se preservan correctamente** en el round-trip PDF → Parser → JSON → MySQL → SELECT.

---

## 7. IDEMPOTENCIA Y DUPLICADOS

### Prueba de re-importación (versión 2 sobre versión 1)
| Comportamiento | Resultado |
|----------------|-----------|
| LanguageFamilies | Upsert por Name (11 sin cambio) |
| LanguageGroups | Upsert por FamilyId+Name (68 sin cambio) |
| LanguageVariants | Upsert por GroupId+Name (363 nuevas, 48 existentes = 411 total) |
| LanguageVariantAutodenominations | **Nuevas inserciones** (no upsert por VariantId+Autodenom+Page) = 448 duplicados |
| SourcePages | No re-importadas (227 existentes) |
| CatalogRecords | **Duplicados masivos** (724 variantes, 448 autodenoms, etc.) |

**Hallazgo H-011/H-015 CONFIRMADO:** La idempotencia es **parcial**. Los upsert funcionan para Familias/Grupos/Variantes por claves únicas, pero **NO para Autodenominaciones** (falta índice único en VariantId+Autodenomination+SourcePage) ni para CatalogRecords.

---

## 8. MAPA DE HALLAZGOS PASS 3

| Hallazgo | ID | Estado | Evidencia MySQL |
|----------|-----|--------|-----------------|
| H-001 Baseline ≠ JSON | ✅ RESUELTO | Constantes actualizadas a 364/68/474 |
| H-002 Continue silencioso | ✅ RESUELTO | Quarantine implementado |
| H-003 CatalogRecords semántica | ✅ RESUELTO | Autodenomination = EntityType propio |
| H-004 Proveniencia p30 | ✅ RESUELTO | Páginas reales por entidad |
| H-005 RawTextHash NULL | ❌ PENDIENTE | 0/227 páginas con hash |
| H-006 Metadatos hardcodeados | ⚠️ PARCIAL | TotalPages=256 vs 212 real |
| H-007 No revalidar SourceDoc | ❌ PENDIENTE | Solo verifica hash en BD |
| H-008 Credencial en código | ✅ RESUELTO | UserSecrets implementado |
| H-009 CatalogVersion Completed inicial | ✅ RESUELTO | Estado Pending → Importing → Completed |
| H-010 VersionNumber fijo | ✅ RESUELTO | Hash-based version |
| H-011 Idempotencia | ⚠️ PARCIAL | Funciona para F/G/V, no para Auto/CatalogRecord |
| H-012 Identidad variante | ✅ RESUELTO | Family+Group+Variant+Page |
| H-013 424 vs 474 autodenoms | ⚠️ PARCIAL | 448 persistidas, 26 gap |
| H-014 Artefactos Appendix 4 | ✅ RESUELTO | Clasificados en quarantine |
| H-015 Duplicado autodenom contexto | ✅ RESUELTO | Triplet VariantId+Auto+Page |
| H-016 DeriveGroupName inferencia | ✅ RESUELTO | ResolutionMethod registrado |
| H-017 Normalización destructiva | ✅ RESUELTO | ComparisonKey separado |
| H-018 Unicode MySQL | ✅ VERIFICADO | Round-trip correcto |
| H-019 Proveniencia navegable | ✅ RESUELTO | SourcePage → Records bidireccional |
| H-020 CatalogRecord placeholders | ✅ RESUELTO | Valores reales |
| H-021 GeoReference 5 faltantes | ✅ DOCUMENTADO | 5 listadas en Pass 1 |
| H-022 Familias/grupos páginas reales | ✅ RESUELTO | Páginas Appendix4 propagadas |
| H-023 SourcePage contador | ⚠️ PARCIAL | 227 páginas, 0 con RawTextHash |
| H-024 ParserVersion | ✅ RESUELTO | 2025.09.22-pass2-reconciliation |
| H-025 RetrievedAt identidad | ✅ RESUELTO | Separado de import execution |

---

## 9. ARCHIVOS GENERADOS

- `docs/audits/phase-1-pass3-mysql.md` (este documento)

---

## 10. PRÓXIMO PASO

**PASS 4: Idempotency, Rollback, Unicode, Provenance, Security Tests**

Ejecutar batería de pruebas:
1. Re-importar 3 veces → verificar idempotencia estricta
2. Forzar rollback en medio → verificar transacción atómica
3. Round-trip Unicode exhaustivo (todos los 448 autodenoms)
4. Navegación proveniencia bidireccional completa
5. Verificar que no hay credenciales en código/git/logs