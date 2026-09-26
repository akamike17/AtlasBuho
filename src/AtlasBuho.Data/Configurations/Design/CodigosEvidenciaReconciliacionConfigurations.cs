using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AtlasBuho.Domain.Entities.Design;

namespace AtlasBuho.Data.Configurations.Design;

public class CanonCodigoLinguisticoConfiguration : IEntityTypeConfiguration<CodigoLinguisticoCanon>
{
    public void Configure(EntityTypeBuilder<CodigoLinguisticoCanon> b)
    {
        b.ToTable("CanonCodigosLinguisticos");
        b.Property(x => x.Id).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.VarianteLinguisticaId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.Sistema).HasConversion<string>().HasMaxLength(50);
        b.Property(x => x.Codigo).HasMaxLength(100).IsRequired();
        b.Property(x => x.FuenteId).HasColumnType("CHAR(36)");
        b.Property(x => x.EstadoVerificacion).HasConversion<string>().HasMaxLength(20);
        b.Property(x => x.Observaciones).HasColumnType("TEXT");
        b.HasIndex(x => new { x.VarianteLinguisticaId, x.Sistema, x.Codigo }).IsUnique();
        b.HasOne(x => x.Variante).WithMany().HasForeignKey(x => x.VarianteLinguisticaId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Domain.Entities.Source>().WithMany().HasForeignKey(x => x.FuenteId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class CanonEvidenciaVarianteConfiguration : IEntityTypeConfiguration<EvidenciaVarianteCanon>
{
    public void Configure(EntityTypeBuilder<EvidenciaVarianteCanon> b)
    {
        b.ToTable("CanonEvidenciaVariante");
        b.Property(x => x.Id).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.VarianteLinguisticaId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.FuenteId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.Pagina).HasMaxLength(100);
        b.Property(x => x.Seccion).HasMaxLength(200);
        b.Property(x => x.TextoEvidencia).HasColumnType("TEXT");
        b.Property(x => x.Ubicacion).HasMaxLength(500);
        b.Property(x => x.HashEvidencia).HasMaxLength(128);
        b.Property(x => x.EstadoVerificacion).HasConversion<string>().HasMaxLength(20);
        b.Property(x => x.Observaciones).HasColumnType("TEXT");
        b.HasOne(x => x.Variante).WithMany().HasForeignKey(x => x.VarianteLinguisticaId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Domain.Entities.Source>().WithMany().HasForeignKey(x => x.FuenteId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class CanonEvidenciaFamiliaConfiguration : IEntityTypeConfiguration<EvidenciaFamiliaCanon>
{
    public void Configure(EntityTypeBuilder<EvidenciaFamiliaCanon> b)
    {
        b.ToTable("CanonEvidenciaFamilia");
        b.Property(x => x.Id).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.FamiliaLinguisticaId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.FuenteId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.Pagina).HasMaxLength(100);
        b.Property(x => x.Seccion).HasMaxLength(200);
        b.Property(x => x.TextoEvidencia).HasColumnType("TEXT");
        b.Property(x => x.Ubicacion).HasMaxLength(500);
        b.Property(x => x.HashEvidencia).HasMaxLength(128);
        b.Property(x => x.EstadoVerificacion).HasConversion<string>().HasMaxLength(20);
        b.Property(x => x.Observaciones).HasColumnType("TEXT");
        b.HasOne(x => x.Familia).WithMany().HasForeignKey(x => x.FamiliaLinguisticaId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Domain.Entities.Source>().WithMany().HasForeignKey(x => x.FuenteId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class CanonEvidenciaAgrupacionConfiguration : IEntityTypeConfiguration<EvidenciaAgrupacionCanon>
{
    public void Configure(EntityTypeBuilder<EvidenciaAgrupacionCanon> b)
    {
        b.ToTable("CanonEvidenciaAgrupacion");
        b.Property(x => x.Id).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.AgrupacionLinguisticaId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.FuenteId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.Pagina).HasMaxLength(100);
        b.Property(x => x.Seccion).HasMaxLength(200);
        b.Property(x => x.TextoEvidencia).HasColumnType("TEXT");
        b.Property(x => x.Ubicacion).HasMaxLength(500);
        b.Property(x => x.HashEvidencia).HasMaxLength(128);
        b.Property(x => x.EstadoVerificacion).HasConversion<string>().HasMaxLength(20);
        b.Property(x => x.Observaciones).HasColumnType("TEXT");
        b.HasOne(x => x.Agrupacion).WithMany().HasForeignKey(x => x.AgrupacionLinguisticaId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Domain.Entities.Source>().WithMany().HasForeignKey(x => x.FuenteId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class CanonEvidenciaCodigoConfiguration : IEntityTypeConfiguration<EvidenciaCodigoCanon>
{
    public void Configure(EntityTypeBuilder<EvidenciaCodigoCanon> b)
    {
        b.ToTable("CanonEvidenciaCodigo");
        b.Property(x => x.Id).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.CodigoLinguisticoId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.FuenteId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.Pagina).HasMaxLength(100);
        b.Property(x => x.Seccion).HasMaxLength(200);
        b.Property(x => x.TextoEvidencia).HasColumnType("TEXT");
        b.Property(x => x.Ubicacion).HasMaxLength(500);
        b.Property(x => x.HashEvidencia).HasMaxLength(128);
        b.Property(x => x.EstadoVerificacion).HasConversion<string>().HasMaxLength(20);
        b.Property(x => x.Observaciones).HasColumnType("TEXT");
        b.HasOne(x => x.Codigo).WithMany().HasForeignKey(x => x.CodigoLinguisticoId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Domain.Entities.Source>().WithMany().HasForeignKey(x => x.FuenteId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class CanonEvidenciaPoblacionConfiguration : IEntityTypeConfiguration<EvidenciaPoblacionCanon>
{
    public void Configure(EntityTypeBuilder<EvidenciaPoblacionCanon> b)
    {
        b.ToTable("CanonEvidenciaPoblacion");
        b.Property(x => x.Id).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.PoblacionId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.FuenteId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.Pagina).HasMaxLength(100);
        b.Property(x => x.Seccion).HasMaxLength(200);
        b.Property(x => x.TextoEvidencia).HasColumnType("TEXT");
        b.Property(x => x.Ubicacion).HasMaxLength(500);
        b.Property(x => x.HashEvidencia).HasMaxLength(128);
        b.Property(x => x.EstadoVerificacion).HasConversion<string>().HasMaxLength(20);
        b.Property(x => x.Observaciones).HasColumnType("TEXT");
        b.HasOne(x => x.Poblacion).WithMany().HasForeignKey(x => x.PoblacionId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Domain.Entities.Source>().WithMany().HasForeignKey(x => x.FuenteId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class CanonEvidenciaLocalidadConfiguration : IEntityTypeConfiguration<EvidenciaLocalidadCanon>
{
    public void Configure(EntityTypeBuilder<EvidenciaLocalidadCanon> b)
    {
        b.ToTable("CanonEvidenciaLocalidad");
        b.Property(x => x.Id).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.LocalidadId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.FuenteId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.Pagina).HasMaxLength(100);
        b.Property(x => x.Seccion).HasMaxLength(200);
        b.Property(x => x.TextoEvidencia).HasColumnType("TEXT");
        b.Property(x => x.Ubicacion).HasMaxLength(500);
        b.Property(x => x.HashEvidencia).HasMaxLength(128);
        b.Property(x => x.EstadoVerificacion).HasConversion<string>().HasMaxLength(20);
        b.Property(x => x.Observaciones).HasColumnType("TEXT");
        b.HasOne(x => x.Localidad).WithMany().HasForeignKey(x => x.LocalidadId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Domain.Entities.Source>().WithMany().HasForeignKey(x => x.FuenteId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class CanonReconciliationResultConfiguration : IEntityTypeConfiguration<ReconciliationResultCanon>
{
    public void Configure(EntityTypeBuilder<ReconciliationResultCanon> b)
    {
        b.ToTable("CanonReconciliationResults");
        b.Property(x => x.Id).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.CatalogRecordId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.FuenteId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.IdentificadorFuente).HasMaxLength(200);
        b.Property(x => x.NombreFuente).HasMaxLength(300);
        b.Property(x => x.VarianteLinguisticaId).HasColumnType("CHAR(36)");
        b.Property(x => x.Resultado).HasConversion<string>().HasMaxLength(50);
        b.Property(x => x.EvidenciaId).HasColumnType("CHAR(36)");
        b.Property(x => x.Observaciones).HasColumnType("TEXT");
        b.HasIndex(x => new { x.CatalogRecordId, x.FuenteId }).IsUnique();
        b.HasOne<Domain.Entities.CatalogRecord>().WithMany().HasForeignKey(x => x.CatalogRecordId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Domain.Entities.Source>().WithMany().HasForeignKey(x => x.FuenteId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Variante).WithMany().HasForeignKey(x => x.VarianteLinguisticaId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class CanonAuditoriaConfiguration : IEntityTypeConfiguration<AuditoriaCanon>
{
    public void Configure(EntityTypeBuilder<AuditoriaCanon> b)
    {
        b.ToTable("CanonAuditorias");
        b.Property(x => x.Id).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.Entidad).HasMaxLength(200).IsRequired();
        b.Property(x => x.EntidadId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.Operacion).HasConversion<string>().HasMaxLength(50);
        b.Property(x => x.Usuario).HasMaxLength(200).IsRequired();
        b.Property(x => x.AntesJson).HasColumnType("TEXT");
        b.Property(x => x.DespuesJson).HasColumnType("TEXT");
        b.Property(x => x.Motivo).HasColumnType("TEXT");
        b.Property(x => x.FuenteId).HasColumnType("CHAR(36)");
    }
}

public class CanonCorrespondenciaFuenteConfiguration : IEntityTypeConfiguration<CorrespondenciaFuenteCanon>
{
    public void Configure(EntityTypeBuilder<CorrespondenciaFuenteCanon> b)
    {
        b.ToTable("CanonCorrespondenciasFuente");
        b.Property(x => x.Id).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.FuenteOrigenId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.EntidadOrigen).HasMaxLength(200).IsRequired();
        b.Property(x => x.EntidadOrigenId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.FuenteDestinoId).HasColumnType("CHAR(36)");
        b.Property(x => x.EntidadDestino).HasMaxLength(200);
        b.Property(x => x.EntidadDestinoId).HasColumnType("CHAR(36)");
        b.Property(x => x.TipoCorrespondencia).HasConversion<string>().HasMaxLength(50);
        b.Property(x => x.FuenteEvidenciaId).HasColumnType("CHAR(36)");
        b.Property(x => x.EstadoVerificacion).HasConversion<string>().HasMaxLength(20);
        b.Property(x => x.Observaciones).HasColumnType("TEXT");
    }
}

public class CanonImportRecordConfiguration : IEntityTypeConfiguration<ImportRecordCanon>
{
    public void Configure(EntityTypeBuilder<ImportRecordCanon> b)
    {
        b.ToTable("CanonImportRecords");
        b.Property(x => x.Id).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.ImportBatchId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.NumeroRegistro).IsRequired();
        b.Property(x => x.TextoOriginal).HasColumnType("TEXT").IsRequired();
        b.Property(x => x.DatosExtraidosJson).HasColumnType("TEXT");
        b.Property(x => x.Estado).HasConversion<string>().HasMaxLength(50);
        b.Property(x => x.MotivoRechazo).HasMaxLength(1000);
        b.HasIndex(x => new { x.ImportBatchId, x.NumeroRegistro }).IsUnique();
    }
}

public class CanonFraseTraduccionConfiguration : IEntityTypeConfiguration<FraseTraduccionCanon>
{
    public void Configure(EntityTypeBuilder<FraseTraduccionCanon> b)
    {
        b.ToTable("CanonFraseTraduccion");
        b.Property(x => x.Id).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.FraseOrigenId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.FraseDestinoId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.FuenteId).HasColumnType("CHAR(36)").IsRequired();
        b.Property(x => x.EstadoVerificacion).HasConversion<string>().HasMaxLength(20);
        b.HasOne<Domain.Entities.Phrase>().WithMany().HasForeignKey(x => x.FraseOrigenId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Domain.Entities.Phrase>().WithMany().HasForeignKey(x => x.FraseDestinoId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Domain.Entities.Source>().WithMany().HasForeignKey(x => x.FuenteId).OnDelete(DeleteBehavior.Restrict);
    }
}
