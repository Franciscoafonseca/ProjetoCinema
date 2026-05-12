using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Acesso> Acessos => Set<Acesso>();

    public DbSet<SessaoFilme> SessaoFilmes => Set<SessaoFilme>();
    public DbSet<Festival> Festivals => Set<Festival>();
    public DbSet<Filme> Filmes => Set<Filme>();

    public DbSet<Sessao> Sessoes => Set<Sessao>();

    public DbSet<Carrinho> Carrinhos => Set<Carrinho>();

    public DbSet<ItemCarrinho> ItensCarrinho => Set<ItemCarrinho>();
    public DbSet<FestivalFilme> FestivalFilmes => Set<FestivalFilme>();
    public DbSet<Avaliacao> Avaliacoes => Set<Avaliacao>();
    public DbSet<Comentario> Comentarios => Set<Comentario>();

    public DbSet<Utilizador> Utilizadores => Set<Utilizador>();
    public DbSet<PerfilUtilizador> PerfisUtilizador => Set<PerfilUtilizador>();
    public DbSet<Genero> Generos => Set<Genero>();
    public DbSet<UtilizadorGeneroFavorito> UtilizadoresGenerosFavoritos =>
        Set<UtilizadorGeneroFavorito>();

    public DbSet<ListaPessoal> ListasPessoais => Set<ListaPessoal>();
    public DbSet<ListaPessoalItem> ListaPessoalItems => Set<ListaPessoalItem>();
    public DbSet<Compra> Compras => Set<Compra>();

    public DbSet<ItemCompra> ItensCompra => Set<ItemCompra>();

    public DbSet<AcessoUtilizador> AcessosUtilizador => Set<AcessoUtilizador>();
    public DbSet<Comunidade> Comunidades => Set<Comunidade>();
    public DbSet<ComunidadeMembro> ComunidadeMembros => Set<ComunidadeMembro>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FestivalFilme>().HasKey(ff => new { ff.FestivalId, ff.FilmeId });

        modelBuilder
            .Entity<FestivalFilme>()
            .HasOne(ff => ff.Festival)
            .WithMany()
            .HasForeignKey(ff => ff.FestivalId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<FestivalFilme>()
            .HasOne(ff => ff.Filme)
            .WithMany()
            .HasForeignKey(ff => ff.FilmeId)
            .OnDelete(DeleteBehavior.Cascade);

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

        modelBuilder
            .Entity<Sessao>()
            .HasOne(s => s.Festival)
            .WithMany()
            .HasForeignKey(s => s.FestivalId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SessaoFilme>().HasKey(sf => new { sf.SessaoId, sf.FilmeId });

        modelBuilder
            .Entity<SessaoFilme>()
            .HasOne(sf => sf.Sessao)
            .WithMany(s => s.FilmesDaSessao)
            .HasForeignKey(sf => sf.SessaoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<SessaoFilme>()
            .HasOne(sf => sf.Filme)
            .WithMany()
            .HasForeignKey(sf => sf.FilmeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<Acesso>()
            .HasOne(a => a.Sessao)
            .WithMany()
            .HasForeignKey(a => a.SessaoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<Acesso>()
            .HasOne(a => a.Festival)
            .WithMany()
            .HasForeignKey(a => a.FestivalId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<Acesso>()
            .HasOne(a => a.Filme)
            .WithMany()
            .HasForeignKey(a => a.FilmeId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder
            .Entity<Carrinho>()
            .HasOne(c => c.Utilizador)
            .WithMany()
            .HasForeignKey(c => c.UtilizadorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Carrinho>().HasIndex(c => c.UtilizadorId).IsUnique();

        modelBuilder
            .Entity<ItemCarrinho>()
            .HasOne(i => i.Carrinho)
            .WithMany(c => c.Itens)
            .HasForeignKey(i => i.CarrinhoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<ItemCarrinho>()
            .HasOne(i => i.Acesso)
            .WithMany()
            .HasForeignKey(i => i.AcessoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<ItemCarrinho>()
            .HasIndex(i => new { i.CarrinhoId, i.AcessoId })
            .IsUnique();

        modelBuilder.Entity<ItemCarrinho>().Property(i => i.PrecoUnitario).HasPrecision(10, 2);
        modelBuilder
            .Entity<Compra>()
            .HasOne(c => c.Utilizador)
            .WithMany()
            .HasForeignKey(c => c.UtilizadorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Compra>().HasIndex(c => c.Referencia).IsUnique();

        modelBuilder.Entity<Compra>().Property(c => c.ValorTotal).HasPrecision(10, 2);

        modelBuilder
            .Entity<ItemCompra>()
            .HasOne(i => i.Compra)
            .WithMany(c => c.Itens)
            .HasForeignKey(i => i.CompraId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<ItemCompra>()
            .HasOne(i => i.Acesso)
            .WithMany()
            .HasForeignKey(i => i.AcessoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ItemCompra>().Property(i => i.PrecoUnitario).HasPrecision(10, 2);

        modelBuilder.Entity<ItemCompra>().Property(i => i.Subtotal).HasPrecision(10, 2);

        modelBuilder
            .Entity<AcessoUtilizador>()
            .HasOne(a => a.Utilizador)
            .WithMany()
            .HasForeignKey(a => a.UtilizadorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<AcessoUtilizador>()
            .HasOne(a => a.Acesso)
            .WithMany()
            .HasForeignKey(a => a.AcessoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<AcessoUtilizador>()
            .HasOne(a => a.Compra)
            .WithMany()
            .HasForeignKey(a => a.CompraId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<AcessoUtilizador>()
            .HasOne(a => a.Sessao)
            .WithMany()
            .HasForeignKey(a => a.SessaoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<AcessoUtilizador>()
            .HasOne(a => a.Festival)
            .WithMany()
            .HasForeignKey(a => a.FestivalId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<AcessoUtilizador>()
            .HasOne(a => a.Filme)
            .WithMany()
            .HasForeignKey(a => a.FilmeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AcessoUtilizador>().HasIndex(a => new { a.UtilizadorId, a.AcessoId });
    }
}
