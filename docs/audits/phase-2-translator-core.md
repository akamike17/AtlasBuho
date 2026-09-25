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
