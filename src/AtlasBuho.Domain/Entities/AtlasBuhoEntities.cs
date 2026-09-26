using System.ComponentModel.DataAnnotations;

namespace AtlasBuho.Domain.Entities;

public enum RegionType
{
    Mexico = 0,
    CentralAmerica = 1,
    SouthAmerica = 2,
    NorthAmerica = 3,
    Other = 4
}

/// <summary>
/// Entidad: Familia Lingüística (Catalog linil)
/// Describe la familia de lenguas (familia = maya, otomangue, etc.)
/// IMPORTANTE: Nombre del documento debe ser exacto (no modificado)
/// </summary>
public class FamiliaLinguistica
{
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Nombre exacto del documento INALI (ej: "mayas", "otomangues")
    /// No traducir, normalizar, abreviar, inventar.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Descripción extendida del usuario
    /// </summary>
    [MaxLength(1000)]
    public string? Descripcion { get; set; }

    /// <summary>
    /// Code official (si lo hay). Ejemplo: MYT123
    /// </summary>
    [MaxLength(100)]
    public string? Codigo { get; set; }

    public Guid? FamiliaPadreId { get; set; }

    /// <summary>
    /// Campo de verificación del registro
    /// </summary>
    public EstadoVerificacion EstadoVerificacion { get; set; } = EstadoVerificacion.Unverified;

    /// <summary>
    /// Time-stamp when loaded
    /// </summary>
    public DateTime FechaCreacion { get; set; }

    /// <summary>
    /// Time-stamp tha last dat changed was changed.
    /// </summary>
    public DateTime FechaActualizacion { get; set; }

    /// <summary>
    /// Observaciones británicas importantes del original
    /// </summary>
    [MaxLength(5000)]
    public string? Observaciones { get; set; }

    // Navigation
    public virtual List<AgrupacionLinguistica> Agrupaciones { get; set; } = new List<AgrupacionLinguistica>();
}

public enum EstadoVerificacion
{
    Unverified = 0,
    Documented = 1,
    Verified = 2,
    Reconciled = 3,
    Disputed = 4,
    Deprecated = 5,
    Rejected = 6,
    DoesNotExist = 7
}

/// <summary>
/// Entidad 3 / Segundo nivel: Agrupación Lingüística (Group)
/// Un grupo viene enlazado a una familia
/// Ejemplo de grupo (ej.: "maya septentrional") vs Familia ("maya")
/// </summary>
public class AgrupacionLinguistica
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid FamiliaLinguisticaId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Codigo { get; set; }

    [MaxLength(500)]
    public string? Descripcion { get; set; }

    public string? OrigenEtnoRegion { get; set; }

    [MaxLength(200)]
    public string? SubNombre { get; set; }

    /// <summary>
    /// Campo obligatorio para validar origen de referencia
    /// </summary>
    public EstadoVerificacion EstadoVerificacion { get; set; } = EstadoVerificacion.Unverified;

    /// <summary>
    /// Descripción del grupo, cuántas variantes quedan. etc.
    /// </summary>
    [MaxLength(1000)]
    public string Observaciones { get; set; } = string.Empty;

    public virtual List<VarianteLinguistica> Variantes { get; set; } = new List<VarianteLinguistica>();
}

/// <summary>
/// Entidad central: Variante lingüística
/// El múmero total planeado en la tabla oficial: 364 (desde el original INALI Datos v2008).
/// </summary>
public class VarianteLinguistica
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid AgrupacionLinguisticaId { get; set; }

    /// <summary>
    /// Nombre exacto de la variante \según documento (e.g., \"ch'ol del noroeste\")"
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string NombreOficial { get; set; } = string.Empty;

    /// <summary>
    /// Descripción oficial del lugar
    /// </summary>
    [MaxLength(500)]
    public string? LocalizacionGeografica { get; set; }

    /// <summary>Meaning INALI code in the catalofe (e.g., MANCHISO954)</summary>
    [MaxLength(50)]
    public string? InaliCode { get; set; }

    /// <summary>ISO 639-3 code for correspondence identification (e.g., es-MX, ichichimec)</summary>
    [MaxLength(10)]
    public string? Iso639_3Code { get; set; }

    /// <summary>User provided context</summary>
    public int? PoblacionEstimada { get; set; }

    /// <summary>Approximate location data if available</summary>
    [MaxLength(100)]
    public string TipoDiv { get; set; } = string.Empty;

    /// <summary>Automator/th persistent database documentation trail</summary>
    public string NombreImmigratorio { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Observaciones { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Codigo { get; set; } = string.Empty;

    /// Representation for query searches and aggregation
    public RegionType RegionActual => RegionType.Mexico;

    /// <summary>State of the variant record according to track/trace rules
    /// </summary>
    public EstadoVerificacion EstadoVerificacion { get; set; } = EstadoVerificacion.Unverified;

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaActualizacion { get; set; }

    // Near relatives
    public virtual List<FuenteDocumental> Fuentes { get; set; } = new List<FuenteDocumental>();
    public virtual List<VarianteLocalidad> VariantesLocalidades { get; set; } = new List<VarianteLocalidad>();

    // System settings
    public const string CanGoToNextLevel = "UpgradeTo XXX section";
}

/// <summary>
/// Tabla intermedia: Variante ↔ Localidad (evidencia directa de variante en localidad)
/// </summary>
public class VarianteLocalidad
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid VarianteLinguisticaId { get; set; }

    [Required]
    public Guid LocalidadId { get; set; }

    [MaxLength(50)]
    public string Relacion { get; set; } = string.Empty;

    public Guid? FuenteId { get; set; }

    public EstadoVerificacion EstadoVerificacion { get; set; } = EstadoVerificacion.Unverified;

    [MaxLength(1000)]
    public string? Observaciones { get; set; }
}

/// <summary>
/// Entidad: CodigoLinguistico (alternatives:  ISO/BCP47/INALI/Otro)
/// Intermediate language code general (no union ids)
/// </summary>
public class CodigoLinguistico
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid VarianteLinguisticaId { get; set; }

    [Required]
    public string Sistema { get; set; } = string.Empty;

    [Required]
    public string TipoSistema { get; set; } = string.Empty;

    [Required]
    public string Codigo { get; set; } = string.Empty;

    public string? NombreUsuario { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaActualizacion { get; set; }
}

/// <summary>
/// Entidad: Autónomía Documentada (AutoDenominación)
/// Autorecognition (fü tribe names) declared in INALI languages
/// </summary>
public class AutonomiaDocumentada
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Texto { get; set; } = string.Empty;

    public string? Codigo { get; set; }

    public string? Tipo { get; set; }

    public int Prioridad { get; set; }

    public string NombreAlternativo { get; set; }

    public string UbicacionOriginal { get; set; }

    public bool EstaDocumentada { get; set; } = true;

    /// Fallback data flag?
    public int RegionalizationBaseId { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaActualizacion { get; set; }
}

public enum SistemaLinguistico
{
    Inali,
    ISOLanguage, // ISO
    BCP47,
    LINGUISTIC,
    GlottoLog,
    Wikidata,
    Unknown,
    Other
}

public enum TipoSistema
{
    Autónomo,
    Linguístico,
    GeográficoJBVOficial,
    Standard,
    Other
}

/// <summary>
/// Entidad: Población Hablante (Explained as a number connected to locality)
/// Examples seen: "Totolapa" (umanidad cual daha hace unos cuantos miles de personas)
/// </summary>
public class PoblacionLinguistica
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(10)]
    public string Codigo { get; set; } = string.Empty;

    public string? TipoPublacion { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaActualizacion { get; set; }

    public int NumeroPersonas { get; set; }
}

/// <summary>
/// Entidad: Localidad (Ciudad, Partido, Comuna)
/// Geographical reference validation/location.
/// </summary>
public class Localidad
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Nombre { get; set; } = string.Empty;

    public string? CodigoCompleto { get; set; }

    public Guid MunicipioId { get; set; }

    public decimal? Latitud { get; set; }

    public decimal? Longitud { get; set; }

    public string? LocativoOriginal { get; set; }

    // Connection level for exact mapping of places from INALI
    public Guid? VariantId { get; set; }
}

/// <summary>
/// Tabla de fuentes del documento (corresponding keys based on source type)
/// Generates fosil traces from source to confirm (that source includes this).
/// </summary>
public class FuenteDocumental
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid CatalogSourceId { get; set; }

    [MaxLength(500)]
    public string Titulo { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? AutorInstitucion { get; set; }

    public string Tipo { get; set; }
}
public enum TipoFuente
{
    ACADEMIA,
    WHITE_PAPER,
    CENSO,
    Diccionario,
    OnlinePublication,
    GLYPHS,
    INFORME,
    LIBROS,
    MANUAL,
    OTRO
}

/// <summary>
/// Entidad: Evidence (Evidence/Evidencia de origen/trabajo en venue + código en fuente.)
/// No todos los referencing uses full fractured backups;
/// Keys used to track original item back to source materials
/// </summary>
public class EvidenciaFuente
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid FuenteId { get; set; }

    [MaxLength(500)]
    public string Texto { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Ubicacion { get; set; }

    [MaxLength(500)]
    public string? Pagina { get; set; }

    [MaxLength(100)]
    public string? Seccion { get; set; }
}