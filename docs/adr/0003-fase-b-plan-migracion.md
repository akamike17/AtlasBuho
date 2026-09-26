# ADR 0003 — Fase B · Plan de migración BD.MD

**Estado:** PLAN APROBADO (PD-A11 = B, PD-A19 = cerrada, PD-A15 = corregido)
**Fecha:** 2026-09-25
**Vinculado a:** ADR 0002 + addendum.
**Baselínea:** commit `0009737` (Fase A CLOSED, build 0/0, tests 68/68 + 45/45).
**Carga 364: PROHIBIDA** hasta cerrar la fase de reconciliación separada (post-Fase B).

## 1. Decisiones cerradas que rigen el plan

- **PD-A11 = B.** Arquitectura Evidence con relaciones explícitas con FK real. La tabla genérica `Evidence` existente continúa (sellada, modelo B10), pero las entidades con necesidad de FK fuerte se modelan aparte: `EvidenciaVariante`, `EvidenciaCodigo`, `EvidenciaAutodenominacion`, `EvidenciaFamilia`, `EvidenciaAgrupacion`, `EvidenciaPoblacion`, `EvidenciaLocalidad`, `EvidenciaLexema`, `EvidenciaSignificado`, `EvidenciaTraduccion`, `EvidenciaPronunciacion`. **Ninguna** `EntityType + EntityId` nueva sin FK real.
- **PD-A19 = cerrada.** Semántica única `NULL / UNKNOWN / UNVERIFIED / DOCUMENTED / VERIFIED / RECONCILED / DISPUTED / DEPRECATED / REJECTED`. Aplicar exactamente en las nuevas entidades; el `Unknown` del modelo sellado queda semánticamente re-etiquetado como `Unverified` (sin renombrar la columna).
- **PD-A15 = corregido.** `CatalogRecordStatus` no se sustituye. `ReconciliationResultCanon` es dimensión separada con `CatalogRecordId NOT NULL` (FK → CatalogRecord).
- **Regla 364/364** se evalúa solo después (Fase de reconciliación), no es objetivo de Fase B.

## 2. Lista de POCOs en Domain (`AtlasBuho.Domain/Entities/Design/`)

En `namespace AtlasBuho.Domain.Entities.Design`. Cada uno trae `EstadoVerificacion EstadoVerificacion` (closed semantics PD-A19) y referencia FK a su entidad padre.

### 2.1 Jerarquía espejo de la fase sellada

- `FamiliaCanon` { Id, LanguageFamilyId, FuenteId, EstadoVerificacion, FechaCreacion, FechaActualizacion, DatosAdicionalesJson? }
- `AgrupacionCanon` { Id, LanguageGroupId, FuenteId, EstadoVerificacion, FechaCreacion, FechaActualizacion, DatosAdicionalesJson? }
- `VarianteCanon` { Id, LanguageVariantId, FuentePrincipalId, PaginaFuentePrincipal, EstadoVerificacion, FechaCreacion, FechaActualizacion, Observaciones? }

### 2.2 Geografía y población normalizadas

- `PaisCanon` { Id, Nombre, Codigo, EstadoVerificacion, FuenteId? }
- `EstadoCanon` { Id, PaisId, Nombre, Codigo, EstadoVerificacion, FuenteId? }
- `MunicipioCanon` { Id, EstadoId, Nombre, Codigo, EstadoVerificacion, FuenteId? }
- `LocalidadCanon` { Id, MunicipioId, Nombre, Codigo, Tipo, Latitud?, Longitud?, EstadoVerificacion, FuenteId? }
- `PoblacionCanon` { Id, Nombre, NombreAlternativo?, Tipo, EstadoVerificacion, FuenteId?, Observaciones? } — Tipo enum { PUEBLO, COMUNIDAD, PUEBLO_INDIGENA, GRUPO_POBLACIONAL, OTRO_DOCUMENTADO } §11
- `VariantePoblacionCanon` { Id, VarianteLinguisticaId, PoblacionId, Relacion, FuenteId, EstadoVerificacion, Observaciones? } — Relacion enum { HABLA, USA, ASOCIADA, DOCUMENTADA_EN, OTRA } §12
- `PoblacionLocalidadCanon` { Id, PoblacionId, LocalidadId, Relacion, FuenteId, EstadoVerificacion, Observaciones? } §14
- `VarianteLocalidadCanon` { Id, VarianteLinguisticaId, LocalidadId, Relacion, FuenteId, EstadoVerificacion, Observaciones? } §15

### 2.3 Identificadores

- `CodigoLinguisticoCanon` { Id, VarianteLinguisticaId, Sistema, Codigo, FuenteId, EstadoVerificacion, Observaciones? } — Sistema enum { BCP47, ISO639_3, INALI, OTRO } §9

### 2.4 Fuentes, evidencia, reconciliación, auditoría

- `TipoFuenteEnum` (aplicado sobre `Source`): columna nueva `TipoFuente` (enum) en Source además del string `Tipo` actual (sin renombrar el campo viejo).
- `EvidenciaVarianteCanon`, `EvidenciaCodigoCanon`, `EvidenciaAutodenominacionCanon`, `EvidenciaFamiliaCanon`, `EvidenciaAgrupacionCanon`, `EvidenciaPoblacionCanon`, `EvidenciaLocalidadCanon`, `EvidenciaLexemaCanon`, `EvidenciaSignificadoCanon`, `EvidenciaTraduccionCanon`, `EvidenciaPronunciacionCanon`
  Cada una: { Id, <Entidad>Id NOT NULL, FuenteId NOT NULL, Pagina?, Seccion?, TextoEvidencia?, Ubicacion?, HashEvidencia?, FechaExtraccion, Observaciones? } con FK real.
- `ReconciliationResultCanon` { Id, **CatalogRecordId** NOT NULL, FuenteId NOT NULL, IdentificadorFuente, NombreFuente, VarianteLinguisticaId? , Resultado, EvidenciaId?, Observaciones? } — Resultado enum { MATCH, MISSING, DUPLICATE, CONFLICT, UNRESOLVED, NOT_APPLICABLE }. `VarianteLinguisticaId` NULL solo cuando Resultado ∈ {MISSING, NOT_APPLICABLE}.
- `AuditoriaCanon` { Id, Entidad, EntidadId, Operacion, Usuario, Fecha, AntesJson?, DespuesJson?, Motivo?, FuenteId? } — Operacion enum { CREATE, UPDATE, DEPRECATE, VERIFY, RECONCILE, REJECT, RESTORE } §22.

### 2.5 Importación extendida

- `ImportRecordCanon` { Id, ImportBatchId, NumeroRegistro, TextoOriginal, DatosExtraidosJson?, Estado, MotivoRechazo? } — Estado enum { RAW, EXTRACTED, ACCEPTED, REJECTED, QUARANTINED }.
- `CatalogVersion.ImportStage` columna nueva (string) con valores { CREATED, PARSED, REVIEW_REQUIRED, RECONCILED, COMMITTED, REJECTED }. El string `ImportStatus` actual se mantiene.

### 2.6 Traducción de frases (deuda explícita)

- `FraseTraduccionCanon` { Id, FraseOrigenId, FraseDestinoId, FuenteId, EstadoVerificacion } — marcada como **blocked-implementation** hasta Fase E.

## 3. Mapeo EF Core

- Proyecto: `src/AtlasBuho.Data/Configurations/Design/`.
- Una clase `IEntityTypeConfiguration<T>` por POCO. Namespaces: `AtlasBuho.Data.Configurations.Design`.
- Tablas nombradas con prefijo `Canon`:
  `CanonFamilias, CanonAgrupaciones, CanonVariantes, CanonPaises, CanonEstados, CanonMunicipios, CanonLocalidades, CanonPoblaciones, CanonVariantePoblacion, CanonPoblacionLocalidad, CanonVarianteLocalidad, CanonCodigosLinguisticos, CanonEvidenciaVariante, CanonEvidenciaCodigo, CanonEvidenciaAutodenominacion, CanonEvidenciaFamilia, CanonEvidenciaAgrupacion, CanonEvidenciaPoblacion, CanonEvidenciaLocalidad, CanonEvidenciaLexema, CanonEvidenciaSignificado, CanonEvidenciaTraduccion, CanonEvidenciaPronunciacion, CanonReconciliationResults, CanonAuditorias, CanonImportRecords, CanonFraseTraduccion`.
- Todos los `Id` `CHAR(36)` ascii general_ci — coincide con el resto.
- FK a entidades selladas usa `OnDelete(DeleteBehavior.Restrict)` (el catálogo sellado nunca se borra; cascades quedan SOLO dentro del área Canon).
- Enums mapeados como `string` con `HasConversion<string>()` y maxLength.
- Campos largos `TextoEvidencia`, `Observaciones`, `AntesJson`, `DespuesJson`, `DatosExtraidosJson` se mapean a `TEXT`/`LONGTEXT`, no `VARCHAR(4000)` (regla de row-size).
- `EstadoVerificacion` siempre NOT NULL default `Unverified`.

## 4. DbContext

En `AtlasBuhoDbContext` (proyecto Data) se añaden DbSet con namespace explícito `Domain.Entities.Design`:

```
public DbSet<Domain.Entities.Design.FamiliaCanon> CanonFamilias => Set<...>();
public DbSet<Domain.Entities.Design.AgrupacionCanon> CanonAgrupaciones => Set<...>();
... (una por POCO Canon)
```

`Source` recibe añadido de columna `TipoFuente` (enum como string) — sin renombrar la columna `Tipo` actual.
`CatalogVersion` recibe añadido de columna `ImportStage` (string, nullable).

## 5. Migration

Nombre: `FaseB-CanonModel`. Orden:

1. CreateTable para cada CanonX (sin datos).
2. AddColumn `TipoFuente` a `Sources` (string, nullable).
3. AddColumn `ImportStage` a `CatalogVersions` (string, nullable).
4. AddForeignKey de cada Canon a su entidad padre (Restrict), a `Source` (Restrict), y entre Canon (`VariantePoblacion`, `PoblacionLocalidad`, `VarianteLocalidad`, `ReconciliationResult → CatalogRecord`).
5. Constraints de unicidad:
   - `CanonCodigosLinguisticos (VarianteLinguisticaId, Sistema, Codigo)` unique.
   - `CanonVariantePoblacion (VarianteLinguisticaId, PoblacionId)` unique.
   - `CanonPoblacionLocalidad (PoblacionId, LocalidadId)` unique.
   - `CanonReconciliationResults (CatalogRecordId, FuenteId)` unique (un resultado por fuente).

**No** se siembra 364. **No** se migra data desde las columnas viejas Iso/Inali a `CanonCodigosLinguisticos` todavía (eso entra en la fase de reconciliación).

## 6. Verificación de preservación de datos (gate)

Antes de cerrar Fase B se demuestra sobre una BD fresca `atlasbuho_fase_b_<epoch>`:

1. `CREATE DATABASE` + `dotnet ef database update` con la migración aplicada.
2. `SHOW TABLES` incluye las 24 tablas Canon y las tablas selladas existentes.
3. `information_schema.REFERENTIAL_CONSTRAINTS` confirma las FKs requeridas; sin errores 3192/1215.
4. Counts pre-migración == post-migración en todas las tablas selladas (LanguageFamilies, LanguageGroups, LanguageVariants, LanguageVariantAutodenominations, Lexemes, LexicalEquivalences, CatalogVersions, CatalogRecords, Sources, Evidence, ImportQuarantines, AiTranslationReviews, V2Candidates, etc.).
5. Tests de integración sellados (45/45) siguen verdes apuntando a una BD de prueba migrada.
6. Build 0/0 Release.
7. Añadir `tests/AtlasBuho.Tests.Integration/CanonSchemaTests.cs` con un caso por tabla Canon: `Migrate + SaveChanges + leer row = ok` (sin datos 364, solo fixtures).

Sólo cuando 1-7 están en evidencia fresca se declara Fase B CLOSED.

## 7. Bloqueadores que aún prohiben Fase C

- Carga 364 sigue **PROHIBIDA**.
- Población y geografía no se llenan hasta Fase C con fuentes documentadas.
- FraseTraduccionCanon solo se migra como esquema; su carga queda para Fase E.
- CodigoLinguisticoCanon solo se migra como esquema; la migración de Iso639_3Code/InaliCode existentes desde LanguageVariant se programa para la fase de reconciliación.
