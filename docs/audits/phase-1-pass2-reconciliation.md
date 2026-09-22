# FASE 1 - PASS 2: DATASET RECONCILIATION

**Fecha:** 2025-09-22
**Commit objetivo:** 313bfe5

---

## 1. COMPARACIÓN SOURCE vs PARSED vs DOCUMENTAL

### 1.1 Familias (11) - ✅ RECONCILIADAS

| Fuente | Cuenta | Estado |
|--------|--------|--------|
| PDF (documental) | 11 | Referencia |
| `inali_final_catalog.json` metadata | 11 | ✅ |
| `groups_by_family` keys | 11 | ✅ |
| Variantes (familias únicas) | 11 | ✅ |
| Apéndice 4 (familias únicas) | 11 | ✅ |

**Familias:** álgica, cochimí-yumana, huave, maya, mixe-zoque, oto-mangue, seri, tarasca, totonaco-tepehua, yuto-nahua, chontal de Oaxaca

---

### 1.2 Grupos / Agrupaciones - ⚠️ DISCREPANCIA CRÍTICA

| Fuente | Cuenta | Detalle |
|--------|--------|---------|
| PDF Apéndice 4 (agrupaciones válidas) | **68** | Excluyendo "Oto-mangue", "u" (artefactos) |
| `inali_final_catalog.json` metadata `groups` | **67** | |
| `groups_by_family` suma | **67** | 11+5+1+18+20+2+1+7+1+1 = 67 |
| Apéndice 4 agrupaciones únicas (raw) | **70** | Incluye 2 artefactos |
| Importer `EXPECTED_GROUPS` | **68** | |

**Análisis:** El Apéndice 4 es la fuente canónica oficial para las 68 agrupaciones. El JSON principal deriva 67 grupos del campo `grupo` de las variantes, pero **las variantes NO tienen campo `grupo`** (solo `family`). Los 67 grupos en el JSON principal vienen de `groups_by_family` que fue calculado durante el parsing original, probablemente usando inferencia.

**Las 68 agrupaciones oficiales (Apéndice 4 válido):**

| Familia | Agrupaciones (cuenta) |
|---------|----------------------|
| Álgica | Kickapoo (1) |
| Cochimí-yumana | cucapá, kiliwa, ku'ahl, kumiai, paipai (5) |
| Huave | huave (1) |
| Maya | Akateko, Awakateko, Chuj, Ixil, Jakalteko, Kaqchikel, K'iche', Mam, Q'anjob'al, Q'eqchi', Teko, ch'ol, chontal de Tabasco, huasteco, lacandón, maya, qato'k, tojolabal, tseltal, tsotsil (20) |
| Mixe-zoque | ayapaneco, mixe, oluteco, popoluca de la sierra, sayulteco, texistepequeño, zoque (7) |
| Oto-mangue | amuzgo, chatino, chichimeco jonaz, chinanteco, chocholteco, cuicateco, ixcateco, matlatzinca, mazahua, mazateco, mixteco, otomí, pame, popoloca, tlahuica, tlapaneco, triqui, zapoteco (18) |
| Seri | seri (1) |
| Tarasca | tarasco (1) |
| Totonaco-tepehua | tepehua, totonaco (2) |
| Yuto-nahua | cora, guarijío, huichol, mayo, náhuatl, pima, pápago, tarahumara, tepehuano del norte, tepehuano del sur, yaqui (11) |
| Chontal de Oaxaca | chontal de Oaxaca (1) |
| **TOTAL** | **68** |

---

### 1.3 Variantes - ⚠️ DISCREPANCIA

| Fuente | Cuenta | Estado |
|--------|--------|--------|
| PDF cuerpo principal (documental) | **364** | Referencia oficial |
| `inali_final_catalog.json` `variants_detail` | **364** | ✅ Coincide |
| Importer `EXPECTED_VARIANTS` | **361** | ❌ **-3 variantes** |
| Importer importadas (última corrida) | **361** | Confirma pérdida |

**Las 3 variantes que se pierden:** Son las que no logran mapear a una agrupación válida del Apéndice 4 durante `ImportVariantsAsync` debido al `continue` silencioso.

**Candidatas probables** (variantes cuyas familias/grupos no tienen mapeo directo en Apéndice 4):
- Variantes de familias con 1 sola agrupación donde el nombre difiere
- Variantes con nombres que no hacen match por fuzzy matching

---

### 1.4 Autodenominaciones - ⚠️ SIN RECONCILIAR DOCUMENTALMENTE

| Fuente | Cuenta | Estado |
|--------|--------|--------|
| Apéndice 4 filas válidas (477 - 3 artefactos) | **474** | Universo documental |
| Importer `EXPECTED_AUTODENOMINATIONS` | **424** | ❌ **Gap de 50** |
| Importer importadas (última corrida) | **424** | Confirma gap |

**Análisis del gap (50 autodenominaciones no importadas):**

El Apéndice 4 usa nombres abreviados en `spanish_name` (ej. "zapoteco") mientras que el catálogo principal tiene nombres completos con ubicación (ej. "zapoteco de Valles, oeste"). El matching por `spanish_name` como prefijo falla cuando:
1. Múltiples variantes comparten el mismo prefijo (ej. 8 variantes de "zapoteco de Valles")
2. El nombre en español del Apéndice es genérico y no permite desambiguar
3. La autodenominación aparece en el Apéndice pero la variante no tiene autodenominación en el catálogo principal

**Distribución por agrupación (top 10 con más autodenoms):**
1. zapoteco: 99
2. mixteco: 90
3. náhuatl: 43
4. otomí: 22
5. mazateco: 16
6. totonaco: 11
7. chinanteco: 20
8. cora: 8
9. tsotsil: 7
10. popoloca: 7

---

### 1.5 Referencias Geoestadísticas - ✅ PARCIALMENTE RECONCILIADAS

| Métrica | Cuenta | Estado |
|---------|--------|--------|
| Variantes totales | 364 | |
| Con `geo_reference` no vacío | 359 | ✅ |
| Sin `geo_reference` | 5 | ✅ Identificadas |

**Las 5 sin geo_reference:**
1. Kickapoo (álgica) - página 31
2. kiliwa (cochimí-yumana) - página 36
3. seri (seri) - página 38
4. Ixil nebajeño (maya) - página 58
5. huave del este (huave) - página 40

---

## 2. MAPEO DE HALLAZGOS A ACCIONES REQUERIDAS

| Hallazgo | Acción Requerida | Prioridad |
|----------|------------------|-----------|
| H-001: Baseline importer ≠ JSON | Actualizar constantes a valores documentales (364, 68, 474) | CRÍTICA |
| H-002: `continue` silencioso | Reemplazar con quarantine/unresolved logging | CRÍTICA |
| H-003: CatalogRecords semántica | Definir si Autodenomination es EntityType propio | ALTA |
| H-004: Proveniencia página 30 | Usar página real de cada entidad | CRÍTICA |
| H-005: RawTextHash vacío | Extraer texto por página y calcular SHA256 | ALTA |
| H-006: Metadatos hardcodeados | Derivar de PDF real y JSON | ALTA |
| H-007: No revalidar SourceDocument | Recalcular hash desde archivo antes de importar | CRÍTICA |
| H-008: Credencial en código | Mover a User Secrets, limpiar git history | CRÍTICA (SEGURIDAD) |
| H-009: CatalogVersion Completed inicial | Estado inicial = Pending/Importing | ALTA |
| H-010: VersionNumber fijo | Política determinista basada en hash+parser | ALTA |
| H-011: Idempotencia no demostrada | Implementar upsert por identidad documental | CRÍTICA |
| H-012: Identidad variante | Definir clave lógica: family+group+variant+page | ALTA |
| H-013: 424 vs 474 autodenoms | Reconciliar: importar resolubles, quarantine no resolubles | CRÍTICA |
| H-014: Artefactos Appendix 4 | Clasificar como ARTIFACT, mantener raw | ALTA |
| H-015: Duplicado autodenom contexto | Identidad = VariantId + Autodenom + SourcePage | ALTA |
| H-016: Inferencia DeriveGroupName | Registrar resolution_method por variante | ALTA |
| H-017: Normalización destructiva | Preservar original, usar comparison_key separado | ALTA |
| H-018: Unicode MySQL | Probar round-trip real | CRÍTICA |
| H-019: Proveniencia navegable | SourcePage → Records bidireccional | ALTA |
| H-020: CatalogRecord placeholders | Eliminar valores hardcoded, usar reales | ALTA |
| H-021: GeoReference 5 faltantes | Documentar individualmente | MEDIA |
| H-022: Familias/grupos páginas reales | Propagar página a CatalogRecord | ALTA |
| H-023: SourcePage contador vs real | Marcar páginas con evidencia extraída | MEDIA |
| H-024: ParserVersion | Versionar por operación de parsing | MEDIA |
| H-025: RetrievedAt identidad | Separar retrieval de import execution | MEDIA |

---

## 3. UNIVERSO DOCUMENTAL OFICIAL (PARA VALIDACIÓN FINAL)

| Entidad | Cuenta Oficial | Fuente |
|---------|----------------|--------|
| Familias | 11 | PDF + JSON principal |
| Agrupaciones | 68 | Apéndice 4 (oficial) |
| Variantes | 364 | PDF cuerpo principal (oficial) |
| Autodenominaciones válidas | 474 | Apéndice 4 (oficial) - 3 artefactos |
| Con geo | 359 | JSON principal (extraído de PDF) |
| Sin geo | 5 | JSON principal |

---

## 4. ARCHIVOS GENERADOS

- `docs/audits/phase-1-pass2-reconciliation.md` (este documento)

---

## 5. PRÓXIMO PASO

**CORRECCIONES**: Implementar fixes para todos los hallazgos H-001 a H-025 en el código del importer y modelo de datos.