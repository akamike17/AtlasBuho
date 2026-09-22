# OPERACIÓN 69 — RESOLUCIÓN DOCUMENTAL DE LA DISCREPANCIA 68/67

## Conclusión Ejecutiva

**ESTADO FINAL: `RESOLVED_68`**

La discrepancia 68 vs 67 **se resuelve documentalmente**: el CLIN contiene consistentemente **68 agrupaciones lingüísticas** en todas sus secciones canónicas. El conteo de 67 en la Operación 68 fue producto de **errores de extracción del parser** (omisión de Q'anjob'al en catálogo principal y Apéndice 4), no de ausencia real en el documento.

---

## Cadena de Evidencia: Q'anjob'al

### 1. Índice Nominal (Páginas 11-12) — Entrada Canónica #47
```
Q'anjob'al VI Familia maya
```
- Página: 11 (DOF Primera Sección, p. 41)
- Sección: "Las 68 agrupaciones lingüísticas catalogadas... se enlistan..."
- Posición: Entrada 47 de 68 en orden alfabético
- **Estado**: PRESENTE ✅

### 2. Catálogo Principal (Páginas 30-212) — Encabezado de Agrupación
```
Página 189 (DOF Tercera Sección, p. 45):
Line 11: "Q'anjob'al"
Line 12: "AUTODENOMINACIÓN DE LA VARIANTE LINGÜÍSTICA Y REFERENCIA GEOESTADÍSTICA"
```
- Página: 189
- Sección: 8.6. Variantes lingüísticas de las agrupaciones de la familia maya
- Estructura: Encabezado "Q'anjob'al" → inmediatamente seguido de "AUTODENOMINACIÓN..."
- Variantes documentadas: `<Q'anjob'al>`, `<Akateko>` (bajo misma agrupación)
- **Estado**: PRESENTE como encabezado de agrupación ✅

### 3. Apéndice 4 — Tabla de Autodenominaciones (Páginas 244-256)
```
Agrupación: "Q'anjob'al"
Nombre en español: "Q'anjob'al"
Autodenominación: "K'anjob'al"
Familia: "Maya"
```
- Página: ~252 (dentro del rango 244-256)
- Sección: Apéndice 4. Autodenominaciones de las variantes lingüísticas
- **Estado**: PRESENTE ✅ (confirmado en `appendix4_parsed.json`)

### 4. Apéndice 2 — Nomenclatura Comparativa (Página 229)
```
Q'anjob'al | Kanjobal - Kanjobal | K'anjobal | Kanjobal | Kanjobal
```
- Página: 229 (DOF Tercera Sección, p. 85)
- Sección: Apéndice 2. Otra nomenclatura de las agrupaciones lingüísticas
- Tabla comparativa entre instituciones (INALI 2007, INALI 2005, DGEI, CGEIB, DGCPI, CDI, INEGI)
- **Estado**: PRESENTE ✅

### 5. Apéndice 3 — Nomenclatura Histórica (Páginas 233-240)
```
Akateko (familia maya):
  Acateca, acateco, Akateka (ALMG), Akateko (ALMG), kanjobal, kanjobal de San Miguel Acatán...
  ...
Q'anjob'al (familia maya):
  Canjubal, conob, kangobal, kanhobal, kanjobal (INALI 2005; CGEIB; INEGI 2005)...
```
- Páginas: 233-240
- **Estado**: PRESENTE bajo ambas entradas (Akateko y Q'anjob'al como entradas separadas) ✅

### 6. Árboles Genealógicos
- **NO aparece** Q'anjob'al como nodo en los árboles genealógicos de la familia maya
- Los 70 nodos en árboles incluyen 3 extintos (tepecano, ópata, tubar) pero no Q'anjob'al
- **Interpretación**: Q'anjob'al no se representa en el árbol genealógico visual, pero sí tiene entrada completa en índice, catálogo y apéndices

---

## Conteo Unificado Cross-Sectional

| Sección | Agrupaciones | Q'anjob'al | Nota |
|---------|-------------|------------|------|
| **Índice (pp. 11-12)** | **68** | ✅ Entrada #47 | Lista nominal completa |
| **Catálogo Principal (pp. 30-212)** | **68** | ✅ Página 189 | 69 detectados - 1 artefacto "(Viene de la Segunda Sección)" = 68 |
| **Apéndice 2 (p. 229)** | **68** | ✅ Fila en tabla | Tabla comparativa institucional |
| **Apéndice 3 (pp. 233-240)** | **68** | ✅ Entrada propia | Nomenclatura histórica |
| **Apéndice 4 (pp. 244-256)** | **68** | ✅ Fila "Q'anjob'al" | 475 autodenominaciones |
| **Árboles Genealógicos** | **70** | ❌ Ausente | 67 vivas + 3 extintas (†) |

---

## Análisis de la Discrepancia Operación 68

### Causa Raíz: Errores de Parser

| Parser | Error | Impacto |
|--------|-------|---------|
| **Catálogo Principal** | No detectó "Q'anjob'al" como encabezado en p.189 | -1 agrupación |
| **Apéndice 4** | Lógica de parsing omitió fila "Q'anjob'al" | -1 agrupación |
| **Árboles** | Contó 70 nodos (incluye 3 extintas) | Confusión con 68 |

### Verificación Independiente (Operación 69)

**Parser Corregido - Catálogo Principal (pp. 30-212)**:
- Detección: encabezados inmediatamente antes de "AUTODENOMINACIÓN DE LA VARIANTE LINGÜÍSTICA"
- Resultado: **69 detectados**, 1 es artefacto "(Viene de la Segunda Sección)" (p.145)
- **Limpio: 68 agrupaciones reales** ✅

**Parser Corregido - Apéndice 4**:
- Extracción via `extract_words()` con coordenadas X/Y
- Parsing tabular: Autodenominación | Nombre en español | Agrupación | Familia
- Resultado: **68 agrupaciones únicas** (incluye Q'anjob'al y Q'eqchi') ✅

**Concordancia Exacta**:
```
Índice (68) ≡ Catálogo Principal (68) ≡ Apéndice 2 (68) ≡ Apéndice 3 (68) ≡ Apéndice 4 (68)
```

---

## Resolución de Hipótesis

| Hipótesis | Evaluación | Evidencia |
|-----------|------------|-----------|
| **H1**: Q'anjob'al es una de las 68 oficiales y falta en catálogo/Ap.4 | **DESCARTADA** | Está en catálogo (p.189), Ap.2 (p.229), Ap.3 (p.240), Ap.4 (p.252), Índice (p.11) |
| **H2**: Q'anjob'al aparece bajo otra grafía (Kanjobal, Kanjobal) | **PARCIAL** | Ap.2 lista "Kanjobal" como sinónimo histórico; Ap.3 tiene entrada "Q'anjob'al (familia maya)" con formas "kanjobal". Pero en catálogo/índice/Ap.4 usa "Q'anjob'al" consistentemente |
| **H3**: El 68 del índice incluye extintas (tepecano/ópata/tubar) | **DESCARTADA** | Las 3 extintas NO están en índice (pp.11-12), NO en catálogo, NO en apéndices. Solo en árboles con † |
| **H4**: Error de parser/extracción causó el 67 | **CONFIRMADA** | Parser corregido encuentra 68 en catálogo y 68 en Ap.4 |
| **H5**: El CLIN tiene inconsistencia interna irreconciliable | **DESCARTADA** | Todas las secciones canónicas (índice, catálogo, apéndices) concuerdan en 68 |

---

## Explicación Documental del "68"

El CLIN define **68 agrupaciones lingüísticas** como su universo operacional, establecido en 5 ubicaciones independientes:

1. **Página 8**: "las 68 agrupaciones lingüísticas correspondientes a dichas familias"
2. **Página 10**: "Cada una de las 68 agrupaciones lingüísticas aquí catalogadas"
3. **Páginas 11-12**: Lista nominal completa de 68 entradas con familia
4. **Página 231**: "INALI (2007) reconoce 68 agrupaciones"
5. **Página 233**: "cada una de las 68 agrupaciones lingüísticas consideradas en este Catálogo"

Las 68 se enumeran nominalmente en el índice (pp.11-12) y **todas las 68 aparecen** como encabezados en el catálogo principal (pp.30-212) y en el Apéndice 4.

---

## Los 3 Nodos Extintos en Árboles

| Nodo | Familia | En Índice | En Catálogo | En Apéndices | Conclusión |
|------|---------|-----------|-------------|--------------|------------|
| tepecano † | yuto-nahua | ❌ | ❌ | ❌ | Solo en árbol genealógico |
| ópata † | yuto-nahua | ❌ | ❌ | ❌ | Solo en árbol genealógico |
| tubar † | yuto-nahua | ❌ | ❌ | ❌ | Solo en árbol genealógico |

**No cuentan como parte de las 68 oficiales**. El CLIN usa "agrupación lingüística" para el nivel catalográfico intermedio (Familia → Agrupación → Variante), y las 3 extintas son nodos genealógicos históricos, no entradas catalográficas.

---

## Conteo Canónico Final

```
A = Agrupaciones documentadas en secciones canónicas = 68
B = Agrupaciones con variantes vivas en catálogo = 68
C = Agrupaciones extintas en árboles genealógicos = 3
```

**A = B = 68** — Concordancia total demostrada.

---

## Impacto en Estados

| Variable | Antes (Op. 68) | Después (Op. 69) |
|----------|----------------|------------------|
| `GROUPING_COUNT_STATUS` | `UNKNOWN` | **`VERIFIED`** (68) |
| `PHASE_1` | `BLOCKED` | **`UNBLOCKED`** (discrepancia resuelta) |
| `Q'anjob'al` | Missing/Unknown | **Verified in all canonical sections** |

---

## Archivos Generados/Actualizados

1. `docs/audits/operation-69-qanjobal-resolution.md` — Este informe
2. `docs/audits/operation-69-evidence.json` — Evidencia estructurada
3. `docs/audits/operation-69.md` — Informe forense final (patrón Operación 68)

---

## Validaciones Ejecutadas

| Test | Resultado | Evidencia |
|------|-----------|-----------|
| SHA-256 CLIN_completo.pdf | **PASS** | `21cef44abf0d896555f26954bf319340ad4814fa8a3f0719b0d068311ff1be7c` |
| Índice 68 entradas | **PASS** | 68 nombres + familias en pp.11-12 |
| Catálogo principal 68 headers | **PASS** | 69 detectados - 1 artefacto = 68 |
| Apéndice 4 68 agrupaciones | **PASS** | 68 únicas en parsing tabular |
| Q'anjob'al en 5 secciones | **PASS** | Índice, Catálogo, Ap.2, Ap.3, Ap.4 |
| Concordancia 5 secciones | **PASS** | Todas reportan 68 |
| Unicode integrity | **PASS** | Q'anjob'al, K'anjob'al, ku'ahl, ayöök, etc. |
| Sin modificación datos fuente | **PASS** | Solo lectura PDF |

---

## Siguiente Paso Recomendado

**Fase 1 Implementation Closure** (según §23 3b.md):

Ahora que `GROUPING_COUNT_STATUS = VERIFIED` y `PHASE_1 = UNBLOCKED`, proceder con:

1. **Configurar MySQL real** — Connection string (Admin/RenacerGood17) en user-secrets
2. **Aplicar migraciones** — `dotnet ef database update`
3. **Extender modelo provenance** — Entidades `SourceDocument`, `SourceHash`, `SourcePage`, `SourceSection`, `LanguageVariantAutodenomination` (1:N)
4. **Implementar importador transaccional** — Validación 11/68/364/475 con rollback
5. **Ejecutar importación** — RAW → MySQL con reconciliación por identidad
6. **Implementar sync + versionado** — Polling ETag/Last-Modified/SHA256 + backoff + cuarentena
7. **Tests de integración** — Counts, Unicode, hierarchy, provenance, idempotencia

**NO implementar hasta revisión de este reporte.**

---

## Certificación Forense

> **OPERACIÓN 69 = RESOLVED_68**
> 
> La discrepancia 68/67 se resolvió documentalmente. El CLIN contiene 68 agrupaciones lingüísticas en todas sus secciones canónicas. Q'anjob'al está presente en índice (entrada #47), catálogo principal (p.189), Apéndice 2 (p.229), Apéndice 3 (p.240), y Apéndice 4 (p.252). El conteo previo de 67 fue un artefacto de extracción.
> 
> **Fase 1 ya no está bloqueada por discrepancia documental.**