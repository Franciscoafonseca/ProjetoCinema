using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Festival> Festivals => Set<Festival>();
    public DbSet<Filme> Filmes => Set<Filme>();
    public DbSet<Avaliacao> Avaliacoes => Set<Avaliacao>();
    public DbSet<Comentario> Comentarios => Set<Comentario>();

    public DbSet<Utilizador> Utilizadores => Set<Utilizador>();
    public DbSet<PerfilUtilizador> PerfisUtilizador => Set<PerfilUtilizador>();
    public DbSet<Genero> Generos => Set<Genero>();
    public DbSet<UtilizadorGeneroFavorito> UtilizadoresGenerosFavoritos =>
        Set<UtilizadorGeneroFavorito>();

    public DbSet<ListaPessoal> ListasPessoais => Set<ListaPessoal>();
    public DbSet<ListaPessoalItem> ListaPessoalItems => Set<ListaPessoalItem>();

    public DbSet<Comunidade> Comunidades => Set<Comunidade>();
    public DbSet<ComunidadeMembro> ComunidadeMembros => Set<ComunidadeMembro>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Utilizador>().HasIndex(u => u.Email).IsUnique();

        modelBuilder
            .Entity<Utilizador>()
            .HasOne(u => u.Perfil)
            .WithOne(p => p.Utilizador)
            .HasForeignKey<PerfilUtilizador>(p => p.UtilizadorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<UtilizadorGeneroFavorito>()
            .HasKey(ug => new { ug.UtilizadorId, ug.GeneroId });

        modelBuilder
            .Entity<UtilizadorGeneroFavorito>()
            .HasOne(ug => ug.Utilizador)
            .WithMany(u => u.GenerosFavoritos)
            .HasForeignKey(ug => ug.UtilizadorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<UtilizadorGeneroFavorito>()
            .HasOne(ug => ug.Genero)
            .WithMany(g => g.UtilizadoresFavoritos)
            .HasForeignKey(ug => ug.GeneroId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Genero>().HasIndex(g => g.Name).IsUnique();

        modelBuilder
            .Entity<Avaliacao>()
            .HasOne(a => a.Usuario)
            .WithMany(u => u.Avaliacoes)
            .HasForeignKey(a => a.UsuarioId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<Avaliacao>()
            .HasOne(a => a.Filme)
            .WithMany(f => f.Avaliacoes)
            .HasForeignKey(a => a.FilmeId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Avaliacao>().HasIndex(a => new { a.UsuarioId, a.FilmeId }).IsUnique();

        modelBuilder
            .Entity<Comentario>()
            .HasOne(c => c.Usuario)
            .WithMany(u => u.Comentarios)
            .HasForeignKey(c => c.UsuarioId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<Comentario>()
            .HasOne(c => c.Comunidade)
            .WithMany(c => c.Comentarios)
            .HasForeignKey(c => c.ComunidadeId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<ListaPessoal>()
            .HasOne(l => l.Utilizador)
            .WithMany(u => u.ListasPessoais)
            .HasForeignKey(l => l.UtilizadorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ListaPessoalItem>().HasKey(i => new { i.ListaPessoalId, i.FilmeId });

        modelBuilder
            .Entity<ListaPessoalItem>()
            .HasOne(i => i.ListaPessoal)
            .WithMany(l => l.Items)
            .HasForeignKey(i => i.ListaPessoalId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<ListaPessoalItem>()
            .HasOne(i => i.Filme)
            .WithMany()
            .HasForeignKey(i => i.FilmeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<Comunidade>()
            .HasOne(c => c.CreatedByUser)
            .WithMany()
            .HasForeignKey(c => c.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder
            .Entity<ComunidadeMembro>()
            .HasKey(cm => new { cm.ComunidadeId, cm.UtilizadorId });

        modelBuilder
            .Entity<ComunidadeMembro>()
            .HasOne(cm => cm.Comunidade)
            .WithMany(c => c.Members)
            .HasForeignKey(cm => cm.ComunidadeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<ComunidadeMembro>()
            .HasOne(cm => cm.Utilizador)
            .WithMany(u => u.Comunidades)
            .HasForeignKey(cm => cm.UtilizadorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
