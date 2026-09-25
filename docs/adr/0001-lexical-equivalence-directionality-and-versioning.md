# ADR 0001: Direccionalidad explícita, LexicalEquivalence y versionado reproducible del corpus léxico

**Fecha:** 2026-09-24
**Estado:** ACEPTADA (decisión de productor/usuarios previa a codificar; ver instrucción "primero un ADR corto y luego la implementación")
**Base:** `b9b8057` — Phase 2 Translator Core endurecido (P0 variant isolation, P1 API semantics)
**Rama:** `review/phase-1-inventario-linguistico`
**Relacionados:** 6B.md §4–§21, §37–§40; decisiones previas en `docs/audits/phase-2-translator-core.md`

---

## 1. Contexto

El motor `dictionary-1.1` resuelve traducciones léxicas directamente contra `Lexeme.SpanishMeaning`
y `Meaning.EnglishMeaning`. Cuatro defectos estructurales identificados por el revisor lo hacen
inviable para un corpus verificado antes de la ingesta masiva:

1. **Direccionalidad implícita.** La forma canónica y `SpanishMeaning` están acopladas dentro del
   propio Lexeme; no existe un lugar donde A→B sea evidencia *explícita*, por lo que nada impide
   que el motor "traduzca en reversa" por accidente estructural.
2. **Proveniencia aplastada.** Un `Distinct()` sobre el texto destino elimina fuentes: dos
   referencias distintas al mismo "sol" colapsan y se pierde evidencia.
3. **"Primaria por orden alfabético".** `OrderBy(Text).First()` solo da orden determinista; NO es
   autoridad lingüística.
4. **Dataset no reproducible.** `DatasetVersion = último CatalogVersionId Completed`, pero los
   Lexemes no están ligados a esa versión; una mutación silenciosa de un Lexeme cambia el resultado
   sin cambiar la versión.

## 2. Decisión

Introducir una entidad de evidencia de equivalencia léxica de primer orden, separada del Lexeme
(el Lexeme describe la FORMA en una variante; la equivalencia describe una evidencia de traducción).

### Entidad: `LexicalEquivalence` (una fila = UNA evidencia)

| Campo | Rol |
|---|---|
| `Id` | PK |
| `SourceLexemeId` | FK → Lexemes (la forma en su variante) |
| `TargetLanguage` | `"es"` / `"en"` — código corto estable; jamás nombre de display |
| `TargetText` | La traducción evidenciada; no colapsada |
| `IsCanonical` | bool; **máximo UNA canónica por (SourceLexemeId, TargetLanguage, CatalogVersionId)** |
| `VerificationStatus` | Verified / Documented / CommunityVerified / AcademicVerified … |
| `SourceId` | FK → Sources (documento/obra origen de la evidencia) |
| `CatalogVersionId` | FK → CatalogVersions; versión inmutable del corpus |
| `CreatedAt` / `RowHash` | integridad y detección de mutación |

`Distinct()` sobre TargetText SOLO se usa en la capa de presentación. La tabla conserva cada
fila de evidencia: "sol"/fuente A y "sol"/fuente B son dos filas distintas.

### Invariantes (duras, no de diseño blando)

I1 — Máximo una canónica por (SourceLexemeId, TargetLanguage, CatalogVersionId). DOS canónicas
para ese triplete es una **violación de integridad**, nunca se "resuelve" ordenando por texto.

I2 — Direccionalidad explícita. Indígena→ES y ES→Indígena requieren cada una sus propias filas
de evidencia. Ninguna equivalencia se INFIERE invirtiendo la dirección.

I2b — Reverse-direction ISO ATOMICS (resolución definitiva — implementada en dictionary-2.0).
La evidencia bidireccional existe: en la dirección reversa (es/en→indigena), la fila explícita
de LexicalEquivalence tiene TargetText = término es/en y el lexeme indígena es el destino. El
engine consulta LexicalEquivalences por TargetText + TargetLanguage (nunca SpanishMeaning como
autoridad) e identifica el resultado por el CanonicalForm del lexeme indígena.

- FORWARD (indígena → es/en): LexicalEquivalence con TargetLanguage ∈ {es,en} y
  SourceLexemeId = lexeme indígena → Translation = TargetText.
- REVERSE (es/en → indigena): LexicalEquivalence con TargetText = término de consulta,
  TargetLanguage ∈ {es,en}, y SourceLexemeId.LanguageVariantId = targetLanguageVariantId →
  Translation = SourceLexeme.CanonicalForm.
- Aún sin evidencia propia de la dirección solicitada: NotFound (never a guess).

P0 — Initially stored: every translation query uses the resolved CatalogVersionId
(latest Completed) as a physical filter — no evidence is used from another version.

I3 — Sin mutaciones al corpus completado. Una `CatalogVersion` con `ImportStatus = Completed`
es una fuente inmutable; nueva evidencia ⇒ nueva versión (nuevo `CatalogVersionId`).

P1 — Trigger reproducibility: database constraints are installed via migration
`20260925033847_LexicalEquivalenceModel` and applied to MySQL real (triggers
`trg_catalogversions_prevent_update_completed`, `trg_catalogversions_prevent_delete_completed`,
`trg_lexeq_block_mutation_completed` — verifiable via `SHOW TRIGGERS`).

I4 — Aislamiento por variante heredado del P0: toda equivalencia vive sobre el lexema de UNA
variante (`LanguageVariantId` resuelto inequívocamente por el engine); jamás se consulta el
mismo textualmente en variantes diferentes.

### Semántica del motor `dictionary-2.0`

- Resuelve el `(source, target)` a identidades estables; 0/ambiguo ⇒ `UnsupportedLanguage`.
- Candidatos = LexicalEquivalence del `SourceLexemeId` resuelto + `TargetLanguage` +
  `CatalogVersionId` corriente, solo VerificationStatus verificada.
- `Translation` (el "primary"):
  - 0 canónicas ⇒ `null` (`NotFound` si tampoco hay evidencia).
  - 1 canónica ⇒ esa.
  - ≥2 canónicas ⇒ `AndIntegrityViolationException` — NUNCA se desambigúa.
- `Alternatives` = cada evidencia verificada (texto + IsCanonical + VerificationStatus + SourceId).
  La presentación puede deduplicar texto; la API nunca lo colapsa.

## 3. Alternativas descartadas

- **Mantener Lexeme.SpanishMeaning como la traducción.** Mezcla forma en origen con evidencia en
  destino; ya descartada por P0/isolación y por direccionalidad.
- **Una tabla Translation sin discriminador de versión.** Mantiene el fallo de reproducibilidad.
- **Primary = orden alfabético.** Explícitamente rechazado: orden determinista ≠ autoridad.

## 4. Impacto

- Entidad + configuración EF Core con índice único parcial (`IsCanonical`) por
  (SourceLexemeId, TargetLanguage, CatalogVersionId). En MySQL/Pomelo el índice parcial se
  materializa como índice único compuesto sobre una columna generada, verificado en migración.
- `dictionary-2.0` reemplaza a `1.1`; el contrato de `TranslationResult` no cambia en forma, solo
  en cómo se rellena `Translation`/`Alternatives`.
- Phase 1 (474 reconciliation) NO se toca.

## 5. Evidencia que haría cerrar la fase (se reproduce en implementación)

- `dotnet build` 0/0 Release; suite completa verde.
- Tests golden: P0 cross-variante, Ind→ES, Ind→EN, reversa con y sin evidencia, dos fuentes con
  mismo target, multi-target, exactamente una canónica, cero canónicas, y **dos canónicas ⇒
  violación detectada** a nivel entidad + configuración.
- Migración aplicada a MySQL real de Phase 1; índice único de canonicidad verificado con
  `SHOW INDEX`; counts Phase 1 intactos.
