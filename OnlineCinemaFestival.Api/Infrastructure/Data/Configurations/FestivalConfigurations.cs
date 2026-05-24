using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Infrastructure.Data.Configurations;

public class FestivalFilmeConfiguration : IEntityTypeConfiguration<FestivalFilme>
{
    public void Configure(EntityTypeBuilder<FestivalFilme> builder)
    {
        builder.HasKey(ff => new { ff.FestivalId, ff.FilmeId });
        builder.Property(ff => ff.DataAdicao).IsRequired();
        builder.Property(ff => ff.Secao).HasMaxLength(150);
        builder.Property(ff => ff.Categoria).HasMaxLength(150);

        builder
            .HasOne(ff => ff.Festival)
            .WithMany(f => f.FestivalFilmes)
            .HasForeignKey(ff => ff.FestivalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(ff => ff.Filme)
            .WithMany(f => f.FestivalFilmes)
            .HasForeignKey(ff => ff.FilmeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class PremioFestivalConfiguration : IEntityTypeConfiguration<PremioFestival>
{
    public void Configure(EntityTypeBuilder<PremioFestival> builder)
    {
        builder.Property(p => p.Nome).HasMaxLength(150).IsRequired();
        builder.Property(p => p.Descricao).HasMaxLength(1000);

        builder
            .HasOne(p => p.Festival)
            .WithMany(f => f.PremiosFestival)
            .HasForeignKey(p => p.FestivalId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class VotoPremioFestivalConfiguration : IEntityTypeConfiguration<VotoPremioFestival>
{
    public void Configure(EntityTypeBuilder<VotoPremioFestival> builder)
    {
        builder.HasIndex(v => new { v.PremioFestivalId, v.UtilizadorId }).IsUnique();

        builder
            .HasOne(v => v.PremioFestival)
            .WithMany(p => p.Votos)
            .HasForeignKey(v => v.PremioFestivalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(v => v.Festival)
            .WithMany(f => f.VotosPremiosFestival)
            .HasForeignKey(v => v.FestivalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(v => v.Filme)
            .WithMany(f => f.VotosPremiosFestival)
            .HasForeignKey(v => v.FilmeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(v => v.Utilizador)
            .WithMany(u => u.VotosPremiosFestival)
            .HasForeignKey(v => v.UtilizadorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ResultadoPremioFestivalConfiguration
    : IEntityTypeConfiguration<ResultadoPremioFestival>
{
    public void Configure(EntityTypeBuilder<ResultadoPremioFestival> builder)
    {
        builder.HasIndex(r => r.PremioFestivalId).IsUnique();

        builder
            .HasOne(r => r.PremioFestival)
            .WithOne(p => p.Resultado)
            .HasForeignKey<ResultadoPremioFestival>(r => r.PremioFestivalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(r => r.FilmeVencedor)
            .WithMany(f => f.ResultadosPremiosFestival)
            .HasForeignKey(r => r.FilmeIdVencedor)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(r => r.PublicadoPorUtilizador)
            .WithMany(u => u.ResultadosPremiosPublicados)
            .HasForeignKey(r => r.PublicadoPorUtilizadorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
