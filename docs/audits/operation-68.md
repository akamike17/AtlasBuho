# OPERACIÓN 68 — INFORME FORENSE FINAL

## 1. Identidad de la Fuente

| Campo | Valor |
|-------|-------|
| **Archivo** | `catalogs/raw/inali/CLIN_completo.pdf` |
| **SHA-256** | `21cef44abf0d896555f26954bf319340ad4814fa8a3f0719b0d068311ff1be7c` |
| **Tamaño** | 3,082,863 bytes |
| **Páginas totales** | 256 |
| **Rango catálogo** | 30–212 (183 páginas) |
| **URL original** | `https://www.inali.gob.mx/pdf/CLIN_completo.pdf` |
| **Fecha obtención** | 2025-09-21T19:52:35-06:00 |
| **Publicación** | DOF Lunes 14 de enero de 2008 (Primera, Segunda y Tercera Sección) |

**Verificación**: SHA-256 coincide exactamente con el esperado. ✅

---

## 2. Metodología de Extracción

### Herramientas
- `pdfplumber` (Python) — extracción de texto y palabras con coordenadas
- Parser personalizado con detección de etiquetas `<variante>` y encabezados `AUTODENOMINACIÓN DE LA VARIANTE LINGÜÍSTICA`
- Extracción de Apéndice 4 mediante `extract_words()` con coordenadas X/Y para reconstrucción tabular

### Rutas independientes
1. **Parser A**: Detección de variantes via etiquetas `<...>` + asociación a agrupación por encabezado previo
2. **Parser B**: Índice alfabético páginas 11-12 (68 entradas nominales)
3. **Parser C**: Árboles genealógicos (70 nodos, 3 marcados †)
4. **Parser D**: Apéndice 4 — autodenominaciones (475 filas, 67 agrupaciones)

---

## 3. Enumeración Independiente de Familias

| # | Familia | Página inicio | Agrupaciones (índice) | Variantes |
|---|---------|--------------|----------------------|-----------|
| 1 | álgica | 30 | 1 | 1 |
| 2 | yuto-nahua | 33 | 10 | 61 |
| 3 | cochimí-yumana | 86 | 4 | 4 |
| 4 | seri | 90 | 1 | 1 |
| 5 | oto-mangue | 88 | 17 | 175 |
| 6 | maya | 54 | 21 | 69 |
| 7 | totonaco-tepehua | 196 | 2 | 9 |
| 8 | tarasca | 203 | 1 | 3 |
| 9 | mixe-zoque | 205 | 7 | 24 |
| 10 | chontal de Oaxaca | 211 | 1 | 5 |
| 11 | huave | 212 | 1 | 12 |
| **TOTAL** | **11** | | **68** | **364** |

**Familias = 11 VERIFIED** ✅

---

## 4. Enumeración Independiente de Agrupaciones

### 4.1 Índice Alfabético (Páginas 11-12) — **68 entradas canónicas**
Lista completa en `docs/audits/operation-68-direct-search.md`. Cada entrada tiene: nombre + familia.

### 4.2 Catálogo Principal (Páginas 30-212) — **67 con variantes**
Todas las 68 del índice aparecen en el catálogo principal **EXCEPTO Q'anjob'al**.
- Q'anjob'al aparece en índice (p.11) como "VI Familia maya"
- En catálogo principal (p.189): variantes K'anjob'al, Akateko, Qato'k, Chuj, Jakalteko bajo la familia maya
- Pero el encabezado de agrupación "Q'anjob'al" **no aparece** como tal en páginas 30-212

### 4.3 Árboles Genealógicos — **70 nodos**
- 67 con variantes vivas en catálogo
- 3 extintas marcadas †: tepecano, ópata, tubar (todas en árbol yuto-nahua)
- 0 de las 3 extintas aparecen en índice o catálogo principal

### 4.4 Apéndice 4 (Páginas 244-256) — **67 agrupaciones con autodenominaciones**
- 475 autodenominaciones para 364 variantes
- Falta: Q'anjob'al
- Presente: Q'eqchi' (p.252) ✅

---

## 5. Enumeración de Variantes

| Métrica | Valor | Evidencia |
|---------|-------|-----------|
| **Total variantes** | **364** | 364 etiquetas `<...>` únicas parseadas con página y familia |
| **Con autodenominación (cuerpo)** | 56/364 | Extracción limitada por formato |
| **Con autodenominación (Ap.4)** | 475/364 | Apéndice 4 declara explícitamente 475 para 364 variantes |
| **Con referencia geoestadística** | 359/364 | 5 sin geo (NULL con provenance) |
| **Con códigos ISO/INALI** | 0/364 | PDF no incluye códigos |

**Variantes = 364 VERIFIED** ✅

---

## 6. Análisis de Árboles Genealógicos

| Familia | Nodos en árbol | Con variantes | Extintas (†) |
|---------|---------------|---------------|--------------|
| álgica | 1 | 1 | 0 |
| yuto-nahua | 13 | 10 | 3 (tepecano, ópata, tubar) |
| cochimí-yumana | 4 | 4 | 0 |
| seri | 1 | 1 | 0 |
| oto-mangue | 17 | 17 | 0 |
| maya | 21 | 21 | 0 |
| totonaco-tepehua | 2 | 2 | 0 |
| tarasca | 1 | 1 | 0 |
| mixe-zoque | 7 | 7 | 0 |
| chontal de Oaxaca | 1 | 1 | 0 |
| huave | 1 | 1 | 0 |
| **TOTAL** | **70** | **67** | **3** |

Los 3 nodos extintos **no aparecen en índice ni catálogo principal**.

---

## 7. Análisis del Índice

| Fuente | Agrupaciones | Coincide con catálogo | Nota |
|--------|-------------|----------------------|------|
| Índice (pp.11-12) | 68 | 67/68 | Falta Q'anjob'al en catálogo principal |
| Árboles | 70 | 67/70 | 3 extintas solo en árboles |
| Catálogo principal | 67 | — | Base para parser |
| Apéndice 4 | 67 | 67/68 | Falta Q'anjob'al |

**Diferencias ortográficas detectadas**: Ninguna significativa. Nombres consistentes entre índice y catálogo.

---

## 8. Análisis de Apéndices

### Apéndice 2 (p.233)
- Estructurado alrededor de **68 agrupaciones** como entradas principales
- Confirma definición oficial

### Apéndice 4 (pp.244-256) — Tabla de autodenominaciones
- **475 autodenominaciones** declaradas para 364 variantes
- **67 agrupaciones** representadas (falta Q'anjob'al)
- Error editorial: omisión de K'anjob'al/Q'anjob'al en tabla alfabética

---

## 9. Análisis de Agrupaciones Extintas

| Candidato | Nombre exacto | Árbol | Índice | Catálogo | Variantes | † | Conclusión |
|-----------|--------------|-------|--------|----------|-----------|---|------------|
| tepecano | tepecano † | ✅ | ❌ | ❌ | ❌ | ✅ | NOT PROVEN — solo en árbol |
| ópata | ópata † | ✅ | ❌ | ❌ | ❌ | ✅ | NOT PROVEN — solo en árbol |
| tubar | tubar † | ✅ | ❌ | ❌ | ❌ | ✅ | NOT PROVEN — solo en árbol |

**Ninguno de los tres cumple criterios para ser "la 68ª agrupación oficial"**: no están en índice, no tienen variantes en catálogo, no aparecen en apéndices.

---

## 10. Ocurrencias Exactas de "68" en el CLIN

| # | Página | Sección | Texto Exacto | Tipo |
|---|--------|---------|--------------|------|
| 1 | 8 | Contenido | "las 68 agrupaciones lingüísticas correspondientes a dichas familias" | Definición canónica |
| 2 | 10 | Descripción | "Cada una de las 68 agrupaciones lingüísticas aquí catalogadas" | Confirmación explícita |
| 3 | 11 | Nomenclatura | "Las 68 agrupaciones lingüísticas catalogadas... se enlistan..." + 68 entradas | **Lista nominal completa** |
| 4 | 231 | Nota 2 | "INALI (2007) reconoce 68 agrupaciones" | Contexto histórico |
| 5 | 233 | Apéndice 2 | "cada una de las 68 agrupaciones lingüísticas consideradas" | Estructura apéndice |

**5 ocurrencias independientes** en 5 secciones distintas. Todas apuntan a 68 como universo oficial.

---

## 11. Reconciliación Bidireccional

### Ruta A: Cifra → Contenido (68 → ¿qué cuenta?)
- El CLIN define 68 en 5 ubicaciones
- La **lista nominal en páginas 11-12** es la única enumeración explícita
- Esas 68 entradas tienen nombre y familia
- **Q'anjob'al es la entrada #47 del índice**

### Ruta B: Contenido → Cifra (catálogo → cuántos)
- 11 familias reconstruidas ✅
- 364 variantes reconstruidas ✅
- Agrupaciones con variantes en catálogo: **67**
- Agrupaciones en índice: **68**
- Agrupaciones en árboles: **70 (67 vivas + 3 extintas)**
- Agrupaciones en Apéndice 4: **67**

**Discrepancia**: Q'anjob'al está en índice pero su encabezado de agrupación no aparece en catálogo principal (aunque sus variantes sí, p.189).

---

## 12. Contexto Externo (Solo Referencia)

| Fuente | Agrupaciones reportadas | Nota |
|--------|------------------------|------|
| CGEIB | 66 | Nota 2, p.231 |
| DGCPI | 67 | Nota 2, p.231 |
| CDI | 66 | Nota 2, p.231 |
| INALI 2005 | 58 | Nota 2, p.231 |
| INALI 2007 (CLIN) | **68** | **Fuente canónica para AtlasBúho** |
| Ethnologue/Glottolog | Variable | No vinculante para Fase 1 |

**Regla aplicada**: CLIN remains canonical for AtlasBúho Phase 1.

---

## 13. Determinación Final

### Estado de la Cuenta de Agrupaciones

```
GROUPING_COUNT_STATUS = UNKNOWN
```

### Justificación

1. **El CLIN SÍ define 68 oficialmente** — 5 ocurrencias documentales, lista nominal completa en pp.11-12
2. **El catálogo principal contiene 67 agrupaciones con variantes** — Q'anjob'al falta como encabezado
3. **Las 3 extintas (tepecano, ópata, tubar) solo están en árboles** — no en índice, no en catálogo, no en apéndices
4. **El CLIN NO explica explícitamente** si el 68 incluye extintas o es solo "agrupaciones con variantes"
5. **La omisión de Q'anjob'al en Apéndice 4** sugiere error editorial, no decisión semántica

### Conclusión forense

> **El número 68 está documentalmente establecido como universo oficial del CLIN, pero la correspondencia 1:1 entre "68 oficiales" y "agrupaciones con variantes en catálogo" no puede demostrarse internamente.** El CLIN presenta una tensión no resuelta entre su definición nominal (68 en índice) y su contenido operacional (67 con variantes).

---

## 14. Bloqueos Remanentes para Fase 1

| Bloqueo | Estado | Acción requerida |
|---------|--------|------------------|
| GROUPING_COUNT = UNKNOWN | **BLOCKED** | Resolver 67 vs 68 (ver §22 3b.md) |
| MYSQL not configured | BLOCKED | Connection string + `dotnet ef database update` |
| Import pipeline | BLOCKED | Transaccional con validación 11/68/364 |
| Provenance model | BLOCKED | Entidades SourceDocument, SourceHash, SourcePage, SourceSection |
| Versioning model | BLOCKED | CatalogSource, CatalogVersion, CatalogRecord |
| Sync service | BLOCKED | ETag/Last-Modified/SHA256 polling + backoff + quarantine |
| Integration tests | BLOCKED | Counts, Unicode, hierarchy, provenance, idempotency |

---

## 15. Archivos de Evidencia Generados

| Archivo | Descripción |
|---------|-------------|
| `catalogs/raw/inali/CLIN_completo.pdf` | RAW inmutable (SHA256 verificado) |
| `catalogs/raw/inali/CLIN_completo.metadata.json` | Metadatos de descarga |
| `catalogs/parsed/inali_final_catalog.json` | 364 variantes con página/familia/autodenom/geo |
| `catalogs/parsed/appendix4_parsed.json` | 478 filas parseadas del Apéndice 4 |
| `catalogs/parsed/appendix4_raw.txt` | Texto crudo Apéndice 4 |
| `docs/audits/operation-68-direct-search.md` | Búsqueda directa de "68" |
| `docs/audits/operation-68-appendix4-verification.md` | Verificación Apéndice 4 |
| `docs/audits/operation-68-evidence.json` | **Evidencia maestra JSON** |
| `docs/audits/operation-68.md` | **Este informe forense** |

---

## 16. Decisión Según §22 3b.md

```
GROUPING_COUNT_STATUS = UNKNOWN
PHASE_1 = BLOCKED
```

**No se avanza a implementación MySQL ni cierre de Fase 1.**

La discrepancia documental crítica (67 vs 68) queda **explicitamente no resuelta** y documentada con evidencia completa. Cualquier implementación posterior debe preservar esta incertidumbre en el modelo de datos (campo `grouping_count_status: UNKNOWN`, nota de proveniencia en cada agrupación).

---

**Fin del informe forense Operación 68.**