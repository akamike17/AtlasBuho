# ADR 0002 Addendum — Fase A · Ajuste de los 19 PD

**Estado:** AJUSTE PROPUESTO (en espera de confirmación final)
**Fecha:** 2026-09-25
**Vinculado a:** ADR 0002 (`docs/adr/0002-fase-a-trazabilidad-bdmd.md`)
**Respuesta a:** revisión del usuario "19 PD = REQUIEREN AJUSTE".

Las líneas PD-A1..A19 del ADR 0002 se redactaron para entidades/tables en modelo sellado B10/6B, y algunas piden cambios que romperían la fase cerrada (renombres, propagación obligatoria del enum EstadoVerificacion, separación física de Familia/Grupo). Este addendum **reformula cada PD con resolución ajustada a las reglas BD.MD** respetando que:

- Carga 364: **PROHIBIDA** hasta aprobación de Fase B.
- Fase sellada: **intocable** salvo añadir metadata, nunca renombrar ni sustituir.
- Toda implementación nueva vive **en paralelo** en `AtlasBuho.Domain.Entities.Design.*` hasta aprobarse.

---

## Ajuste por PD (resolución propuesta)

### PD-A1 — FamiliaLinguistica.EstadoVerificacion
**Ajuste:** no añadir el enum a `LanguageFamily` (sellado). Definir en Design:
`FamiliaCanon` { Id, LanguageFamilyId (link), EstadoVerificacion, FuenteId, FechaCreacion, FechaActualizacion, DatosAdicionalesJson }.
Regla BD.MD §19 cumplida en la entidad espejo, sin alterar la sellada.

### PD-A2 — AgrupacionLinguistica.EstadoVerificacion
**Ajuste:** espejo simétrico `AgrupacionCanon`. Mismo razonamiento que PD-A1.

### PD-A3 — VarianteLinguistica.FuentePrincipalId + EstadoVerificacion
**Ajuste:** espejo `VarianteCanon` { Id, LanguageVariantId, FuentePrincipalId, PaginaFuentePrincipal, EstadoVerificacion }. Los códigos Iso/Inali/BCP47 oprimirán a `CodigoLinguistico` (PD-A9); la columna plana original queda como índice de compatibilidad, nunca canónica.

### PD-A4 — VarianteLocalidad directa
**Ajuste:** crear `VarianteLocalidadCanon` (en Design) con los 7 campos §15. Hoy la única `VarianteLocalidad` vive en `AtlasBuhoEntities.cs` (archivo scratch, sin DbSet ni Configuration) — se propone migratear su shape a Design y dejar la scratch deprecada en Fase B.

### PD-A5 — Separación Población ≠ Localidad
**Ajuste:** nueva `PoblacionCanon` separada de `Localidad`. `Community` queda como la localización poblacional actual, no se borra; `PoblacionCanon` agrega capa semántica §11.

### PD-A6 — Tabla geográfica normalizada
**Ajuste:** `PaisCanon`, `EstadoCanon`, `MunicipioCanon`, `LocalidadCanon`. La `Region` actual se mantiene (rol distinto: clasificación lingüística, no geográfica).

### PD-A7 — VariantePoblacion M:N
**Ajuste:** `VariantePoblacionCanon` con `Relacion` enum { HABLA, USA, ASOCIADA, DOCUMENTADA_EN, OTRA } + EstadoVerificacion + FuenteId. Sustituye lógicamente la FK directa una vez Fase C migre datos.

### PD-A8 — PoblacionLocalidad M:N
**Ajuste:** `PoblacionLocalidadCanon` con los 7 campos §14.

### PD-A9 — CodigoLinguistico como tabla separada
**Ajuste:** crear `CodigoLinguisticoCanon` con Sistema enum { BCP47, ISO639_3, INALI, OTRO } + VarianteLinguisticaId + Codigo + FuenteId + EstadoVerificacion + Observaciones. Las columnas Iso639_3Code/InaliCode en `LanguageVariant` quedan **deprecated** (uso pasarela-compatibilidad) y se retiran solo cuando la reconciliación 364 esté verificada.

### PD-A10 — Source.Tipo enum controlado
**Ajuste:** añadir enum `TipoFuenteCanon` { INALI, CLIN, ISO, BCP47, PUBLICACION_ACADEMICA, CENSO, DOCUMENTO_GUBERNAMENTAL, DICCIONARIO, GRAMATICA, REPOSITORIO, OTRO } y mapearlo como columna adicional en `Source`. No se renombra la columna actual `Source.Tipo` (string) — se añade una enum, ambas poblándose durante Fase B.

### PD-A11 — EvidenciaFuente explícita
**Ajuste:** crear `EvidenciaFuenteCanon` { Id, FuenteId, Pagina, Seccion, TextoEvidencia, Ubicacion, HashEvidencia, FechaExtraccion, Observaciones }. No se renombra `Evidence` actual — `EvidenciaFuenteCanon` es la versión §17 pura; durante Fase B se enlaza.

### PD-A12 — PhraseTraduccion
**Ajuste:** definir en Design como `FraseTraduccionCanon` { Id, FraseOrigenId, FraseDestinoId, FuenteId, EstadoVerificacion } pero **bloquear su migración** hasta Fase E (traducción). Hoy `Phrase`/`PhraseLexeme` no lo cubren; queda como deuda explícita.

### PD-A13 — Estados intermedios ImportBatch
**Ajuste:** añadir a `CatalogVersion.ImportStatus` validación por enum wrapper o columna adicional `ImportStage` con { CREATED, PARSED, REVIEW_REQUIRED, RECONCILED, COMMITTED, REJECTED }. No se sustituye la cadena existente (la usan tests sellados); se añade estructura.

### PD-A14 — ImportRecord como RAW separado
**Ajuste:** crear `ImportRecordCanon` { Id, ImportBatchId, NumeroRegistro, TextoOriginal, DatosExtraidosJson, Estado, MotivoRechazo } en Design. `ImportQuarantine` actual queda como subset de resolución humana; `ImportRecordCanon` es el registro **antes** de clasificación. Se materializa en Fase B.

### PD-A15 — ReconciliacionCatalogo.Result enum
**Ajuste:** crear `ReconciliacionCatalogoCanon` con `Resultado` enum { MATCH, MISSING, DUPLICATE, CONFLICT, UNRESOLVED, NOT_APPLICABLE } + los 8 campos de §35. `CatalogRecord.Status` actual es interno, ocupa otra palabra; `ReconciliacionCatalogoCanon` es la reconciliación externa contra fuente oficial.

### PD-A16 — Auditoría como tabla
**Ajuste:** crear `AuditoriaCanon` { Id, Entidad, EntidadId, Operacion, Usuario, Fecha, AntesJson, DespuesJson, Motivo, FuenteId } con `Operacion` enum { CREATE, UPDATE, DEPRECATE, VERIFY, RECONCILE, REJECT, RESTORE }. Hoy no existe; se monta en Fase B y se activa desde triggers/MySQL, no desde código.

### PD-A17 — CorrespondenciaFuente
**Ajuste:** crear `CorrespondenciaFuenteCanon` §48 con `TipoCorrespondencia` enum { SAME_ENTITY, POSSIBLE_MATCH, BROADER, NARROWER, RELATED, CONFLICT, NOT_EQUIVALENT }. Fase C.

### PD-A18 — EstadoVerificacion propagado
**Ajuste:** no propagar al modelo sellado. El enum se fuerza en TODAS las entidades `*Canon` nuevas; la correlación sellada queda con el campo en la entidad espejo correspondiente.

### PD-A19 — Unknown vs Unverified
**Ajuste:** alinear por documentación ADR. En `*Canon`, BD.MD §42 se respeta: `Unverified` (candidato sin comprobar) ≠ `Unknown` (la fuente dice que se desconoce). En el modelo sellado el `VerificationStatus.Unknown` actual queda **deprecated** y re-etiquetado semántico `Unverified` en la próxima grande migración; mientras tanto es documentado como "actualmente equivale a Unverified" para no romper tests.

---

## Cierre Fase A — criterios ajustados

Fase A se considera CERRADA cuando:

1. Este addendum está aprobado.
2. Existe el documento/paquete `src/AtlasBuho.Domain/Entities/Design/` con clases POCO para:
   - `FamiliaCanon`, `AgrupacionCanon`, `VarianteCanon`,
   - `PoblacionCanon`, `PaisCanon`, `EstadoCanon`, `MunicipioCanon`, `LocalidadCanon`,
   - `VariantePoblacionCanon`, `PoblacionLocalidadCanon`, `VarianteLocalidadCanon`,
   - `CodigoLinguisticoCanon`, `EvidenciaFuenteCanon`,
   - `ReconciliacionCatalogoCanon`, `AuditoriaCanon`, `CorrespondenciaFuenteCanon`,
   - `ImportRecordCanon`, `FraseTraduccionCanon`,
   y enums asociados (TipoFuenteCanon, TipoPoblacion, TipoLocalidad, EstadoVerificacion / ReconciliacionResultado / AuditoriaOperacion / TipoCorrespondencia / AutonomoniaTipo / CodigoSistema / FraseTraduccionTipo).
3. **Sin DbSet, sin Configuration, sin migración.** Solo clase C# pura.
4. Build 0/0 + tests unit 68/68 + integration 45/45 siguen verdes.
5. Carga 364 sigue PROHIBIDA y se declara explícitamente en el siguiente commit.

## Próximo movimiento (tras tu OK)

Un único commit con:
- `src/AtlasBuho.Domain/Entities/Design/*.cs` — POCOs puros.
- Nada más. Cero migraciones, cero cambios en `AtlasBuhoDbContext`, cero cambios en tests.

Fase B queda bloqueada por aprobación explícita (no la iniciamos aún).
