using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Infrastructure.Data.Configurations;

public class ComentarioConfiguration : IEntityTypeConfiguration<Comentario>
{
    public void Configure(EntityTypeBuilder<Comentario> builder)
    {
        builder.ToTable(t =>
            t.HasCheckConstraint(
                "CK_Comentarios_Alvo",
                "ComunidadeId IS NOT NULL OR FilmeId IS NOT NULL"
            )
        );

        builder
            .HasOne(c => c.Usuario)
            .WithMany(u => u.Comentarios)
            .HasForeignKey(c => c.UsuarioId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(c => c.Comunidade)
            .WithMany(c => c.Comentarios)
            .HasForeignKey(c => c.ComunidadeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(c => c.Filme)
            .WithMany(f => f.Comentarios)
            .HasForeignKey(c => c.FilmeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ListaPessoalConfiguration : IEntityTypeConfiguration<ListaPessoal>
{
    public void Configure(EntityTypeBuilder<ListaPessoal> builder)
    {
        builder
            .HasOne(l => l.Utilizador)
            .WithMany(u => u.ListasPessoais)
            .HasForeignKey(l => l.UtilizadorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ListaPessoalItemConfiguration : IEntityTypeConfiguration<ListaPessoalItem>
{
    public void Configure(EntityTypeBuilder<ListaPessoalItem> builder)
    {
        builder.HasKey(i => new { i.ListaPessoalId, i.FilmeId });

        builder
            .HasOne(i => i.ListaPessoal)
            .WithMany(l => l.Items)
            .HasForeignKey(i => i.ListaPessoalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(i => i.Filme)
            .WithMany(f => f.ListaPessoalItems)
            .HasForeignKey(i => i.FilmeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ComunidadeConfiguration : IEntityTypeConfiguration<Comunidade>
{
    public void Configure(EntityTypeBuilder<Comunidade> builder)
    {
        builder.HasIndex(c => c.PublicId).IsUnique();
        builder.HasIndex(c => c.CodigoConvite).IsUnique();

        builder
            .HasOne(c => c.CreatedByUser)
            .WithMany()
            .HasForeignKey(c => c.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class ComunidadeMembroConfiguration : IEntityTypeConfiguration<ComunidadeMembro>
{
    public void Configure(EntityTypeBuilder<ComunidadeMembro> builder)
    {
        builder.HasKey(cm => new { cm.ComunidadeId, cm.UtilizadorId });

        builder
            .HasOne(cm => cm.Comunidade)
            .WithMany(c => c.Members)
            .HasForeignKey(cm => cm.ComunidadeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(cm => cm.Utilizador)
            .WithMany(u => u.Comunidades)
            .HasForeignKey(cm => cm.UtilizadorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ReporteUtilizadorConfiguration : IEntityTypeConfiguration<ReporteUtilizador>
{
    public void Configure(EntityTypeBuilder<ReporteUtilizador> builder)
    {
        builder.HasIndex(r => new { r.UtilizadorReportadoId, r.ReportadoPorUtilizadorId })
            .IsUnique();

        builder
            .HasOne(r => r.UtilizadorReportado)
            .WithMany()
            .HasForeignKey(r => r.UtilizadorReportadoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(r => r.ReportadoPorUtilizador)
            .WithMany()
            .HasForeignKey(r => r.ReportadoPorUtilizadorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class RewardTransacaoConfiguration : IEntityTypeConfiguration<RewardTransacao>
{
    public void Configure(EntityTypeBuilder<RewardTransacao> builder)
    {
        builder.Property(t => t.Motivo).HasMaxLength(250);
        builder.Property(t => t.ChaveAcao).HasMaxLength(160);

        builder.HasIndex(t => new { t.UtilizadorId, t.ChaveAcao }).IsUnique();
    }
}
