namespace AtlasBuho.Domain.Entities.Design;

/// <summary>Código lingüístico como tabla separada según BD.MD §9.</summary>
public class CodigoLinguisticoCanon
{
    public Guid Id { get; set; }
    public Guid VarianteLinguisticaId { get; set; }
    public CodigoSistema Sistema { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public Guid? FuenteId { get; set; }
    public EstadoVerificacion EstadoVerificacion { get; set; } = EstadoVerificacion.Unverified;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
    public string? Observaciones { get; set; }
    public virtual VarianteCanon? Variante { get; set; }
}

/// <summary>Evidencia por entidad con FK real (PD-A11 opción B). Reemplaza Evidence.EntityType+EntityId polimórfico para entidades críticas.</summary>
public class EvidenciaVarianteCanon
{
    public Guid Id { get; set; }
    public Guid VarianteLinguisticaId { get; set; }
    public Guid FuenteId { get; set; }
    public string? Pagina { get; set; }
    public string? Seccion { get; set; }
    public string? TextoEvidencia { get; set; }
    public string? Ubicacion { get; set; }
    public string? HashEvidencia { get; set; }
    public DateTime FechaExtraccion { get; set; } = DateTime.UtcNow;
    public EstadoVerificacion EstadoVerificacion { get; set; } = EstadoVerificacion.Unverified;
    public string? Observaciones { get; set; }
    public virtual VarianteCanon? Variante { get; set; }
}

public class EvidenciaFamiliaCanon
{
    public Guid Id { get; set; }
    public Guid FamiliaLinguisticaId { get; set; }
    public Guid FuenteId { get; set; }
    public string? Pagina { get; set; }
    public string? Seccion { get; set; }
    public string? TextoEvidencia { get; set; }
    public string? Ubicacion { get; set; }
    public string? HashEvidencia { get; set; }
    public DateTime FechaExtraccion { get; set; } = DateTime.UtcNow;
    public EstadoVerificacion EstadoVerificacion { get; set; } = EstadoVerificacion.Unverified;
    public string? Observaciones { get; set; }
    public virtual FamiliaCanon? Familia { get; set; }
}

public class EvidenciaAgrupacionCanon
{
    public Guid Id { get; set; }
    public Guid AgrupacionLinguisticaId { get; set; }
    public Guid FuenteId { get; set; }
    public string? Pagina { get; set; }
    public string? Seccion { get; set; }
    public string? TextoEvidencia { get; set; }
    public string? Ubicacion { get; set; }
    public string? HashEvidencia { get; set; }
    public DateTime FechaExtraccion { get; set; } = DateTime.UtcNow;
    public EstadoVerificacion EstadoVerificacion { get; set; } = EstadoVerificacion.Unverified;
    public string? Observaciones { get; set; }
    public virtual AgrupacionCanon? Agrupacion { get; set; }
}

public class EvidenciaCodigoCanon
{
    public Guid Id { get; set; }
    public Guid CodigoLinguisticoId { get; set; }
    public Guid FuenteId { get; set; }
    public string? Pagina { get; set; }
    public string? Seccion { get; set; }
    public string? TextoEvidencia { get; set; }
    public string? Ubicacion { get; set; }
    public string? HashEvidencia { get; set; }
    public DateTime FechaExtraccion { get; set; } = DateTime.UtcNow;
    public EstadoVerificacion EstadoVerificacion { get; set; } = EstadoVerificacion.Unverified;
    public string? Observaciones { get; set; }
    public virtual CodigoLinguisticoCanon? Codigo { get; set; }
}

public class EvidenciaPoblacionCanon
{
    public Guid Id { get; set; }
    public Guid PoblacionId { get; set; }
    public Guid FuenteId { get; set; }
    public string? Pagina { get; set; }
    public string? Seccion { get; set; }
    public string? TextoEvidencia { get; set; }
    public string? Ubicacion { get; set; }
    public string? HashEvidencia { get; set; }
    public DateTime FechaExtraccion { get; set; } = DateTime.UtcNow;
    public EstadoVerificacion EstadoVerificacion { get; set; } = EstadoVerificacion.Unverified;
    public string? Observaciones { get; set; }
    public virtual PoblacionCanon? Poblacion { get; set; }
}

public class EvidenciaLocalidadCanon
{
    public Guid Id { get; set; }
    public Guid LocalidadId { get; set; }
    public Guid FuenteId { get; set; }
    public string? Pagina { get; set; }
    public string? Seccion { get; set; }
    public string? TextoEvidencia { get; set; }
    public string? Ubicacion { get; set; }
    public string? HashEvidencia { get; set; }
    public DateTime FechaExtraccion { get; set; } = DateTime.UtcNow;
    public EstadoVerificacion EstadoVerificacion { get; set; } = EstadoVerificacion.Unverified;
    public string? Observaciones { get; set; }
    public virtual LocalidadCanon? Localidad { get; set; }
}

/// <summary>ReconciliationResult según PD-A15 (corregido). FK explícita a CatalogRecord.</summary>
public class ReconciliationResultCanon
{
    public Guid Id { get; set; }
    public Guid CatalogRecordId { get; set; }  // NOT NULL — cierra 0..N
    public Guid FuenteId { get; set; }
    public string? IdentificadorFuente { get; set; }
    public string? NombreFuente { get; set; }
    public Guid? VarianteLinguisticaId { get; set; }  // NULL solo cuando Resultado = MISSING/NOT_APPLICABLE
    public ReconciliacionResultado Resultado { get; set; }
    public Guid? EvidenciaId { get; set; }
    public string? Observaciones { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public virtual VarianteCanon? Variante { get; set; }
    public virtual FamiliaCanon? Evidencia { get; set; }
}

/// <summary>Auditoría general según BD.MD §22.</summary>
public class AuditoriaCanon
{
    public Guid Id { get; set; }
    public string Entidad { get; set; } = string.Empty;
    public Guid EntidadId { get; set; }
    public AuditoriaOperacion Operacion { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public string? AntesJson { get; set; }
    public string? DespuesJson { get; set; }
    public string? Motivo { get; set; }
    public Guid? FuenteId { get; set; }
}

/// <summary>Correspondencia entre fuentes según BD.MD §48.</summary>
public class CorrespondenciaFuenteCanon
{
    public Guid Id { get; set; }
    public Guid FuenteOrigenId { get; set; }
    public string EntidadOrigen { get; set; } = string.Empty;
    public Guid EntidadOrigenId { get; set; }
    public Guid? FuenteDestinoId { get; set; }
    public string? EntidadDestino { get; set; }
    public Guid? EntidadDestinoId { get; set; }
    public TipoCorrespondencia TipoCorrespondencia { get; set; }
    public Guid? FuenteEvidenciaId { get; set; }
    public EstadoVerificacion EstadoVerificacion { get; set; } = EstadoVerificacion.Unverified;
    public string? Observaciones { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}

/// <summary>ImportRecord RAW según BD.MD §33. Antes de clasificación.</summary>
public class ImportRecordCanon
{
    public Guid Id { get; set; }
    public Guid ImportBatchId { get; set; }
    public int NumeroRegistro { get; set; }
    public string TextoOriginal { get; set; } = string.Empty;
    public string? DatosExtraidosJson { get; set; }
    public ImportRecordEstado Estado { get; set; } = ImportRecordEstado.RAW;
    public string? MotivoRechazo { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}

/// <summary>Traducción de frase según BD.MD §30. Bloqueada para Fase E.</summary>
public class FraseTraduccionCanon
{
    public Guid Id { get; set; }
    public Guid FraseOrigenId { get; set; }
    public Guid FraseDestinoId { get; set; }
    public Guid FuenteId { get; set; }
    public EstadoVerificacion EstadoVerificacion { get; set; } = EstadoVerificacion.Unverified;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
}
