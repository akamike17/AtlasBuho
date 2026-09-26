namespace AtlasBuho.Domain.Entities.Design;

/// <summary>Tabla geográfica normalizada según BD.MD §13. Sustituye al string plano Community.State.</summary>
public class PaisCanon
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Codigo { get; set; }
    public EstadoVerificacion EstadoVerificacion { get; set; } = EstadoVerificacion.Unverified;
    public Guid? FuenteId { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
    public virtual List<EstadoCanon> Estados { get; set; } = new();
}

public class EstadoCanon
{
    public Guid Id { get; set; }
    public Guid PaisId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Codigo { get; set; }
    public EstadoVerificacion EstadoVerificacion { get; set; } = EstadoVerificacion.Unverified;
    public Guid? FuenteId { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
    public virtual PaisCanon? Pais { get; set; }
    public virtual List<MunicipioCanon> Municipios { get; set; } = new();
}

public class MunicipioCanon
{
    public Guid Id { get; set; }
    public Guid EstadoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Codigo { get; set; }
    public EstadoVerificacion EstadoVerificacion { get; set; } = EstadoVerificacion.Unverified;
    public Guid? FuenteId { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
    public virtual EstadoCanon? Estado { get; set; }
    public virtual List<LocalidadCanon> Localidades { get; set; } = new();
}

public class LocalidadCanon
{
    public Guid Id { get; set; }
    public Guid MunicipioId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Codigo { get; set; }
    public LocalidadTipo Tipo { get; set; } = LocalidadTipo.LOCALIDAD;
    public decimal? Latitud { get; set; }
    public decimal? Longitud { get; set; }
    public EstadoVerificacion EstadoVerificacion { get; set; } = EstadoVerificacion.Unverified;
    public Guid? FuenteId { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
    public virtual MunicipioCanon? Municipio { get; set; }
}

/// <summary>Población hablante documentada según BD.MD §11. Separada de Localidad.</summary>
public class PoblacionCanon
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? NombreAlternativo { get; set; }
    public PoblacionTipo Tipo { get; set; } = PoblacionTipo.OTRO_DOCUMENTADO;
    public EstadoVerificacion EstadoVerificacion { get; set; } = EstadoVerificacion.Unverified;
    public Guid? FuenteId { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
    public string? Observaciones { get; set; }
}

/// <summary>M:N Variante↔Población según BD.MD §12.</summary>
public class VariantePoblacionCanon
{
    public Guid Id { get; set; }
    public Guid VarianteLinguisticaId { get; set; }
    public Guid PoblacionId { get; set; }
    public RelacionTipo Relacion { get; set; } = RelacionTipo.OTRA;
    public EstadoVerificacion EstadoVerificacion { get; set; } = EstadoVerificacion.Unverified;
    public Guid? FuenteId { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
    public string? Observaciones { get; set; }
    public virtual VarianteCanon? Variante { get; set; }
    public virtual PoblacionCanon? Poblacion { get; set; }
}

/// <summary>M:N Población↔Localidad según BD.MD §14.</summary>
public class PoblacionLocalidadCanon
{
    public Guid Id { get; set; }
    public Guid PoblacionId { get; set; }
    public Guid LocalidadId { get; set; }
    public RelacionTipo Relacion { get; set; } = RelacionTipo.OTRA;
    public EstadoVerificacion EstadoVerificacion { get; set; } = EstadoVerificacion.Unverified;
    public Guid? FuenteId { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
    public string? Observaciones { get; set; }
    public virtual PoblacionCanon? Poblacion { get; set; }
    public virtual LocalidadCanon? Localidad { get; set; }
}

/// <summary>Variante↔Localidad directa según BD.MD §15 (evidencia directa).</summary>
public class VarianteLocalidadCanon
{
    public Guid Id { get; set; }
    public Guid VarianteLinguisticaId { get; set; }
    public Guid LocalidadId { get; set; }
    public RelacionTipo Relacion { get; set; } = RelacionTipo.OTRA;
    public EstadoVerificacion EstadoVerificacion { get; set; } = EstadoVerificacion.Unverified;
    public Guid? FuenteId { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
    public string? Observaciones { get; set; }
    public virtual VarianteCanon? Variante { get; set; }
    public virtual LocalidadCanon? Localidad { get; set; }
}
