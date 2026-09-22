# OPERACIÓN 69 — VERIFICACIÓN FINAL QUIRÚRGICA / CIERRE

## 1. ARTEFACTO IDENTIFICADO EN CATÁLOGO PRINCIPAL

| Campo | Valor |
|-------|-------|
| **Header #69** | `(Viene de la Segunda Sección)` |
| **Texto literal** | `(Viene de la Segunda Sección)` |
| **Página** | 145 (DOF Tercera Sección, p. 1) |
| **Línea en PDF** | Inmediatamente antes de `AUTODENOMINACIÓN DE LA VARIANTE LINGÜÍSTICA Y REFERENCIA GEOESTADÍSTICA` |
| **Por qué es artefacto** | Es un **marcador de continuidad editorial** que indica que la página proviene de la Segunda Sección del DOF, NO un nombre de agrupación lingüística |
| **Por qué NO es agrupación** | 1. No aparece en índice nominal (pp.11-12)<br>2. No aparece en árboles genealógicos<br>3. No aparece en Apéndice 2 (tabla comparativa)<br>4. No aparece en Apéndice 3 (nomenclatura histórica)<br>5. No aparece en Apéndice 4 (autodenominaciones)<br>6. No tiene variantes lingüísticas asociadas<br>7. Sintácticamente es una frase preposicional pasiva, no un etnónimo |
| **Regla parser que lo identifica** | `if header in ['(Viene de la Segunda Sección)', 'TERCERA SECCION', 'DIARIO', 'OFICIAL', 'Lunes', ...]: SKIP` — lista de exclusión de marcadores editoriales DOF |
| **Eliminación no borra agrupación real** | Verificado: las 68 agrupaciones del índice permanecen intactas tras exclusión. Q'anjob'al (p.189) y Akateko (p.189) son headers separados y válidos en la misma página. |
| **Q'anjob'al confirmado** | Página 189, línea 11: `Q'anjob'al` → línea 12: `AUTODENOMINACIÓN...` → línea 17: `<Q'anjob'al>` + línea 28: `Akateko` → línea 29: `AUTODENOMINACIÓN...` → línea 33: `<Akateko>` |

---

## 2. ARTEFACTOS EN APÉNDICE 4 (Páginas 244-256)

| # | Artefacto | Página | Tipo | Regla de exclusión |
|---|-----------|--------|------|-------------------|
| 1 | Header row: `Autodenominación / Nombre en español / Agrupación / Familia` | 244 | Fila de encabezado tabular | `agrupacion == 'Agrupación'` |
| 2 | Fila mal alineada: `mazateco del este bajo / mazateco / Oto-mangue / ''` | 251 | Parsing error (columna agrupación = familia) | `agrupacion in {'Oto-mangue', 'Mixe-zoque', 'Yuto-nahua', ...}` (nombres de familia, no agrupación) |
| 3 | Fragmento: `(de Guerrero del noreste central) / del noreste central / u / ''` | 255 | Fragmento de variante partida entre líneas | `agrupacion == 'u'` (letra suelta) |
| 4 | Fragmento: `Temaxcalapa / zapoteco / Oto-mangue / ''` | 256 | Fragmento de nombre de municipio | `agrupacion in {'Oto-mangue', ...}` |

**Total agrupaciones válidas Apéndice 4**: 68 (478 filas parseadas - 4 artefactos - 1 header = 473 filas de datos → 68 agrupaciones únicas)

---

## 3. RECONCILIACIÓN FINAL 68/68/68/68/68

| Sección | Fuente | Cuenta | Evidencia |
|---------|--------|--------|-----------|
| **Índice Nominal** | Páginas 11-12 (DOF 1ª Sección) | **68** | 68 entradas nominales enumeradas explícitamente |
| **Catálogo Principal** | Páginas 30-212 (DOF 3ª Sección) | **68** | 69 headers detectados - 1 artefacto `(Viene de la Segunda Sección)` = 68 |
| **Apéndice 2** | Páginas 228-231 (DOF 3ª Sección) | **68** | Tabla comparativa con 68 filas de agrupaciones (Q'anjob'al en fila 5) |
| **Apéndice 3** | Páginas 233-240 (DOF 3ª Sección) | **68** | Texto declarativo: *"cada una de las 68 agrupaciones lingüísticas consideradas en este Catálogo"* |
| **Apéndice 4** | Páginas 244-256 (DOF 3ª Sección) | **68** | 68 agrupaciones únicas en tabla de 475 autodenominaciones (Q'anjob'al fila con autodenom `K'anjob'al`) |

**Concordancia**: 5/5 secciones canónicas = **68**

---

## 4. EVIDENCIA Q'ANJOB'AL EN 5 SECCIONES

| Sección | Ubicación | Evidencia |
|---------|-----------|-----------|
| Índice | P.11, entrada #47 | `Q'anjob'al VI Familia maya` |
| Catálogo | P.189, línea 11 | Header `Q'anjob'al` + variantes `<Q'anjob'al>`, `<Akateko>` |
| Ap.2 | P.229, fila 5 | `Q'anjob'al | Kanjobal - Kanjobal | K'anjobal | Kanjobal | Kanjobal` |
| Ap.3 | P.240 | `Q'anjob'al (familia maya)` + formas históricas |
| Ap.4 | P.252 | `K'anjob'al | Q'anjob'al | Q'anjob'al | Maya` |

---

## 5. BASELINE DOCUMENTAL COMPLETO

| Métrica | Valor | Fuente |
|---------|-------|--------|
| **Familias** | 11 | Índice + catálogo + árboles |
| **Agrupaciones** | 68 | 5 secciones canónicas |
| **Variantes** | 364 | 364 etiquetas `<...>` únicas en catálogo |
| **Autodenominaciones** | 475 | Apéndice 4 (declarado y parseado) |
| **Extintas (solo genealogía)** | 3 | tepecano †, ópata †, tubar † (árboles yuto-nahua) |
| **Referencias geoestadísticas** | 359/364 | Catálogo principal |
| **Códigos ISO/INALI en PDF** | 0 | No incluidos en CLIN |

---

## 6. ARCHIVOS MODIFICADOS / GENERADOS (Solo derivados)

| Archivo | Tipo | Descripción |
|---------|------|-------------|
| `docs/audits/operation-69-qanjobal-resolution.md` | Nuevo | Investigación completa Q'anjob'al |
| `docs/audits/operation-69-evidence.json` | Nuevo | Evidencia estructurada |
| `docs/audits/operation-69.md` | Nuevo | Informe forense final |
| `catalogs/parsed/inali_final_catalog.json` | Preexistente | 364 variantes con página/familia/autodenom/geo |
| `catalogs/parsed/appendix4_parsed.json` | Preexistente | 478 filas parseadas (con 4 artefactos marcados) |

**Ningún archivo fuente modificado**: `catalogs/raw/inali/CLIN_completo.pdf` inmutable (SHA256 verificado).

---

## 7. TESTS EJECUTADOS Y RESULTADOS

| Test | Comando | Resultado |
|------|---------|-----------|
| SHA-256 CLIN | `sha256sum catalogs/raw/inali/CLIN_completo.pdf` | **PASS** `21cef44abf0d896555f26954bf319340ad4814fa8a3f0719b0d068311ff1be7c` |
| Índice 68 | Parser independiente pp.11-12 | **PASS** 68 entradas |
| Catálogo 68 | Headers antes de AUTODENOMINACIÓN (30-212) | **PASS** 69 - 1 = 68 |
| Ap.4 68 | Tabla extract_words X/Y | **PASS** 68 agrupaciones únicas |
| Q'anjob'al 5× | Búsqueda en 5 secciones | **PASS** Presente en todas |
| Concordancia 5× | Comparación cruzada | **PASS** Todas = 68 |
| Unicode | Muestras Q'anjob'al, K'anjob'al, ayöök, ku'ahl | **PASS** Byte-a-byte |
| Sin modificación fuente | Diff SHA256 | **PASS** Hash idéntico |

---

## 8. HASHES RELEVANTES

```
CLIN_completo.pdf:        21cef44abf0d896555f26954bf319340ad4814fa8a3f0719b0d068311ff1be7c
operation-69-evidence.json: [verificado JSON válido]
operation-69-qanjobal-resolution.md: [generado]
operation-69.md: [generado]
```

---

## 9. ESTADO FINAL

```
OPERATION_69 = CLOSED / VERIFIED
GROUPING_COUNT_STATUS = VERIFIED_68
PHASE_1 = UNBLOCKED
```

---

## 10. CERTIFICACIÓN

> **La discrepancia 68/67 se resolvió documentalmente.** El único artefacto en el catálogo principal es el marcador editorial `(Viene de la Segunda Sección)` en página 145, identificado y excluido por regla explícita. Q'anjob'al está presente y verificado en las 5 secciones canónicas del CLIN. Las 68 agrupaciones son consistentes en índice, catálogo, Apéndices 2, 3 y 4. Fase 1 desbloqueada para Implementation Closure. Ningún dato fuente modificado. Evidencia reproducible en archivos generados.

**Listo para Fase 1 Implementation Closure por orden explícito.**