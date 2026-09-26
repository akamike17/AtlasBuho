# ADR 0002 — Fase A · Trazabilidad BD.MD ↔ esquema canónico en producción

**Estado:** PROPUESTO PARA REVISIÓN HUMANA
**Fecha:** 2026-09-25
**Baselínea:** commit `f0c3d92` (build 0/0, unit 68/68, integration 45/45)
**Documento fuente:** `BD.MD` (Diseño Maestro de Base de Datos Lingüística, estado PROPUESTA PARA REVISIÓN HUMANA)
**Regla §62:** BD.MD no autoriza todavía la carga definitiva ni la sustitución automática del catálogo. Este ADR **no crea tablas**, **no modifica EF/Migrations**, **no toca MySQL**. Solo produce trazabilidad para que la revisión humana decida.

---

## 1. Decisión

Para Fase A se adopta la opción **"Mapear BD.MD sobre el modelo existente"** en lugar de **"crear tablas paralelas BD.MD"** o **"sustituir el modelo actual"**. El modelo canónico en producción (LanguageFamily / LanguageGroup / LanguageVariant / Community / Lexeme / Meaning / Source / Evidence / LexicalEquivalence / LanguageVariantAutodenomination / CatalogVersion / CatalogRecord / AiReview) ya está validado (B10/6B cerrado). Cualquier sustitución queda **marcada como fase posterior** condicionada a aprobación.

BD.MD es la **fuente de reglas estructurales**; este ADR documenta dónde cada regla ya se aplica, dónde falta y qué agujero debe llenarse antes de cualquier migración.

---

## 2. Correspondencia de entidades BD.MD → modelo canónico actual

### 2.1 Jerarquía principal (§2-§5, §66)

| BD.MD | Modelo actual | Estado | Diferencia / acción |
|-------|----------------|--------|---------------------|
| FamiliaLinguistica | `LanguageFamily` | PARCIAL | OK Name/Description/Source; falta EstadoVerificacion, FuenteId, FechaCreacion/FechaActualizacion explícitas. PD-A1. |
| AgrupacionLinguistica | `LanguageGroup` | PARCIAL | OK Name/Description/Source; falta EstadoVerificacion, FuenteId. PD-A2. |
| VarianteLinguistica | `LanguageVariant` | PARCIAL | OK Name (=NombreOficial), Iso639_3Code, InaliCode como columnas. BD.MD los exige en tabla separada; falta FuentePrincipalId, EstadoVerificacion. PD-A3. |
| VarianteLocalidad (§15) | — | FALTA | Hoy `VarianteLocalidad` existe solo en el scratch `AtlasBuhoEntities.cs` (Domain/Entities/AtlasBuhoEntities.cs) sin DbSet/Configuration. No hay tabla canónica que conecte variante ↔ localidad directo. PD-A4. |
| Poblacion (§11) | `Community` | PARCIAL | Community cubre Localidad+Estado+Municipio+Population, pero mezcla localidad y población en una entidad (BD.MD §10 prohíbe esto). PD-A5. |
| Localidad / Municipio / Estado / Pais (§13) | incrustado en `Community.State`, `Municipality`, `Locality` (string) | FALTA | No hay tabla geográfica normalizada. Hoy Region existe pero es otra cosa. PD-A6. |
| VariantePoblacion (§12) | FK directa `Community.LanguageVariantId` | PARCIAL | M:N no expresado: una comunidad solo apunta a una variante. BD.MD exige tabla intermedia. PD-A7. |
| PoblacionLocalidad (§14) | incrustado en `Community` | FALTA | No hay tabla intermedia. PD-A8. |

### 2.2 Identificación y códigos (§9, §37, §38)

| BD.MD | Modelo actual | Estado | Acción |
|-------|----------------|--------|--------|
| CodigoLinguistico (table separada) | `LanguageVariant.Iso639_3Code` y `.InaliCode` como columnas | DIVERGE | BD.MD §9: tabla CodigoLinguistico + Sistema enum, nunca columna. PD-A9. |
| FuenteDocumental (§16) | `Source` (tabla existente) | OK-ish | Cubre BD.MD §16, pero falta `Tipo` enum controlado (Source.Tipo actualmente libre). PD-A10. |
| EvidenciaFuente (§17) | `Evidence` + `CatalogRecord` (con Quote, PageReference, SectionReference, SourceHash) | PARCIAL | La cobertura varía por entidad; `CatalogRecord` la tiene completa para el catalog INALI. PD-A11. |

### 2.3 Léxico y traducción (§23-§28, §30)

| BD.MD | Modelo actual | Estado | Acción |
|-------|----------------|--------|--------|
| Lexema (§23) | `Lexeme` | OK | ADR 0001 ya cubre canonical form, variant pinning y verification status. |
| Significado (§24) | `Meaning` | OK | M:N con lexema está implícito. |
| Traduccion (§25) | `LexicalEquivalence` (ADR 0001) | OK | Política I2: una equivalencia = una dirección evidenciada, nunca inferida. |
| EquivalenciaLexica (§26) | `LexicalEquivalence` con `IsCanonical` | OK | Regla I1 en vivo: max 1 canónico por (source, target, version). |
| Frase + FraseTraduccion (§29, §30) | `Phrase` + `PhraseLexeme` | PARCIAL | Sin tabla PhraseTraduccion; hoy las frases se componen de lexemas. PD-A12. |
| Pronunciacion (§27) | `Pronunciation` (tabla propia) | OK | |
| RecursoAudio (§28) | `AudioRecording` | OK | |

### 2.4 Versionado y reconciliación (§21, §32-§36)

| BD.MD | Modelo actual | Estado | Acción |
|-------|----------------|--------|--------|
| RegistroVersion (§21) | `CatalogRecord` + `CatalogRecordStatus` | OK | Funciona como ledger versión → estado. |
| ImportBatch (§32) | `CatalogVersion` | PARCIAL | Cubre Created/Parssed/Completed pero no los estados intermedios REVIEW_REQUIRED / RECONCILED / COMMITTED / REJECTED. PD-A13. |
| ImportRecord (§33) | `ImportQuarantine` | PARCIAL | `ImportQuarantine` cubre los rechazados pero no tipifica los aceptados por separado. PD-A14. |
| ReconciliacionCatalogo (§35), regla 364/364 (§36) | `CatalogRecord.Status` ∈ {Current, Duplicate, Quarantine, Missing, Variant} | PARCIAL | Aproximado pero BD.MD exige MATCH / MISSING / DUPLICATE / CONFLICT / UNRESOLVED / NOT_APPLICABLE como enum separado. PD-A15. |
| Auditoria (§22) | — | FALTA | No hay tabla de auditoría. BD.MD §22 lo exige. PD-A16. |
| CorrespondenciaFuente (§48) | — | FALTA | No existe. PD-A17. |

### 2.5 IA (§31)

| BD.MD | Modelo actual | Estado | Acción |
|-------|----------------|--------|--------|
| AiSuggestion | `AiTranslationReview` + `V2Candidate` | OK | ADR 0001 + B10 separation: AI suggestion ≠ canonical, lifecycle PENDING→REVIEW→ACCEPTED/REJECTED→PROMOTED, sin FK a canónicas. |

### 2.6 EstadoVerificacion (§19, §42)

| BD.MD | Modelo actual | Estado | Acción |
|-------|----------------|--------|--------|
| EstadoVerificacion como campo obligatorio en cada tabla crítica | solo `Lexeme.VerificationStatus` y `LexicalEquivalence.VerificationStatus`; no propagado a Family/Group/Variant/Community/Source/Evidence | FALTA GLOBAL | PD-A18: no es satisfactorio cumplir §19 mientras la entidad canónica no lo tenga. |
| NULL vs UNKNOWN distinction (§42) | enum tiene `Unknown` además de los de BD.MD | DIVERGE semántico | ADR 0001 usa Unknown = no-verificado, en BD.MD Unknown ≠ Unverified. PD-A19: alinear nombres de enum o documentar explícitamente la desviación. |

---

## 3. Agujeros detectados (Pendientes de Diseño)

| ID | Tema | Descripción | Requerirá antes de cerrar Fase B |
|----|-------|-------------|-------------------------------|
| PD-A1 | FamiliaLinguistica.EstadoVerificacion | falta en LanguageFamily | Sí |
| PD-A2 | AgrupacionLinguistica.EstadoVerificacion | falta en LanguageGroup | Sí |
| PD-A3 | VarianteLinguistica.FuentePrincipalId + EstadoVerificacion | falta en LanguageVariant | Sí |
| PD-A4 | VarianteLocalidad directa | falta tabla | Sí (geografía) |
| PD-A5 | Separación Población ≠ Localidad | hoy mezclada en Community | Sí |
| PD-A6 | Tabla geográfica normalizada (Pais/Estado/Municipio/Localidad) | falta | Sí (geografía) |
| PD-A7 | VariantePoblacion (M:N) | hoy 1:N | Sí |
| PD-A8 | PoblacionLocalidad (M:N) | falta | Sí |
| PD-A9 | CodigoLinguistico como tabla separada | hoy columnas Iso639_3Code/InaliCode en variant | Sí |
| PD-A10 | Source.Tipo enum controlado (§16 list) | hoy libre | Sí |
| PD-A11 | EvidenciaFuente como tabla explícita separada de Evidence | hoy solo Evidence | Opcional si se argumenta doble-propósito |
| PD-A12 | PhraseTraduccion | falta | Fase E |
| PD-A13 | Estados ImportBatch intermedios (REVIEW_REQUIRED, RECONCILED, COMMITTED, REJECTED) | hoy reducidos | Sí (Fase B) |
| PD-A14 | ImportRecord como entidad separada (RAW antes de aceptar/rechazar) | hoy solo Quarantine | Sí (Fase B) |
| PD-A15 | ReconciliacionCatalogo.Result enum nomenclatura BD.MD | hoy CatalogRecordStatus | Sí |
| PD-A16 | Auditoria como tabla | falta | Sí (Fase A/B) |
| PD-A17 | CorrespondenciaFuente | falta | Fase C |
| PD-A18 | EstadoVerificacion propagado a Family/Group/Variant/Community/Source/Evidence | falta global | Sí |
| PD-A19 | Unknown vs Unverified semántica | diverge | Sí (decisión documentada) |

---

## 4. Lo que NO se hace en este ADR

- No se crea ninguna tabla, ningún DbSet, ninguna configuración EF, ninguna migración.
- No se renombra ninguna entidad existente (LanguageFamily/LanguageGroup/etc. quedan).
- No se toca `AtlasBuhoEntities.cs` (scratch) ni el modelo sellado.
- No se ejecuta ningún `dotnet ef` ni `mysql`.
- No se han aprobado aún los cambios de PD-A1..A19 — la revisión humana decide.

## 5. Criterio para cerrar Fase A

Fase A puede declararse COMPLETE solo cuando:

1. Este ADR ha sido revisado y aprobado.
2. Cada PD-A{1..19} tiene una resolución explícita: o tabla nueva mapeada a una migración concreta, o argumento documental de por qué NO se implementa (con referencia al texto BD.MD).
3. Ninguna migración ha sido aplicada todavía (esto dispara Fase B).
4. Build 0/0 + tests unit 68/68 + integration 45/45 siguen verdes (este ADR no introduce código).

## 6. Referencias

- BD.MD §2-§66.
- ADR 0001 (LexicalEquivalence / I1 / I2 / I3).
- Skill `atlasbuho-surgical-md` — reglas de integridad del catálogo.
- ` src/AtlasBuho.Domain/Entities/{LanguageFamily,LanguageGroup,LanguageVariant,LanguageVariantAutodenomination,Lexeme,Meaning,Source,Evidence,LexicalEquivalence,CatalogVersion,CatalogRecord,ImportQuarantine,Community,Region,Phrase,PhraseLexeme,Pronunciation,AudioRecording}.cs`.
- `src/AtlasBuho.Data/AtlasBuhoDbContext.cs` (35 DbSets).
- Commit base: `f0c3d92`.
