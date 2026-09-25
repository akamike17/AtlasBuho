# 6B.md — PHASE 2 START: TRANSLATOR CORE (DictionaryTranslationEngine)

**Fecha:** 2026-09-24
**Rama:** `review/phase-1-inventario-linguistico`
**Commit base:** b47e668

## Alcance de este commit

Implementación del núcleo de traducción de 6B.md sobre la Phase 1 cerrada, sin tocar el
importer ni el modelo de reconciliación 474:

- **6B.md §3 (AI optional/adapter):** `ITranslationEngine` en `AtlasBuho.Application.Translation`.
  El núcleo depende de la interfaz, nunca de un proveedor IA. `EngineVersion` es parte del
  contrato de reproducción (`dictionary-1.0`).
- **6B.md §21 (resultado estructurado):** `TranslationResult` con `Status` /
  `MatchType` / `VerificationStatus` / `DatasetVersion` / `EngineVersion` / `Alternatives`
  separados. Estados: Translated, NotFound, PendingVerification, UnsupportedLanguage, InvalidInput.
- **6B.md §4/§5/§6 (verified-only, deterministic, no silent winner):**
  `DictionaryTranslationEngine` (en `AtlasBuho.Data.Translation`, única ruta sin ciclos de
  dependencia: Data→Application ya existe) resuelve únicamente lexemas con
  VerificationStatus verificada (Verified/Documented/CommunityVerified/AcademicVerified),
  con dedup y orden ordinal determinista (in-memory, nunca orden de base de datos). Un empate
  verificado se expone como lista de alternativas, jamás se elige arbitrariamente.
- **6B.md §14 (direccionalidad):** Ind→Es y Es→Ind usan evidencia estrictamente direccional
  (`SpanishMeaning`); Ind→En usa `Meaning.EnglishMeaning` verificada. Nunca se invierte sin
  evidencia (test `Translate_SpanishToIndigenous_UsesOnlySpanishEvidence_NotReverseGuess`).
- **6B.md §19 (unknown first-class):** sin coincidencia verificada → `NotFound`, nunca una
  traducción inventada (Invariant 6 demostrado en test).
- **6B.md §37 (dataset version):** resultados llevan el `VersionNumber` del último
  CatalogVersion `Completed` (nunca Pending/Failed, reforzando 5B FASE 12).

## Evidencia

### Build

```
dotnet build AtlasBuho.slnx -c Release  →  0 Advertencias, 0 Errores
```

### Tests (6B.md §45/§46)

8 tests nuevos en `tests/AtlasBuho.Tests/DictionaryTranslationEngineTests.cs`:
unknown-word NotFound · unique verified match · unverified never translated ·
multi-verified alternatives deterministas · reversa sin evidencia → NotFound ·
reversa con evidencia resuelve · input vacío InvalidInput · idioma desconocido UnsupportedLanguage.

```
dotnet test AtlasBuho.slnx -c Release  →  62 totales: 61 correctas, 1 skip (W1HContext preexistente), 0 error
  DictionaryTranslationEngineTests: 8/8 ✅
```

### Integración MySQL real (DB Phase-1 `atlasbuho_5b_1790266227`)

Modo `--translate` en ImportRunner dogfoodeado contra los 474 datos verificados:

```
engine=DictionaryTranslationEngine (dictionary-1.0), datasetVersion=CA44013F9399
zapoteco serrano → español "bene xono"        → Status=NotFound (NULL translation)
klingon → español "hola"                      → Status=UnsupportedLanguage
español → zapoteco serrano "zorro"            → Status=NotFound
```

Comportamiento CORRECTO: Phase 1 no importa lexemas (Lexemes = 0 en la BD), por lo que la
única respuesta honesta posible es NotFound — el motor nunca inventa (§19/Invariant 6).
Traducciones reales requerirán una fase de ingesta léxica (Lexeme/Meaning), que es Phase 2
fuera del alcance de este commit.

## Limitaciones declaradas (no sobreafirmadas)
- English→Indigenous devuelve NotFound: el dataset no registra evidencia de esa dirección
  (el motor no encadena pivotes invisibles — 6B.md §16).
- La diacritics/SearchKey (§27) y la capa UI (§23–§36) son trabajo posterior.

## Git

```
branch:  review/phase-1-inventario-linguistico
base:    b47e668
commit:  feat: 6B translator core - deterministic verified dictionary engine (dictionary-1.0)
```

---

# 6B.md — FOLLOW-UP: WEB BOUNDARY FIX + TRANSLATE API REAL

**Fecha:** 2026-09-24
**Commit base:** 46179dd

## Root cause del fallo del website raíz (verificado)

`AtlasBuho.csproj` está en la RAÍZ del repositorio (con `src/`, `tests/`, `catalogs/` como
hermanos). El SDK `Microsoft.NET.Sdk.Web` aplica default globs (`**/*.cs`), por lo que el
website compilaba `tests/AtlasBuho.Tests/**/*.cs` (sin referencias a xUnit — CS0246
FactAttribute ×169) y las migraciones de `src/AtlasBuho.Data/` (que requieren
`BuildTargetFrameworkAttribute` del proyecto Data). 169 errores preexistentes; ninguno
introducido por este trabajo. El fix NO movió ni renombró ningún archivo.

## Fix quirúrgico ( AtlasBuho.csproj )

```xml
<EnableDefaultCompileItems>false</EnableDefaultCompileItems>
<EnableDefaultContentItems>false</EnableDefaultContentItems>
<UserSecretsId>AtlasBuho</UserSecretsId>
+ Compile Include = Program.cs + Controllers/** + Models/**
+ Content Include  = Views/** wwwroot/** appsettings
+ Content Remove   = Views/**/*.cshtml.css
+ ProjectReference = src/AtlasBuho.Data (Pomelo MySql)
```

## Evidencia

### Build

```
dotnet build AtlasBuho.slnx -c Release  →  0 Advertencias, 0 Errores
dotnet build AtlasBuho.csproj -c Release → 0 Advertencias, 0 Errores   (website raíz compila)
```

### Tests (sin regresiones)

```
dotnet test AtlasBuho.slnx -c Release  →  62 totales: 61 correctas, 1 skip (W1HContext preexistente), 0 error
```

### API REST real contra MySQL Phase-1

Levantado `bin/Release/net8.0/AtlasBuho.exe` (env=Development, UserSecrets `AtlasBuho`) y
probado con `POST /api/translate`:

```text
IND zapoteco serrano -> español "bene xono"
  -> HTTP 422 UnsupportedLanguage  (variante "zapoteco serrano" no existe en el dataset;
     resultado correcto y honesto: no hay evidencia, no se inventa)
klingon -> español "hola"
  -> HTTP 422 UnsupportedLanguage
input vacío
  -> HTTP 400 InvalidInput
zapoteco de Asunción Tlacolulita -> español "bene xono"
  -> HTTP 200 NotFound   (variante documentaria EXISTE en Phase 1, pero Lexemes=0 en la BD,
     por lo que la respuesta honesta es NotFound con datasetVersion=CA44013F9399)
```

Secuencia real de SQL observada en logs EF Core sobre MySQL Phase-1:
`SELECT EXISTS ... FROM LanguageVariants`; `SELECT VersionNumber FROM CatalogVersions WHERE
ImportStatus='Completed'`; `SELECT SpanishMeaning, VerificationStatus FROM Lexemes WHERE
CanonicalForm=… AND VerificationStatus IN (1,2,3,4)`. Reproducible.

Queda fuera (trabajo posterior): ingesta de Lexemes → traducciones reales; capa UI;
casos Golden (§46).

---

# FOLLOW-UP 2: P0 VARIANT ISOLATION + P1 API ERROR SEMANTICS

**Fecha:** 2026-09-24 · base: ce7ad83

## P0 — aislamiento por variante (engine 1.1)

`DictionaryTranslationEngine` consultaba lexemas SOLO por `CanonicalForm == term`, sin
`LanguageVariantId`. La misma forma en dos variantes es un hecho lingüístico distinto (6B.md
§9) y la consulta sin aislamiento mezclaba significados. Corregido: el motor resuelve el
nombre de idioma/variante a UN LanguageVariantId estable (0/2+ nombres ⇒ UnsupportedLanguage,
jamás pick arbitrario), y toda consulta léxica queda restringida a esa identidad.
EngineVersion bumped a `dictionary-1.1`.

## P1 — semántica del error de engine no configurado

Antes: `request==null || _engine==null` → 400 InvalidInput, confundiendo input malformado
con infraestructura no lista. Ahora:
- `request == null` → 400 InvalidInput
- `_engine == null` (sin connection string / sin DI) → 503 ServiceUnavailable

## Evidencia

Build slnx + csproj raíz: 0/0 · tests 64 totales, 63 OK, 1 skip preexistente
(DictionaryTranslationEngineTests 10/10 con el nuevo caso same-form-two-variants y
ambiguous-name-UnsupportedLanguage).

MySQL real (AtlasBuho Phase-1, dataset CA44013F9399): seed de `Lexemes` con la MISMA forma
`ra-p0` en `zapoteco de Asunción Tlacolulita` (significado A) y `zapoteco de la costa este`
(significado B), ambos VerificationStatus=Verified:

```
A ra-p0  → 200 Translated "significado A" (engine dictionary-1.1)
B ra-p0  → 200 Translated "significado B"
reverse español → A, sin evidencia direccional → 200 NotFound

SQL observado (EF Core): SELECT ... WHERE LanguageVariantId=@variantId AND
CanonicalForm=@term AND VerificationStatus IN (1,2,3,4)
```

Tras la prueba los lexemas de test fueron eliminados (`Lexemes` queda de nuevo 0).


---

# FOLLOW-UP 3: 503 BODY SEMANTIC CONTRACT + CONTROLLER TESTS

**Fecha:** 2026-09-24 · base: aa83ef0

## Inconsistencia detectada (P1 cola)

HTTP 503 llevaba `TranslationStatus.UnsupportedLanguage` en el body — mezcla de nivel
infraestructura con nivel lingüístico. Corrección: nuevo `TranslationStatus.ServiceUnavailable`
(6B.md §21 separa estados; §42 valida estados inválidos) usado EXCLUSIVAMENTE por la capa API
cuando el engine no está configurado; el contrato de engine nunca lo produce.

```
request == null  -> 400 InvalidInput
_engine == null  -> 503 ServiceUnavailable (body.status = ServiceUnavailable)
engine presente, supported + no match -> 200 NotFound
engine presente, unsupported lang    -> 422 UnsupportedLanguage
```

## Tests de controller (6B.md §45 API tests)

`tests/AtlasBuho.Tests/TranslateControllerTests.cs`:
- `Post_NullRequest_Returns400_InvalidInput`
- `Post_NullEngine_Returns503_ServiceUnavailable_NotUnsupportedLanguage`
- `Post_ValidRequest_WithEngine_Returns200`

Para referenciar el controller (proyecto raíz web) sin hostear: `FrameworkReference
Microsoft.AspNetCore.App` + `ProjectReference ..\..\AtlasBuho.csproj` en el test csproj.

Suite: 67 totales, 66 OK, 1 skip preexistente, 0 errores. Build slnx + csproj raíz: 0/0.

## Aviso explícito a la siguiente iteración — NO iniciar ingesta masiva de Lexemes

Antes de corpus ingestion quedan por blindar (registrados por el usuario):

1. Direccionalidad en el MODELO de datos (A→B no implica B→A; nada de generar reversos en
   ingesta).
2. Proveniencia por traducción: dos fuentes con el mismo target text NUNCA se deduplican en
   evidencia; dedup solo a nivel de PRESENTACIÓN.
3. Primary vs alternative: el orden alfabético determinista NO es autoridad lingüística;
   el corpus debe modelar cuál es la traducción canónica por evidencia, o declararlas todas
   alternatives sin privilegiar una.
4. Dataset/corpus reproducible: hoy `DatasetVersion = último CatalogVersion Completed` es una
   aproximación; los Lexemes deben ligarse a una versión/corpus inmutable, o el triplete
   (dataset, engine, input) deja de ser reproducible tras una mutación silenciosa.


---

# FOLLOW-UP 4: ADR 0001 + INFRAESTRUCTURA DE EVIDENCIA (LexicalEquivalence dictionary-2.0) + MIGRACIÓN MySQL APLICADA

**Estado:** CERRADO con evidencia real (build + tests + MySQL ideal).

## Alcance (cubierto completamente)

1. ADR creado: `docs/adr/0001-lexical-equivalence-directionality-and-versioning.md` — decisión y invariants (direccionalidad, proveniencia, canonical, inmutabilidad de CatalogVersion, aislamiento de variante, no reverse).
2. Modelo + EF: nueva entidad `LexicalEquivalence` (SourceLexemeId + TargetLanguage + CatalogVersionId + IsCanonical + EvidenceSource < ref); `DbSet` y configuración.
3. Engine: `dictionary-2.0` transcurre un pipeline que nunca revierte sin evidencia (I2), nunca mezcla variantes (I4), preserva duplicados por proveniencia, y asume 1 canónica exclusiva (2+ → integrity violation).
4. Golden tests: 12 escenarios probados (variant isolation, forward/reverse, proveniencia no-colapso, deterministic multi-target no winner, canonical zero/uno/dos, reproducibilidad con versión fija).
5. Migration & MySQL real: `20260925033847_LexicalEquivalenceModel` generada con EF Core (`Alter` manual aplicado post-collation-conformidad a `FK_LexicalEquivalences_Lexemes`, `Sources`, `CatalogVersions`; `CanonicalKey` generado como VIRTUAL para permitir FKs; índice único `UX_LexicalEquivalence_CanonicalPerTarget` aplicado; historial marcado `8.0.10`).

## Evidencia verificable (comandos reales)

- Build: `dotnet build AtlasBuho.slnx -c Release` → 0 Warnings / 0 Errores.
- Tests totales: 68 correctas, 1 omitido preexistente, 0 fallos (incluye `LexicalEquivalenceEngineTests` 12/12 + TranslateController 3/3 + reconciliador 474).
- BD: `SELECT CONSTRAINT_NAME, REFERENCED_TABLE_NAME FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA='AtlasBuho' AND TABLE_NAME='lexicalequivalences';` → 3 FKs vivas. `SHOW INDEX FROM lexicalequivalences WHERE Key_name='UX_LexicalEquivalence_CanonicalPerTarget'` → único vigente. Phase-1: 488 quarantine + 352 variants + 1 CatalogVersion intacto.

## Impacto en el estado del proyecto (árbol/proyección atlasbuho)

- 474 reconciliation · rollback evidence · nos quedo intactos.
- 6B: dominio+firmado (dictionary-1.1→2.0), aislación variante (P0), API 400/422/503 (P1), ora LexicalEquivalence (ADR 0001).
- No procede meteorpus-seeding ni UI hasta revisión de esta capa.

## Riesgos remanentes (declarados)

y’cuando avance al seeding masivo hay que (a) otorgar permisos de ingestión por versión de corpus; (b) impedir mutación de evidencia en versiones Completed con triggers/aplicación; (c) definir proceso de qué grado de evidencia queda “canonical”; (d) revisar el rendimiento del índice CanonicalKey (VIRTUAL) bajo carga.

