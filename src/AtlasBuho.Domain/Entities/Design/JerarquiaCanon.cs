namespace AtlasBuho.Domain.Entities.Design;

/// <summary>
/// Espejo canónico de FamiliaLinguistica según BD.MD §4.
/// No reemplaza LanguageFamily; añade EstadoVerificacion y FuenteId trazable.
/// </summary>
public class FamiliaCanon
{
    public Guid Id { get; set; }
    public Guid LanguageFamilyId { get; set; }
    public Guid? FuenteId { get; set; }
    public EstadoVerificacion EstadoVerificacion { get; set; } = EstadoVerificacion.Unverified;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
    public string? DatosAdicionalesJson { get; set; }
}

/// <summary>
/// Espejo canónico de AgrupacionLinguistica según BD.MD §5.
/// </summary>
public class AgrupacionCanon
{
    public Guid Id { get; set; }
    public Guid LanguageGroupId { get; set; }
    public Guid? FuenteId { get; set; }
    public EstadoVerificacion EstadoVerificacion { get; set; } = EstadoVerificacion.Unverified;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
    public string? DatosAdicionalesJson { get; set; }
}

/// <summary>
/// Espejo canónico de VarianteLinguistica según BD.MD §6-§7.
/// NombreOficial se conserva literalmente; códigos van a CodigoLinguisticoCanon.
/// </summary>
public class VarianteCanon
{
    public Guid Id { get; set; }
    public Guid LanguageVariantId { get; set; }
    public Guid? FuentePrincipalId { get; set; }
    public string? PaginaFuentePrincipal { get; set; }
    public EstadoVerificacion EstadoVerificacion { get; set; } = EstadoVerificacion.Unverified;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
    public string? Observaciones { get; set; }
}
