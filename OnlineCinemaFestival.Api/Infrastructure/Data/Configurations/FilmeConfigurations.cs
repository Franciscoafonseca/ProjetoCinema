using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Infrastructure.Data.Configurations;

public class FilmeConfiguration : IEntityTypeConfiguration<Filme>
{
    public void Configure(EntityTypeBuilder<Filme> builder)
    {
        builder.HasIndex(f => f.TmdbId);
    }
}

public class GeneroConfiguration : IEntityTypeConfiguration<Genero>
{
    public void Configure(EntityTypeBuilder<Genero> builder)
    {
        builder.HasIndex(g => g.Name).IsUnique();
    }
}

public class FilmeGeneroConfiguration : IEntityTypeConfiguration<FilmeGenero>
{
    public void Configure(EntityTypeBuilder<FilmeGenero> builder)
    {
        builder.HasKey(fg => new { fg.FilmeId, fg.GeneroId });

        builder
            .HasOne(fg => fg.Filme)
            .WithMany(f => f.FilmeGeneros)
            .HasForeignKey(fg => fg.FilmeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(fg => fg.Genero)
            .WithMany(g => g.Filmes)
            .HasForeignKey(fg => fg.GeneroId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class PessoaConfiguration : IEntityTypeConfiguration<Pessoa>
{
    public void Configure(EntityTypeBuilder<Pessoa> builder)
    {
        builder.HasIndex(p => p.TmdbPessoaId);
        builder.HasIndex(p => p.Nome);
    }
}

public class FilmePessoaConfiguration : IEntityTypeConfiguration<FilmePessoa>
{
    public void Configure(EntityTypeBuilder<FilmePessoa> builder)
    {
        builder.HasKey(fp => new { fp.FilmeId, fp.PessoaId, fp.Funcao });

        builder
            .HasOne(fp => fp.Filme)
            .WithMany(f => f.PessoasDoFilme)
            .HasForeignKey(fp => fp.FilmeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(fp => fp.Pessoa)
            .WithMany(p => p.Filmes)
            .HasForeignKey(fp => fp.PessoaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class AvaliacaoConfiguration : IEntityTypeConfiguration<Avaliacao>
{
    public void Configure(EntityTypeBuilder<Avaliacao> builder)
    {
        builder
            .HasOne(a => a.Usuario)
            .WithMany(u => u.Avaliacoes)
            .HasForeignKey(a => a.UsuarioId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(a => a.Filme)
            .WithMany(f => f.Avaliacoes)
            .HasForeignKey(a => a.FilmeId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => new { a.UsuarioId, a.FilmeId }).IsUnique();
    }
}
