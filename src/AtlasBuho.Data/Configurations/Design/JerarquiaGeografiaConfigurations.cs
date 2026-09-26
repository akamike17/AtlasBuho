using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities.Design;

namespace AtlasBuho.Data.Configurations.Design;

public class CanonFamiliaConfiguration : IEntityTypeConfiguration<FamiliaCanon>
{
    public void Configure(EntityTypeBuilder<FamiliaCanon> b)
    {
        b.ToTable("CanonFamilias");
        b.Property(x => x.Id).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.LanguageFamilyId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.FuenteId).HasColumnType("CHAR(36)");
        b.Property(x => x.EstadoVerificacion).HasConversion<string>().HasMaxLength(20);
        b.Property(x => x.DatosAdicionalesJson).HasColumnType("TEXT");
        b.HasOne<Domain.Entities.LanguageFamily>().WithMany().HasForeignKey(x => x.LanguageFamilyId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Domain.Entities.Source>().WithMany().HasForeignKey(x => x.FuenteId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class CanonAgrupacionConfiguration : IEntityTypeConfiguration<AgrupacionCanon>
{
    public void Configure(EntityTypeBuilder<AgrupacionCanon> b)
    {
        b.ToTable("CanonAgrupaciones");
        b.Property(x => x.Id).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.LanguageGroupId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.FuenteId).HasColumnType("CHAR(36)");
        b.Property(x => x.EstadoVerificacion).HasConversion<string>().HasMaxLength(20);
        b.Property(x => x.DatosAdicionalesJson).HasColumnType("TEXT");
        b.HasOne<Domain.Entities.LanguageGroup>().WithMany().HasForeignKey(x => x.LanguageGroupId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Domain.Entities.Source>().WithMany().HasForeignKey(x => x.FuenteId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class CanonVarianteConfiguration : IEntityTypeConfiguration<VarianteCanon>
{
    public void Configure(EntityTypeBuilder<VarianteCanon> b)
    {
        b.ToTable("CanonVariantes");
        b.Property(x => x.Id).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.LanguageVariantId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.FuentePrincipalId).HasColumnType("CHAR(36)");
        b.Property(x => x.PaginaFuentePrincipal).HasMaxLength(100);
        b.Property(x => x.EstadoVerificacion).HasConversion<string>().HasMaxLength(20);
        b.Property(x => x.Observaciones).HasColumnType("TEXT");
        b.HasOne<Domain.Entities.LanguageVariant>().WithMany().HasForeignKey(x => x.LanguageVariantId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Domain.Entities.Source>().WithMany().HasForeignKey(x => x.FuentePrincipalId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class CanonPaisConfiguration : IEntityTypeConfiguration<PaisCanon>
{
    public void Configure(EntityTypeBuilder<PaisCanon> b)
    {
        b.ToTable("CanonPaises");
        b.Property(x => x.Id).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.Nombre).HasMaxLength(200).IsRequired();
        b.Property(x => x.Codigo).HasMaxLength(50);
        b.Property(x => x.EstadoVerificacion).HasConversion<string>().HasMaxLength(20);
        b.Property(x => x.FuenteId).HasColumnType("CHAR(36)");
        b.HasOne<Domain.Entities.Source>().WithMany().HasForeignKey(x => x.FuenteId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class CanonEstadoConfiguration : IEntityTypeConfiguration<EstadoCanon>
{
    public void Configure(EntityTypeBuilder<EstadoCanon> b)
    {
        b.ToTable("CanonEstados");
        b.Property(x => x.Id).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.PaisId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.Nombre).HasMaxLength(200).IsRequired();
        b.Property(x => x.Codigo).HasMaxLength(50);
        b.Property(x => x.EstadoVerificacion).HasConversion<string>().HasMaxLength(20);
        b.Property(x => x.FuenteId).HasColumnType("CHAR(36)");
        b.HasOne<Domain.Entities.Source>().WithMany().HasForeignKey(x => x.FuenteId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Pais).WithMany(p => p.Estados).HasForeignKey(x => x.PaisId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class CanonMunicipioConfiguration : IEntityTypeConfiguration<MunicipioCanon>
{
    public void Configure(EntityTypeBuilder<MunicipioCanon> b)
    {
        b.ToTable("CanonMunicipios");
        b.Property(x => x.Id).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.EstadoId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.Nombre).HasMaxLength(200).IsRequired();
        b.Property(x => x.Codigo).HasMaxLength(50);
        b.Property(x => x.EstadoVerificacion).HasConversion<string>().HasMaxLength(20);
        b.Property(x => x.FuenteId).HasColumnType("CHAR(36)");
        b.HasOne<Domain.Entities.Source>().WithMany().HasForeignKey(x => x.FuenteId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Estado).WithMany(e => e.Municipios).HasForeignKey(x => x.EstadoId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class CanonLocalidadConfiguration : IEntityTypeConfiguration<LocalidadCanon>
{
    public void Configure(EntityTypeBuilder<LocalidadCanon> b)
    {
        b.ToTable("CanonLocalidades");
        b.Property(x => x.Id).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.MunicipioId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.Nombre).HasMaxLength(300).IsRequired();
        b.Property(x => x.Codigo).HasMaxLength(50);
        b.Property(x => x.Tipo).HasConversion<string>().HasMaxLength(50);
        b.Property(x => x.Latitud).HasPrecision(10, 7);
        b.Property(x => x.Longitud).HasPrecision(10, 7);
        b.Property(x => x.EstadoVerificacion).HasConversion<string>().HasMaxLength(20);
        b.Property(x => x.FuenteId).HasColumnType("CHAR(36)");
        b.HasOne<Domain.Entities.Source>().WithMany().HasForeignKey(x => x.FuenteId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Municipio).WithMany(m => m.Localidades).HasForeignKey(x => x.MunicipioId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class CanonPoblacionConfiguration : IEntityTypeConfiguration<PoblacionCanon>
{
    public void Configure(EntityTypeBuilder<PoblacionCanon> b)
    {
        b.ToTable("CanonPoblaciones");
        b.Property(x => x.Id).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.Nombre).HasMaxLength(300).IsRequired();
        b.Property(x => x.NombreAlternativo).HasMaxLength(300);
        b.Property(x => x.Tipo).HasConversion<string>().HasMaxLength(50);
        b.Property(x => x.EstadoVerificacion).HasConversion<string>().HasMaxLength(20);
        b.Property(x => x.FuenteId).HasColumnType("CHAR(36)");
        b.Property(x => x.Observaciones).HasColumnType("TEXT");
        b.HasOne<Domain.Entities.Source>().WithMany().HasForeignKey(x => x.FuenteId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class CanonVariantePoblacionConfiguration : IEntityTypeConfiguration<VariantePoblacionCanon>
{
    public void Configure(EntityTypeBuilder<VariantePoblacionCanon> b)
    {
        b.ToTable("CanonVariantePoblacion");
        b.Property(x => x.Id).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.VarianteLinguisticaId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.PoblacionId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.Relacion).HasConversion<string>().HasMaxLength(50);
        b.Property(x => x.EstadoVerificacion).HasConversion<string>().HasMaxLength(20);
        b.Property(x => x.FuenteId).HasColumnType("CHAR(36)");
        b.Property(x => x.Observaciones).HasColumnType("TEXT");
        b.HasIndex(x => new { x.VarianteLinguisticaId, x.PoblacionId }).IsUnique();
        b.HasOne(x => x.Variante).WithMany().HasForeignKey(x => x.VarianteLinguisticaId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Poblacion).WithMany().HasForeignKey(x => x.PoblacionId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Domain.Entities.Source>().WithMany().HasForeignKey(x => x.FuenteId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class CanonPoblacionLocalidadConfiguration : IEntityTypeConfiguration<PoblacionLocalidadCanon>
{
    public void Configure(EntityTypeBuilder<PoblacionLocalidadCanon> b)
    {
        b.ToTable("CanonPoblacionLocalidad");
        b.Property(x => x.Id).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.PoblacionId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.LocalidadId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.Relacion).HasConversion<string>().HasMaxLength(50);
        b.Property(x => x.EstadoVerificacion).HasConversion<string>().HasMaxLength(20);
        b.Property(x => x.FuenteId).HasColumnType("CHAR(36)");
        b.Property(x => x.Observaciones).HasColumnType("TEXT");
        b.HasIndex(x => new { x.PoblacionId, x.LocalidadId }).IsUnique();
        b.HasOne(x => x.Poblacion).WithMany().HasForeignKey(x => x.PoblacionId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Localidad).WithMany().HasForeignKey(x => x.LocalidadId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Domain.Entities.Source>().WithMany().HasForeignKey(x => x.FuenteId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class CanonVarianteLocalidadConfiguration : IEntityTypeConfiguration<VarianteLocalidadCanon>
{
    public void Configure(EntityTypeBuilder<VarianteLocalidadCanon> b)
    {
        b.ToTable("CanonVarianteLocalidad");
        b.Property(x => x.Id).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.VarianteLinguisticaId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.LocalidadId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.Relacion).HasConversion<string>().HasMaxLength(50);
        b.Property(x => x.EstadoVerificacion).HasConversion<string>().HasMaxLength(20);
        b.Property(x => x.FuenteId).HasColumnType("CHAR(36)");
        b.Property(x => x.Observaciones).HasColumnType("TEXT");
        b.HasIndex(x => new { x.VarianteLinguisticaId, x.LocalidadId }).IsUnique();
        b.HasOne(x => x.Variante).WithMany().HasForeignKey(x => x.VarianteLinguisticaId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Localidad).WithMany().HasForeignKey(x => x.LocalidadId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Domain.Entities.Source>().WithMany().HasForeignKey(x => x.FuenteId).OnDelete(DeleteBehavior.Restrict);
    }
}
