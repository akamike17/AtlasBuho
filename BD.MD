# AtlasBuho

## Diseño Maestro de Base de Datos Lingüística

**Documento:** Diseño de BD — Inventario lingüístico, población, variantes, fuentes y conocimiento lingüístico
**Proyecto:** AtlasBuho
**Fase:** Diseño / Arquitectura de datos
**Estado:** PROPUESTA PARA REVISIÓN HUMANA
**Regla:** Este documento define estructura. No autoriza todavía la carga definitiva del catálogo lingüístico.

---

# 1. Objetivo

Diseñar una base de datos lingüística capaz de representar correctamente:

* Familias lingüísticas.
* Agrupaciones lingüísticas.
* Variantes lingüísticas.
* Autodenominaciones.
* Poblaciones hablantes.
* Localidades y regiones.
* Relaciones entre población y variante.
* Identificadores técnicos.
* Fuentes documentales.
* Evidencia de cada dato.
* Lexemas.
* Significados.
* Equivalencias.
* Traducciones.
* Pronunciaciones.
* Escrituras alternativas.
* Estado de verificación.
* Versionado de información.
* Auditoría.
* Datos provenientes de IA sin permitir que sustituyan datos canónicos.

El diseño debe permitir que una misma variante tenga:

* múltiples autodenominaciones;
* múltiples localidades;
* múltiples poblaciones;
* múltiples fuentes;
* múltiples códigos técnicos;
* múltiples lexemas;
* múltiples traducciones;
* múltiples evidencias.

Y debe impedir que esos conceptos se confundan entre sí.

---

# 2. Principio fundamental

## El nombre lingüístico no es el código.

AtlasBuho debe conservar separadamente:

```text
Familia
   ↓
Agrupación
   ↓
Variante
   ↓
Autodenominación
```

Y por separado:

```text
Variante
   ├── códigos técnicos
   ├── fuentes
   ├── localidades
   ├── población hablante
   ├── lexemas
   └── traducciones
```

Nunca debe utilizarse:

* BCP 47 como sustituto del nombre oficial.
* ISO 639-3 como sustituto del nombre oficial.
* un nombre generado por IA como sustituto del nombre documental.
* una localidad como sustituto de una variante.
* una autodenominación como sustituto de un nombre documental.
* una traducción como nombre de lengua.

---

# 3. Jerarquía lingüística

La estructura canónica será:

```text
FamiliaLingüística
        │
        └── AgrupaciónLingüística
                    │
                    └── VarianteLingüística
                                │
                                ├── Autodenominación
                                ├── Código
                                ├── Fuente
                                ├── Localidad
                                ├── Población
                                └── Datos lingüísticos
```

## 3.1 Familia

Representa una familia lingüística.

Ejemplo conceptual:

```text
Familia
  └── Nombre
```

No debe contener:

* población;
* localidad;
* traducciones;
* lexemas;
* códigos de variantes.

---

# 4. Tabla: FamiliaLinguistica

## Propósito

Catálogo de familias lingüísticas.

### Campos

| Campo              | Tipo conceptual | Obligatorio | Descripción             |
| ------------------ | --------------- | ----------: | ----------------------- |
| Id                 | PK              |          Sí | Identificador interno   |
| Nombre             | VARCHAR         |          Sí | Nombre documental       |
| Descripcion        | TEXT            |          No | Descripción documentada |
| FuenteId           | FK              |          No | Fuente primaria         |
| EstadoVerificacion | ENUM            |          Sí | Estado de verificación  |
| FechaCreacion      | DATETIME        |          Sí | Auditoría               |
| FechaActualizacion | DATETIME        |          Sí | Auditoría               |

### Regla

`Nombre` no debe ser generado ni traducido automáticamente.

---

# 5. Tabla: AgrupacionLinguistica

## Propósito

Representa la agrupación lingüística perteneciente a una familia.

### Campos

| Campo                | Tipo     | Obligatorio |
| -------------------- | -------- | ----------: |
| Id                   | PK       |          Sí |
| FamiliaLinguisticaId | FK       |          Sí |
| Nombre               | VARCHAR  |          Sí |
| Descripcion          | TEXT     |          No |
| FuenteId             | FK       |          No |
| EstadoVerificacion   | ENUM     |          Sí |
| FechaCreacion        | DATETIME |          Sí |
| FechaActualizacion   | DATETIME |          Sí |

### Relación

```text
Familia 1 ───── N Agrupaciones
```

---

# 6. Tabla: VarianteLinguistica

Esta es la entidad central del inventario lingüístico.

## Campos

| Campo                   | Tipo     | Obligatorio | Descripción                    |
| ----------------------- | -------- | ----------: | ------------------------------ |
| Id                      | PK       |          Sí | ID interno AtlasBuho           |
| AgrupacionLinguisticaId | FK       |          Sí | Agrupación                     |
| NombreOficial           | VARCHAR  |          Sí | Nombre exactamente documentado |
| Descripcion             | TEXT     |          No | Descripción documentada        |
| EstadoVerificacion      | ENUM     |          Sí | Estado                         |
| FuentePrincipalId       | FK       |          No | Fuente primaria                |
| PaginaFuentePrincipal   | VARCHAR  |          No | Página/sección                 |
| Observaciones           | TEXT     |          No | Notas de auditoría             |
| FechaCreacion           | DATETIME |          Sí | Auditoría                      |
| FechaActualizacion      | DATETIME |          Sí | Auditoría                      |

---

# 7. Regla crítica para NombreOficial

`NombreOficial` debe conservarse **literalmente**.

No se permite:

```text
Normalizar
Traducir
Corregir
Abreviar
Cambiar mayúsculas
Eliminar acentos
Cambiar apóstrofes
Cambiar espacios
```

Ejemplo:

```text
Fuente:
"Nombre exacto documentado"

BD:
"Nombre exacto documentado"
```

No:

```text
"nombre exacto documentado"
"Nombre Exacto Documentado"
"Nombre documentado"
"Nombre corregido"
```

La presentación alternativa debe almacenarse en otra entidad.

---

# 8. Tabla: AutoDenominacion

Una variante puede tener cero, una o múltiples autodenominaciones.

### Campos

| Campo                 | Tipo    | Obligatorio |
| --------------------- | ------- | ----------: |
| Id                    | PK      |          Sí |
| VarianteLinguisticaId | FK      |          Sí |
| Texto                 | VARCHAR |          Sí |
| Tipo                  | ENUM    |          Sí |
| FuenteId              | FK      |          Sí |
| PaginaFuente          | VARCHAR |          No |
| EstadoVerificacion    | ENUM    |          Sí |
| Observaciones         | TEXT    |          No |

### Tipos posibles

```text
AUTODENOMINACION
ENDONIMO
VARIANTE_ORTOGRAFICA
NOMBRE_ALTERNATIVO_DOCUMENTADO
OTRO_DOCUMENTADO
```

No se debe utilizar esta tabla para guardar nombres inventados.

---

# 9. Tabla: CodigoLinguistico

Los códigos técnicos deben estar separados del nombre lingüístico.

### Campos

| Campo                 | Tipo    |
| --------------------- | ------- |
| Id                    | PK      |
| VarianteLinguisticaId | FK      |
| Sistema               | ENUM    |
| Codigo                | VARCHAR |
| FuenteId              | FK      |
| EstadoVerificacion    | ENUM    |
| Observaciones         | TEXT    |

### Sistemas

```text
BCP47
ISO639_3
INALI
OTRO
```

## Regla

No generar códigos automáticamente.

Si un código no está demostrado:

```text
NULL
```

o:

```text
EstadoVerificacion = UNVERIFIED
```

Nunca:

```text
"probablemente ..."
```

---

# 10. Población hablante

La población no debe almacenarse directamente como texto dentro de `VarianteLinguistica`.

Se necesita una entidad independiente.

```text
Variante
    ↓
Poblacion
```

Pero también:

```text
Poblacion
    ↓
Localidad
```

y potencialmente:

```text
Localidad
    ↓
Municipio
    ↓
Estado
    ↓
Pais
```

---

# 11. Tabla: Poblacion

## Propósito

Representar una población o comunidad hablante documentada.

### Campos

| Campo              | Tipo    | Obligatorio |
| ------------------ | ------- | ----------: |
| Id                 | PK      |          Sí |
| Nombre             | VARCHAR |          Sí |
| NombreAlternativo  | VARCHAR |          No |
| Tipo               | ENUM    |          Sí |
| FuenteId           | FK      |          No |
| EstadoVerificacion | ENUM    |          Sí |
| Observaciones      | TEXT    |          No |

### Tipos

```text
PUEBLO
COMUNIDAD
PUEBLO_INDIGENA
GRUPO_POBLACIONAL
OTRO_DOCUMENTADO
```

La población no debe confundirse con:

* variante;
* agrupación;
* familia;
* localidad.

---

# 12. Relación Variante ↔ Población

Una variante puede estar asociada con múltiples poblaciones y una población puede estar asociada con múltiples variantes.

Por lo tanto:

```text
Variante N ───── N Poblacion
```

requiere tabla intermedia.

# Tabla: VariantePoblacion

| Campo                 | Tipo |
| --------------------- | ---- |
| Id                    | PK   |
| VarianteLinguisticaId | FK   |
| PoblacionId           | FK   |
| Relacion              | ENUM |
| FuenteId              | FK   |
| EstadoVerificacion    | ENUM |
| Observaciones         | TEXT |

### Relación

```text
HABLA
USA
ASOCIADA
DOCUMENTADA_EN
OTRA
```

No inferir la relación.

---

# 13. Localidades

La localidad es un concepto geográfico, no lingüístico.

## Tabla: Pais

```text
Id
Nombre
Codigo
```

## Tabla: Estado

```text
Id
PaisId
Nombre
Codigo
```

## Tabla: Municipio

```text
Id
EstadoId
Nombre
Codigo
```

## Tabla: Localidad

```text
Id
MunicipioId
Nombre
Codigo
Tipo
Latitud
Longitud
FuenteId
EstadoVerificacion
```

Las coordenadas son opcionales y requieren fuente.

---

# 14. Relación Población ↔ Localidad

```text
Poblacion N ───── N Localidad
```

Tabla:

# PoblacionLocalidad

| Campo              | Tipo |
| ------------------ | ---- |
| Id                 | PK   |
| PoblacionId        | FK   |
| LocalidadId        | FK   |
| Relacion           | ENUM |
| FuenteId           | FK   |
| EstadoVerificacion | ENUM |
| Observaciones      | TEXT |

Esto permite representar correctamente que una población puede encontrarse en varias localidades.

---

# 15. Relación Variante ↔ Localidad

Aunque puede derivarse mediante población, se permite una relación directa cuando la fuente documenta específicamente una variante en una localidad.

# VarianteLocalidad

```text
Id
VarianteLinguisticaId
LocalidadId
Relacion
FuenteId
EstadoVerificacion
Observaciones
```

Esto evita perder evidencia directa.

---

# 16. Fuentes documentales

La fuente debe ser una entidad de primera clase.

# Tabla: Fuente

| Campo            | Tipo     |
| ---------------- | -------- |
| Id               | PK       |
| AutorInstitucion | VARCHAR  |
| Titulo           | VARCHAR  |
| Tipo             | ENUM     |
| Anio             | INT      |
| Url              | VARCHAR  |
| Identificador    | VARCHAR  |
| Version          | VARCHAR  |
| FechaConsulta    | DATETIME |
| Observaciones    | TEXT     |

### Tipos

```text
INALI
CLIN
ISO
BCP47
PUBLICACION_ACADEMICA
CENSO
DOCUMENTO_GUBERNAMENTAL
DICCIONARIO
GRAMATICA
REPOSITORIO
OTRO
```

---

# 17. Evidencia

No basta con guardar una fuente.

Hay que poder responder:

> ¿Exactamente dónde dice esto?

Por ello se crea:

# Tabla: EvidenciaFuente

| Campo           | Tipo     |
| --------------- | -------- |
| Id              | PK       |
| FuenteId        | FK       |
| Pagina          | VARCHAR  |
| Seccion         | VARCHAR  |
| TextoEvidencia  | TEXT     |
| Ubicacion       | VARCHAR  |
| HashEvidencia   | VARCHAR  |
| FechaExtraccion | DATETIME |
| Observaciones   | TEXT     |

---

# 18. Relación genérica con evidencia

Cada dato crítico debe poder apuntar a evidencia.

Como mínimo:

```text
Familia → Fuente
Agrupacion → Fuente
Variante → Fuente
Autodenominacion → Fuente
Codigo → Fuente
Poblacion → Fuente
Localidad → Fuente
VariantePoblacion → Fuente
Lexema → Fuente
Significado → Fuente
Traduccion → Fuente
Pronunciacion → Fuente
```

No depender exclusivamente de un `FuenteId` general.

---

# 19. Estado de verificación

Se recomienda un catálogo controlado.

# EstadoVerificacion

```text
UNVERIFIED
DOCUMENTED
VERIFIED
RECONCILED
DISPUTED
DEPRECATED
REJECTED
```

## Significado

### UNVERIFIED

Existe como dato candidato, pero todavía no fue comprobado.

### DOCUMENTED

Existe en una fuente identificable.

### VERIFIED

Fue comprobado contra la fuente correspondiente.

### RECONCILED

Fue comprobado y además reconciliado contra el inventario canónico.

### DISPUTED

Existen fuentes o interpretaciones incompatibles.

### DEPRECATED

Ya no debe utilizarse como dato activo, pero se conserva por trazabilidad.

### REJECTED

El registro fue determinado como inválido.

---

# 20. Nunca borrar información lingüística

La información lingüística debe tener trazabilidad.

No hacer:

```sql
DELETE
```

para corregir un dato documental.

Preferir:

```text
Estado = DEPRECATED
```

o versionado.

---

# 21. Versionado

# Tabla: RegistroVersion

| Campo         | Tipo     |
| ------------- | -------- |
| Id            | PK       |
| Entidad       | VARCHAR  |
| EntidadId     | BIGINT   |
| Version       | INT      |
| Estado        | ENUM     |
| DatosJson     | JSON     |
| FuenteCambio  | VARCHAR  |
| UsuarioCambio | VARCHAR  |
| FechaCambio   | DATETIME |

Permite reconstruir:

```text
qué dato existía
cuándo existía
quién lo modificó
qué fuente justificó el cambio
```

---

# 22. Auditoría

# Tabla: Auditoria

```text
Id
Entidad
EntidadId
Operacion
Usuario
Fecha
AntesJson
DespuesJson
Motivo
FuenteId
```

Operaciones:

```text
CREATE
UPDATE
DEPRECATE
VERIFY
RECONCILE
REJECT
RESTORE
```

---

# 23. Léxico

La información lingüística real debe estar separada del catálogo administrativo.

# Tabla: Lexema

| Campo                 | Tipo    |
| --------------------- | ------- |
| Id                    | PK      |
| VarianteLinguisticaId | FK      |
| Forma                 | VARCHAR |
| FormaNormalizada      | VARCHAR |
| Escritura             | VARCHAR |
| Tipo                  | ENUM    |
| FuenteId              | FK      |
| EstadoVerificacion    | ENUM    |

## Regla

`Forma` conserva la forma documentada.

`FormaNormalizada` es opcional y nunca debe sustituir a `Forma`.

---

# 24. Significado

# Tabla: Significado

```text
Id
Texto
Descripcion
Idioma
FuenteId
EstadoVerificacion
```

Un lexema puede tener múltiples significados.

```text
Lexema N ───── N Significado
```

Tabla:

# LexemaSignificado

```text
Id
LexemaId
SignificadoId
FuenteId
EstadoVerificacion
```

---

# 25. Traducción

Una traducción no es un lexema.

Por eso debe tener entidad independiente.

# Tabla: Traduccion

```text
Id
VarianteOrigenId
VarianteDestinoId
TextoOrigen
TextoDestino
Tipo
FuenteId
EstadoVerificacion
```

Ejemplo conceptual:

```text
Variante A
    ↓
"sol"
    ↓
Variante B
    ↓
"forma documentada"
```

---

# 26. Equivalencia léxica

Una equivalencia no debe asumirse porque dos palabras parezcan iguales.

# Tabla: EquivalenciaLexica

```text
Id
LexemaOrigenId
LexemaDestinoId
Tipo
FuenteId
EstadoVerificacion
ConfianzaDocumental
Observaciones
```

### Tipos

```text
EQUIVALENCIA_EXACTA
EQUIVALENCIA_SEMANTICA
VARIANTE_FORMAL
NO_EQUIVALENTE
DOCUMENTADA
```

La relación debe provenir de evidencia.

---

# 27. Pronunciación

La pronunciación es un dato separado de la ortografía.

# Tabla: Pronunciacion

```text
Id
LexemaId
Representacion
Sistema
AudioReferencia
FuenteId
EstadoVerificacion
```

### Sistemas posibles

```text
IPA
ORTOGRAFIA_DOCUMENTADA
AUDIO
OTRO
```

No inventar pronunciaciones.

---

# 28. Audio

# Tabla: RecursoAudio

```text
Id
Uri
Hash
Formato
DuracionMs
FuenteId
FechaCarga
EstadoVerificacion
```

Puede asociarse a:

```text
Lexema
Frase
Pronunciacion
Variante
```

mediante relaciones específicas.

---

# 29. Frases

# Tabla: Frase

```text
Id
VarianteLinguisticaId
Texto
Tipo
FuenteId
EstadoVerificacion
```

Tipos:

```text
EJEMPLO
SALUDO
FRASE_DOCUMENTADA
PRUEBA
TRADUCCION
OTRO
```

---

# 30. Traducción de frases

# Tabla: FraseTraduccion

```text
Id
FraseOrigenId
FraseDestinoId
FuenteId
EstadoVerificacion
```

No almacenar una traducción generada por IA como canónica automáticamente.

---

# 31. IA

La IA debe estar completamente separada del conocimiento canónico.

# Tabla: AiSuggestion

```text
Id
TipoEntidad
EntidadId
Proveedor
Modelo
Entrada
Salida
Fecha
Estado
MotivoRevision
Revisor
```

Estados:

```text
GENERATED
PENDING_REVIEW
ACCEPTED
REJECTED
```

## Regla crítica

```text
AI suggestion ≠ canonical data
```

Una sugerencia de IA jamás entra automáticamente al catálogo canónico.

---

# 32. Importaciones

Para futuras cargas desde documentos:

# Tabla: ImportBatch

```text
Id
FuenteId
NombreArchivo
HashArchivo
FechaImportacion
RegistrosDetectados
RegistrosAceptados
RegistrosRechazados
Estado
```

Estados:

```text
CREATED
PARSED
REVIEW_REQUIRED
RECONCILED
COMMITTED
REJECTED
```

---

# 33. Registro bruto de importación

Antes de transformar datos:

# Tabla: ImportRecord

```text
Id
ImportBatchId
NumeroRegistro
TextoOriginal
DatosExtraidosJson
Estado
MotivoRechazo
```

Esto es fundamental.

Permite conservar:

```text
DOCUMENTO ORIGINAL
        ↓
EXTRACCION
        ↓
NORMALIZACION
        ↓
RECONCILIACION
        ↓
CATALOGO
```

sin perder el origen.

---

# 34. Separación entre extracción y canon

El pipeline debe ser:

```text
FUENTE PRIMARIA
      │
      ▼
DOCUMENTO
      │
      ▼
EXTRACCIÓN LITERAL
      │
      ▼
REGISTRO BRUTO
      │
      ▼
RECONCILIACIÓN HUMANA
      │
      ▼
CATÁLOGO CANÓNICO
```

Nunca:

```text
PDF
 ↓
IA
 ↓
BD definitiva
```

---

# 35. Inventario canónico

La reconciliación de AtlasBuho debe poder expresar:

```text
Fuente oficial
     │
     ├── registro 1
     ├── registro 2
     ├── ...
     └── registro 364
             │
             ▼
      Variante AtlasBuho
```

Para cada registro se necesita:

# Tabla: ReconciliacionCatalogo

```text
Id
FuenteId
IdentificadorFuente
NombreFuente
VarianteLinguisticaId
Resultado
EvidenciaId
Observaciones
```

Resultados:

```text
MATCH
MISSING
DUPLICATE
CONFLICT
UNRESOLVED
NOT_APPLICABLE
```

---

# 36. Regla 364/364

Para el inventario actual de la fase correspondiente:

```text
Registros oficiales esperados = 364
```

La BD no debe declarar reconciliación completa si:

```text
MATCH != 364
```

o si existen:

```text
MISSING
CONFLICT
UNRESOLVED
```

pendientes.

La cantidad **no debe corregirse artificialmente** para hacer coincidir el número.

---

# 37. Identificador interno vs identificador externo

AtlasBuho debe tener sus propios IDs:

```text
Id
```

Los IDs externos deben conservarse separados:

```text
InaliCode
Iso6393
Bcp47
ExternalId
```

Nunca utilizar un código externo como PK interna.

---

# 38. Unicidad

Las restricciones deben evitar duplicados accidentales.

Ejemplos:

```text
Familia + Nombre
Agrupacion + Nombre
Variante + Agrupacion + NombreOficial
CodigoLinguistico + Sistema
```

Pero **no** debe asumirse que un nombre textual idéntico representa necesariamente la misma entidad.

Por ello, los duplicados potenciales deben pasar por reconciliación.

---

# 39. Búsqueda

La aplicación debe poder buscar por:

```text
Nombre oficial
Autodenominación
Familia
Agrupación
Código INALI
ISO 639-3
BCP47
Población
Localidad
Lexema
Significado
```

Pero el resultado debe mostrar claramente qué entidad encontró.

Ejemplo:

```text
Consulta:
"zapoteco"

Resultados:

Familia:
...

Agrupación:
...

Variante:
...

Autodenominación:
...

Población:
...
```

No mezclar entidades en una lista plana.

---

# 40. Modelo conceptual completo

```text
                         ┌──────────────────┐
                         │      FUENTE      │
                         └────────┬─────────┘
                                  │
                         ┌────────▼─────────┐
                         │    EVIDENCIA     │
                         └────────┬─────────┘
                                  │
                                  │
┌────────────────┐       ┌────────▼─────────┐
│    FAMILIA     │ 1──N  │   AGRUPACIÓN     │
└────────────────┘       └────────┬─────────┘
                                  │ 1
                                  │
                                  │ N
                         ┌────────▼─────────┐
                         │     VARIANTE     │
                         └───┬────┬────┬────┘
                             │    │    │
               ┌─────────────┘    │    └──────────────┐
               │                  │                   │
        ┌──────▼──────┐    ┌──────▼──────┐     ┌──────▼──────┐
        │AUTODENOMINA │    │   CODIGOS   │     │  LEXEMAS    │
        └─────────────┘    └─────────────┘     └──────┬───────┘
                                                       │
                                                ┌──────▼───────┐
                                                │ SIGNIFICADOS │
                                                └──────────────┘

                         ┌────────────────┐
                         │   POBLACIÓN    │
                         └───────┬────────┘
                                 │
                                 │ N:N
                                 │
                         ┌───────▼────────┐
                         │VARIANTE_POBL.  │
                         └────────────────┘
                                 │
                                 │
                         ┌───────▼────────┐
                         │   LOCALIDAD    │
                         └────────────────┘

                         ┌────────────────┐
                         │  TRADUCCIONES  │
                         └────────────────┘

                         ┌────────────────┐
                         │      IA        │
                         │   SUGERENCIAS  │
                         └────────────────┘
                                  │
                                  │
                         NO CANÓNICA
```

---

# 41. Reglas de integridad

## Regla 1

Toda variante debe pertenecer a una agrupación.

```text
Variante.AgrupacionId NOT NULL
```

## Regla 2

Toda agrupación debe pertenecer a una familia.

```text
Agrupacion.FamiliaId NOT NULL
```

## Regla 3

Una autodenominación debe tener variante.

## Regla 4

Un código debe tener sistema explícito.

## Regla 5

Un dato documentado debe poder rastrearse hasta una fuente.

## Regla 6

Un dato no verificado no puede marcarse como canónico.

## Regla 7

Una sugerencia de IA nunca debe insertarse directamente como dato canónico.

## Regla 8

No borrar datos históricos.

## Regla 9

No inventar datos faltantes.

## Regla 10

No utilizar `UNKNOWN` como sustituto silencioso de evidencia.

Debe distinguirse:

```text
NULL
UNVERIFIED
UNKNOWN
DOCUMENTED
VERIFIED
```

---

# 42. NULL vs UNKNOWN

No significan lo mismo.

### NULL

No existe información cargada.

### UNKNOWN

La fuente/documentación indica que el dato es desconocido.

### UNVERIFIED

Existe un dato candidato pero aún no se comprobó.

Ejemplo:

```text
CodigoISO = NULL
Estado = UNVERIFIED
```

es diferente de:

```text
CodigoISO = NULL
Estado = VERIFIED
```

porque el segundo significa que se verificó que no existe/no corresponde/no está disponible según la fuente.

---

# 43. Datos que NO deben mezclarse

Nunca poner en una misma columna:

```text
"Variante / pueblo / localidad"
```

Nunca:

```text
Nombre = "Zapoteco de X, comunidad Y"
```

si la fuente realmente distingue:

```text
Agrupación = X
Variante = Y
Población = Z
Localidad = W
```

Cada concepto debe tener su entidad.

---

# 44. Estados del catálogo

El flujo recomendado:

```text
RAW
 ↓
EXTRACTED
 ↓
DOCUMENTED
 ↓
VERIFIED
 ↓
RECONCILED
 ↓
CANONICAL
```

Un registro puede detenerse en cualquier etapa.

Por ejemplo:

```text
RAW
 ↓
EXTRACTED
 ↓
UNRESOLVED
```

y permanecer ahí.

No se debe rellenar artificialmente para avanzar.

---

# 45. Seguridad contra corrupción del catálogo

Debe existir una protección explícita contra:

* nombres geográficos introducidos como autodenominaciones;
* familias introducidas como variantes;
* variantes introducidas como poblaciones;
* códigos inventados;
* traducciones inferidas;
* duplicados;
* registros incompletos;
* registros fuera del inventario oficial;
* texto basura extraído del PDF;
* errores OCR;
* encabezados;
* pies de página;
* referencias bibliográficas;
* notas editoriales.

---

# 46. Auditoría de extracción

Antes de importar un documento:

```text
PDF
 ↓
extracción
 ↓
conteo
 ↓
detección de encabezados
 ↓
detección de duplicados
 ↓
detección de basura
 ↓
revisión
 ↓
reconciliación
```

No:

```text
PDF → INSERT
```

---

# 47. Diseño para múltiples fuentes

INALI/CLIN puede representar una estructura.

Otra fuente puede utilizar otra.

La BD no debe destruir la información original.

Debe poder almacenar:

```text
Fuente A:
Familia A
Agrupación A
Variante A

Fuente B:
Grupo B
Variante B
```

y posteriormente establecer una relación documentada:

```text
CorrespondenciaFuente
```

---

# 48. Tabla: CorrespondenciaFuente

```text
Id
FuenteOrigenId
EntidadOrigen
EntidadOrigenId
FuenteDestinoId
EntidadDestino
EntidadDestinoId
TipoCorrespondencia
FuenteEvidenciaId
EstadoVerificacion
Observaciones
```

Tipos:

```text
SAME_ENTITY
POSSIBLE_MATCH
BROADER
NARROWER
RELATED
CONFLICT
NOT_EQUIVALENT
```

No asumir equivalencia.

---

# 49. Multi-idioma de la aplicación

Los nombres lingüísticos canónicos no deben traducirse.

La interfaz sí puede ser multilingüe.

Por ello, si AtlasBuho posteriormente necesita:

```text
Español
English
Náhuatl
etc.
```

se debe utilizar:

```text
TranslationResource
```

y no alterar:

```text
Variante.NombreOficial
```

---

# 50. Preparación para API

El modelo debe permitir endpoints conceptuales:

```text
GET /api/families
GET /api/groups
GET /api/variants
GET /api/variants/{id}
GET /api/variants/{id}/names
GET /api/variants/{id}/codes
GET /api/variants/{id}/populations
GET /api/variants/{id}/locations
GET /api/variants/{id}/lexicon
GET /api/sources
GET /api/evidence
```

La API no debe devolver una estructura plana que pierda la jerarquía.

---

# 51. Respuesta conceptual de una variante

```json
{
  "id": 123,
  "officialName": "...",
  "family": {
    "id": 1,
    "name": "..."
  },
  "group": {
    "id": 10,
    "name": "..."
  },
  "autodenominations": [],
  "codes": [],
  "populations": [],
  "locations": [],
  "sources": [],
  "evidence": []
}
```

Los valores reales se incorporarán únicamente después de la reconciliación documental.

---

# 52. Índices

Índices esperados:

```text
Familia.Nombre
Agrupacion.Nombre
Variante.NombreOficial
AutoDenominacion.Texto
CodigoLinguistico.Codigo
Poblacion.Nombre
Localidad.Nombre
Lexema.Forma
Significado.Texto
Fuente.Titulo
```

Para búsquedas grandes podrán incorporarse índices FULLTEXT según el motor utilizado.

---

# 53. Eliminación lógica

Las entidades lingüísticas principales no deben eliminarse físicamente.

Utilizar:

```text
IsActive
```

o preferentemente:

```text
EstadoVerificacion
```

según el caso.

Esto permite preservar historial.

---

# 54. Restricciones recomendadas

### Variante

```text
AgrupacionLinguisticaId NOT NULL
NombreOficial NOT NULL
```

### Agrupación

```text
FamiliaLinguisticaId NOT NULL
Nombre NOT NULL
```

### Autodenominación

```text
VarianteLinguisticaId NOT NULL
Texto NOT NULL
FuenteId NOT NULL
```

### Código

```text
VarianteLinguisticaId NOT NULL
Sistema NOT NULL
Codigo NOT NULL
```

### Evidencia

```text
FuenteId NOT NULL
```

---

# 55. Migraciones

La implementación debe realizarse mediante migraciones versionadas.

Nunca modificar la BD manualmente como mecanismo principal.

Proceso:

```text
Modelo
 ↓
Migration
 ↓
Build
 ↓
Tests
 ↓
Migration DB
 ↓
Integration Tests
```

---

# 56. Datos de prueba

Los tests no deben confundirse con el catálogo real.

Por ejemplo:

```text
test-p0-*
```

deben estar claramente separados.

Nunca deben aparecer como variantes oficiales.

Se recomienda:

```text
Environment = Test
```

o una BD aislada:

```text
atlasbuho_test
```

---

# 57. Reglas de tests

Debe existir cobertura para:

### Jerarquía

```text
Familia → Agrupación → Variante
```

### Autodenominación

```text
Variante → múltiples autodenominaciones
```

### Población

```text
Variante ↔ Población
```

### Localidad

```text
Población ↔ Localidad
```

### Códigos

```text
ISO / BCP47 / INALI separados
```

### Evidencia

```text
Dato → Fuente → Evidencia
```

### IA

```text
AI suggestion ≠ canonical
```

### Reconciliación

```text
364 oficiales
364 reconciliados
```

cuando el inventario oficial correspondiente haya sido completamente verificado.

---

# 58. Regla para la carga inicial

La primera carga NO debe ser:

```text
INSERT directo desde parser
```

Debe ser:

```text
Documento INALI/CLIN
       ↓
Extracción literal
       ↓
364 registros esperados
       ↓
Auditoría
       ↓
Mapping
       ↓
Revisión humana
       ↓
Reconciliación
       ↓
Carga canónica
```

---

# 59. B10.2 y el problema que este diseño evita

Los problemas detectados durante la auditoría demuestran por qué las entidades deben estar separadas.

Ejemplo de dato incorrecto:

```text
Autodenominacion = "mayan"
```

si esa cadena realmente procede de otro campo documental.

Con este diseño:

```text
AutoDenominacion
   ├── Texto
   ├── VarianteId
   ├── FuenteId
   └── EvidenciaId
```

no puede considerarse válida simplemente porque el parser la encontró cerca del nombre de una variante.

Debe existir evidencia.

---

# 60. Regla de oro del catálogo

> **Si no podemos señalar la fuente y la evidencia exacta, el dato no es canónico.**

Esto aplica a:

* nombres;
* autodenominaciones;
* códigos;
* población;
* localidades;
* pronunciaciones;
* lexemas;
* significados;
* traducciones;
* equivalencias.

---

# 61. Orden recomendado de implementación

## Fase A — Modelo

Crear entidades y relaciones.

```text
Familia
Agrupación
Variante
Autodenominación
Código
Población
Localidad
Fuente
Evidencia
```

## Fase B — Catálogo

Reconciliar el inventario oficial.

```text
364 / 364
```

## Fase C — Población/geografía

Cargar únicamente información documentada.

## Fase D — Léxico

```text
Lexema
Significado
Pronunciación
```

## Fase E — Traducción

```text
Traduccion
EquivalenciaLexica
Frase
```

## Fase F — IA

```text
AiSuggestion
```

siempre separada del catálogo canónico.

---

# 62. Lo que NO se debe hacer todavía

Hasta cerrar la reconciliación documental:

```text
NO poblar las 364 variantes
NO inventar autodenominaciones
NO inventar códigos
NO corregir nombres "a criterio"
NO agregar población inferida
NO agregar localidades inferidas
NO generar traducciones canónicas
NO generar pronunciaciones canónicas
NO modificar el catálogo actual automáticamente
```

Primero:

```text
MODELO
 ↓
AUDITORÍA
 ↓
RECONCILIACIÓN
 ↓
APROBACIÓN
 ↓
IMPLEMENTACIÓN
```

---

# 63. Criterio de aprobación del diseño

El diseño puede considerarse aprobado cuando:

* La jerarquía lingüística está separada correctamente.
* Población y localidad están separadas.
* Autodenominación está separada de nombre oficial.
* Códigos están separados de nombres.
* Fuentes y evidencias son trazables.
* El léxico está separado del catálogo administrativo.
* IA está separada del conocimiento canónico.
* Existe versionado/auditoría.
* Se puede representar información de múltiples fuentes.
* Se pueden conservar conflictos sin destruir información.
* No existe una columna genérica que mezcle conceptos lingüísticos distintos.

---

# 64. Criterio de cierre de la implementación

La implementación futura solamente podrá declararse:

```text
CLOSED / VERIFIED
```

cuando exista evidencia real de:

```text
Migration aplicada
        +
Build PASS
        +
Tests PASS
        +
Integration PASS
        +
Reconciliación documental PASS
        +
Git diff revisado
        +
Working tree limpio
        +
HEAD == origin
```

Un reporte de agente por sí solo no constituye evidencia suficiente.

---

# 65. Resumen de arquitectura

```text
                         ATLASBUHO DB
                              │
          ┌───────────────────┼────────────────────┐
          │                   │                    │
          ▼                   ▼                    ▼
     CATÁLOGO             GEOGRAFÍA           FUENTES
          │                   │                    │
          │                   │                    ├── Fuente
          │                   │                    └── Evidencia
          │                   │
          ├── Familia         ├── País
          ├── Agrupación      ├── Estado
          └── Variante        ├── Municipio
               │              └── Localidad
               ├── Autodenominación
               ├── Código
               └── Población
                       │
                       └── Localidad

                              │
                              ▼
                         CONOCIMIENTO
                              │
                    ┌─────────┼─────────┐
                    ▼         ▼         ▼
                 Lexema  Significado  Pronunciación
                    │
                    ▼
               Equivalencias
                    │
                    ▼
               Traducciones

                              │
                              ▼
                          IA AISLADA
                              │
                         Suggestions
                              │
                              ▼
                      REVISIÓN HUMANA
                              │
                  ┌───────────┴───────────┐
                  ▼                       ▼
              REJECTED                ACCEPTED
                                          │
                                          ▼
                              DATOS CANÓNICOS
```

---

# 66. Decisión arquitectónica

AtlasBuho no debe diseñarse como un simple:

```text
Language
 ├── Name
 ├── Population
 └── Translation
```

Debe diseñarse como un **sistema de conocimiento lingüístico trazable**:

```text
ENTIDAD
   +
RELACIÓN
   +
FUENTE
   +
EVIDENCIA
   +
ESTADO
   +
VERSIONADO
```

Esta arquitectura permite que AtlasBuho crezca sin tener que rediseñar la BD cuando aparezcan:

* nuevas variantes;
* nuevas fuentes;
* nuevas autodenominaciones;
* nuevas comunidades;
* nuevos códigos;
* nuevos lexemas;
* nuevos idiomas destino;
* nuevas evidencias;
* conflictos documentales;
* datos de audio;
* modelos de IA;
* nuevas fuentes gubernamentales o académicas.

---

# 67. Regla final

**La BD debe almacenar lo que está documentado, no lo que creemos que debería decir.**

La interpretación, normalización, equivalencia o generación automática debe permanecer separada del registro documental original.

La fuente primaria manda.

La evidencia manda.

La reconciliación decide qué entra al catálogo canónico.

La IA puede asistir.

La IA no decide el catálogo.
