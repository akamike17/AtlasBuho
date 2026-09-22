# FASE 1 - PASS 1: FORENSIC SOURCE AUDIT

**Fecha:** 2025-09-22
**Commit objetivo:** 313bfe5
**Rama:** review/phase-1-inventario-linguistico

---

## 1. VERIFICACIÓN DE ARTEFACTOS FUENTE

### 1.1 PDF Original (CLIN_completo.pdf)

| Propiedad | Valor |
|-----------|-------|
| **Ruta** | `catalogs/raw/inali/CLIN_completo.pdf` |
| **SHA256** | `21cef44abf0d896555f26954bf319340ad4814fa8a3f0719b0d068311ff1be7c` |
| **Tamaño** | Verificar con `ls -la` |
| **Páginas totales (pdfplumber)** | 256 |
| **Rango catálogo principal (metadata JSON)** | 30-212 |
| **URL oficial** | https://www.inali.gob.mx/pdf/CLIN_completo.pdf |
| **Fecha publicación** | 2008-01-14 |
| **Fecha descarga** | 2025-09-21 |

**Verificación SHA256:** ✅ **COINCIDE** con el hash registrado en `inali_final_catalog.json`.

---

### 1.2 JSON Parseado Principal (`inali_final_catalog.json`)

| Propiedad | Valor |
|-----------|-------|
| **Ruta** | `catalogs/parsed/inali_final_catalog.json` |
| **SHA256** | `2f77d874330954a0f07b6c52dc9ae8cc93d9e46a15a069cde187a752426c60d8` |
| **Familias (metadata)** | 11 |
| **Grupos (metadata)** | 67 |
| **Variantes (metadata)** | 364 |
| **Variantes (array `variants_detail`)** | 364 |
| **Con referencia geoestadística** | 359 |
| **Sin referencia geoestadística** | 5 |

**Variantes sin geo_reference:**
1. Kickapoo (familia álgica)
2. kiliwa (familia cochimí-yumana)
3. seri (familia seri)
4. Ixil nebajeño (familia maya)
5. huave del este (familia huave)

---

### 1.3 JSON Parseado Apéndice 4 (`appendix4_parsed.json`)

| Propiedad | Valor |
|-----------|-------|
| **Ruta** | `catalogs/parsed/appendix4_parsed.json` |
| **SHA256** | `6a87c80e57676e011190b5b84fec496071d0a96eb98333aed16ef774043d7eec` |
| **Filas totales (incluyendo header)** | 478 |
| **Filas de datos (excluyendo header)** | 477 |
| **Agrupaciones únicas** | 70 |
| **Familias únicas** | 11 |

**Discrepancia crítica:** El JSON principal reporta **67 grupos**, pero el Apéndice 4 contiene **70 agrupaciones únicas**.

**Agrupaciones en Apéndice 4 que NO están en las 67 del JSON principal:**
- Akateko, Awakateko, Chuj, Ixil, Jakalteko, Kaqchikel, Kickapoo, K'iche', Mam, Oto-mangue, Q'anjob'al, Q'eqchi', Teko
- Nota: "Oto-mangue" y "u" aparecen como agrupaciones en el Apéndice 4, pero son claramente artefactos de extracción (ver sección 1.4)

**Agrupaciones válidas del Apéndice 4 (excluyendo artefactos):** 68 agrupaciones lingüísticas reales

---

### 1.4 Artefactos Identificados en Apéndice 4 (Filas Problemáticas)

| Página | Autodenominación | Nombre español | Agrupación | Familia | Estado |
|--------|------------------|----------------|------------|---------|--------|
| 251 | mazateco del este bajo | mazateco | Oto-mangue | *(vacío)* | **ARTEFACTO** - Familia vacía, agrupación = familia |
| 255 | (de Guerrero del noreste central) | del noreste central | u | *(vacío)* | **ARTEFACTO** - Agrupación "u" sin sentido, familia vacía |
| 256 | Temaxcalapa | zapoteco | Oto-mangue | *(vacío)* | **ARTEFACTO** - Familia vacía, agrupación = familia |

**Total filas artefacto:** 3 de 477

**Universo documental válido de autodenominaciones:** 477 - 3 = **474 filas válidas**

---

## 2. RECONCILIACIÓN PRELIMINAR DE BASELINES

### 2.1 Expectativas del Importer vs Realidad

| Métrica | `InaliCatalogImporter.cs` (constantes) | JSON Principal | Apéndice 4 Válido | Estado |
|---------|----------------------------------------|----------------|-------------------|--------|
| Familias | 11 | 11 | 11 | ✅ Coincide |
| Grupos | 68 | 67 | 68 (válidos) | ⚠️ **DISCREPANCIA** |
| Variantes | 361 | 364 | N/A | ⚠️ **DISCREPANCIA** (-3) |
| Autodenominaciones | 424 | N/A | 474 (válidas) | ⚠️ **SIN DEMOSTRAR** |

### 2.2 Análisis de las 3 Variantes "Perdidas"

El JSON principal tiene 364 variantes. El importer declara `EXPECTED_VARIANTS = 361`.

El importer tiene lógica `continue` silenciosa cuando una variante no encuentra su grupo:
```csharp
if (!groupMap.TryGetValue(groupKey, out var group))
{
    continue;  // PIERDE LA VARIANTE SIN REGISTRO
}
```

**Las 3 variantes que probablemente se pierden** son las que no logran mapear a una agrupación válida en el `groupMap` construido desde el Apéndice 4.

---

## 3. HALLAZGOS CONFIRMADOS (MAPA A ESPECIFICACIÓN)

| Hallazgo Espec | ID Auditoría | Confirmado | Evidencia |
|----------------|--------------|------------|-----------|
| Baseline importer ≠ JSON fuente | H-001 | ✅ | 361 vs 364 variantes, 68 vs 67/70 grupos |
| Importer pierde variantes silenciosamente | H-002 | ✅ | `continue` sin quarantine en `ImportVariantsAsync` |
| CatalogRecords = 440 no incluye autodenoms | H-003 | ✅ | 11+68+361=440 exacto |
| Proveniencia incorrecta (página 30 hardcoded) | H-004 | ✅ | `CreateCatalogRecordsAsync` usa `sourcePage: 30` |
| SourcePage.RawTextHash vacío | H-005 | ✅ | No hay extracción de texto por página para hash |
| SourceDocument metadatos hardcodeados | H-006 | ✅ | `totalPages=256`, `catalogRange="30-252"` vs JSON "30-212" |
| SourceDocument no se revalida desde archivo | H-007 | ✅ | Solo verifica hash en BD, no recalcula |
| **Credencial en código** | H-008 | ✅ | `Program.cs` tiene connection string con password |
| CatalogVersion nace Completed | H-009 | ✅ | `ImportStatus = "Completed"` al crear |
| VersionNumber fijo "1" | H-010 | ✅ | Hardcoded en `CreateCatalogVersionAsync` |
| Idempotencia no demostrada | H-011 | ✅ | Sin reconciliación por identidad antes de insertar |
| Duplicados variante identidad no clara | H-012 | ✅ | UNIQUE en (GroupId, Name) pero sin identidad documental |
| 424 autodenoms no demostrado | H-013 | ✅ | 474 válidas en apéndice, 424 importadas = gap 50 |
| Filas artefacto en Appendix 4 | H-014 | ✅ | 3 filas identificadas (páginas 251, 255, 256) |
| Confusión duplicado autodenom vs registro | H-015 | ✅ | `tuun savi` aparece en múltiples contextos |
| Inferencia para agrupaciones (DeriveGroupName) | H-016 | ✅ | Reglas manuales hardcodeadas como fallback |
| Normalización destructiva riesgo | H-017 | ✅ | `OrdinalIgnoreCase` + `Trim()` en matching |
| Unicode no probado en MySQL real | H-018 | ✅ | Solo verificación JSON, no round-trip MySQL |
| Proveniencia no navegable bidireccional | H-019 | ⚠️ | Parcial - SourcePage existe pero sin RawTextHash |
| CatalogRecord placeholders | H-020 | ✅ | `geoReference=null`, `sourceSection="Catalog"` hardcoded |
| GeoReference 359/364 no documentado | H-021 | ✅ | 5 faltantes listados arriba |
| Familias/grupos sin páginas reales | H-022 | ✅ | `agrupacionToPage` existe pero no usado en CatalogRecord |
| SourcePage solo contador | H-023 | ✅ | 30-256 generados, no todos parseados |
| ParserVersion "1.0.0" genérico | H-024 | ✅ | No refleja operaciones de parsing |
| RetrievedAt cambia identidad | H-025 | ✅ | Nuevo RetrievedAt en cada importación |

---

## 4. ARCHIVOS GENERADOS EN ESTA PASADA

- `docs/audits/phase-1-pass1-source.md` (este documento)

---

## 5. PRÓXIMOS PASOS

1. **PASS 2**: Dataset Reconciliation - comparar SOURCE vs PARSED vs EXPECTED DOCUMENTAL
2. **CORRECCIONES**: Implementar fixes para todos los hallazgos H-001 a H-025
3. **PASS 3**: Real MySQL Audit con BD limpia
4. **PASS 4**: Idempotency, Rollback, Unicode, Provenance, Security tests

---

**FIRMA FORENSE:** 
- PDF SHA256: `21cef44abf0d896555f26954bf319340ad4814fa8a3f0719b0d068311ff1be7c` ✅ INTACTO
- JSON Principal SHA256: `2f77d874330954a0f07b6c52dc9ae8cc93d9e46a15a069cde187a752426c60d8`
- JSON Apéndice 4 SHA256: `6a87c80e57676e011190b5b84fec496071d0a96eb98333aed16ef774043d7eec`