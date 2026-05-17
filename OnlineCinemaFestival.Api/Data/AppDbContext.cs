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
    public DbSet<FilmeGenero> FilmeGeneros => Set<FilmeGenero>();
    public DbSet<Pessoa> Pessoas => Set<Pessoa>();
    public DbSet<FilmePessoa> FilmePessoas => Set<FilmePessoa>();

    public DbSet<Sessao> Sessoes => Set<Sessao>();

    public DbSet<Carrinho> Carrinhos => Set<Carrinho>();
    public DbSet<CarrinhoItem> ItensCarrinho => Set<CarrinhoItem>();

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
    public DbSet<Pagamento> Pagamentos => Set<Pagamento>();

    public DbSet<AcessoUtilizador> AcessosUtilizador => Set<AcessoUtilizador>();
    public DbSet<Visualizacao> Visualizacoes => Set<Visualizacao>();
    public DbSet<Comunidade> Comunidades => Set<Comunidade>();
    public DbSet<ComunidadeMembro> ComunidadeMembros => Set<ComunidadeMembro>();

    public DbSet<Reward> Rewards => Set<Reward>();
    public DbSet<RewardTransacao> RewardsTransacoes => Set<RewardTransacao>();
    public DbSet<PremioFestival> PremiosFestival => Set<PremioFestival>();
    public DbSet<VotoPremioFestival> VotosPremiosFestival => Set<VotoPremioFestival>();
    public DbSet<ResultadoPremioFestival> ResultadosPremiosFestival =>
        Set<ResultadoPremioFestival>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FestivalFilme>().HasKey(ff => new { ff.FestivalId, ff.FilmeId });

        modelBuilder.Entity<FestivalFilme>().Property(ff => ff.DataAdicao).IsRequired();

        modelBuilder.Entity<FestivalFilme>().Property(ff => ff.Secao).HasMaxLength(150);

        modelBuilder.Entity<FestivalFilme>().Property(ff => ff.Categoria).HasMaxLength(150);

        modelBuilder.Entity<PremioFestival>().Property(p => p.Nome).HasMaxLength(150).IsRequired();

        modelBuilder.Entity<PremioFestival>().Property(p => p.Descricao).HasMaxLength(1000);

        modelBuilder
            .Entity<PremioFestival>()
            .HasOne(p => p.Festival)
            .WithMany(f => f.PremiosFestival)
            .HasForeignKey(p => p.FestivalId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<VotoPremioFestival>()
            .HasIndex(v => new { v.PremioFestivalId, v.UtilizadorId })
            .IsUnique();

        modelBuilder
            .Entity<VotoPremioFestival>()
            .HasOne(v => v.PremioFestival)
            .WithMany(p => p.Votos)
            .HasForeignKey(v => v.PremioFestivalId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<VotoPremioFestival>()
            .HasOne(v => v.Festival)
            .WithMany(f => f.VotosPremiosFestival)
            .HasForeignKey(v => v.FestivalId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<VotoPremioFestival>()
            .HasOne(v => v.Filme)
            .WithMany(f => f.VotosPremiosFestival)
            .HasForeignKey(v => v.FilmeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<VotoPremioFestival>()
            .HasOne(v => v.Utilizador)
            .WithMany(u => u.VotosPremiosFestival)
            .HasForeignKey(v => v.UtilizadorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<ResultadoPremioFestival>()
            .HasIndex(r => r.PremioFestivalId)
            .IsUnique();

        modelBuilder
            .Entity<ResultadoPremioFestival>()
            .HasOne(r => r.PremioFestival)
            .WithOne(p => p.Resultado)
            .HasForeignKey<ResultadoPremioFestival>(r => r.PremioFestivalId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<ResultadoPremioFestival>()
            .HasOne(r => r.FilmeVencedor)
            .WithMany(f => f.ResultadosPremiosFestival)
            .HasForeignKey(r => r.FilmeIdVencedor)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<ResultadoPremioFestival>()
            .HasOne(r => r.PublicadoPorUtilizador)
            .WithMany(u => u.ResultadosPremiosPublicados)
            .HasForeignKey(r => r.PublicadoPorUtilizadorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<FestivalFilme>()
            .HasOne(ff => ff.Festival)
            .WithMany(f => f.FestivalFilmes)
            .HasForeignKey(ff => ff.FestivalId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<FestivalFilme>()
            .HasOne(ff => ff.Filme)
            .WithMany(f => f.FestivalFilmes)
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

        modelBuilder.Entity<FilmeGenero>().HasKey(fg => new { fg.FilmeId, fg.GeneroId });

        modelBuilder
            .Entity<FilmeGenero>()
            .HasOne(fg => fg.Filme)
            .WithMany(f => f.FilmeGeneros)
            .HasForeignKey(fg => fg.FilmeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<FilmeGenero>()
            .HasOne(fg => fg.Genero)
            .WithMany(g => g.Filmes)
            .HasForeignKey(fg => fg.GeneroId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Filme>().HasIndex(f => f.TmdbId);

        modelBuilder.Entity<Pessoa>().HasIndex(p => p.TmdbPessoaId);

        modelBuilder.Entity<Pessoa>().HasIndex(p => p.Nome);

        modelBuilder.Entity<FilmePessoa>().HasKey(fp => new
        {
            fp.FilmeId,
            fp.PessoaId,
            fp.Funcao,
        });

        modelBuilder
            .Entity<FilmePessoa>()
            .HasOne(fp => fp.Filme)
            .WithMany(f => f.PessoasDoFilme)
            .HasForeignKey(fp => fp.FilmeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<FilmePessoa>()
            .HasOne(fp => fp.Pessoa)
            .WithMany(p => p.Filmes)
            .HasForeignKey(fp => fp.PessoaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<Avaliacao>()
            .HasOne(a => a.Usuario)
            .WithMany(u => u.Avaliacoes)
            .HasForeignKey(a => a.UsuarioId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<Avaliacao>()
            .HasOne(a => a.Filme)
            .WithMany(f => f.Avaliacoes)
            .HasForeignKey(a => a.FilmeId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Avaliacao>().HasIndex(a => new { a.UsuarioId, a.FilmeId }).IsUnique();

        modelBuilder
            .Entity<Comentario>()
            .ToTable(t =>
                t.HasCheckConstraint(
                    "CK_Comentarios_Alvo",
                    "ComunidadeId IS NOT NULL OR FilmeId IS NOT NULL"
                )
            );

        modelBuilder
            .Entity<Comentario>()
            .HasOne(c => c.Usuario)
            .WithMany(u => u.Comentarios)
            .HasForeignKey(c => c.UsuarioId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<Comentario>()
            .HasOne(c => c.Comunidade)
            .WithMany(c => c.Comentarios)
            .HasForeignKey(c => c.ComunidadeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<Comentario>()
            .HasOne(c => c.Filme)
            .WithMany(f => f.Comentarios)
            .HasForeignKey(c => c.FilmeId)
            .OnDelete(DeleteBehavior.Restrict);

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
            .WithMany(f => f.ListaPessoalItems)
            .HasForeignKey(i => i.FilmeId)
            .OnDelete(DeleteBehavior.Restrict);

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
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<Sessao>()
            .HasOne(s => s.Festival)
            .WithMany(f => f.Sessoes)
            .HasForeignKey(s => s.FestivalId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SessaoFilme>().HasKey(sf => new { sf.SessaoId, sf.FilmeId });

        modelBuilder.Entity<SessaoFilme>().Property(sf => sf.Ordem).IsRequired();

        modelBuilder.Entity<SessaoFilme>().Property(sf => sf.InicioOffsetSegundos).IsRequired();

        modelBuilder.Entity<SessaoFilme>().Property(sf => sf.IntervaloAposSegundos).IsRequired();

        modelBuilder.Entity<SessaoFilme>().HasIndex(sf => new { sf.SessaoId, sf.Ordem }).IsUnique();

        modelBuilder
            .Entity<SessaoFilme>()
            .HasOne(sf => sf.Sessao)
            .WithMany(s => s.FilmesDaSessao)
            .HasForeignKey(sf => sf.SessaoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<SessaoFilme>()
            .HasOne(sf => sf.Filme)
            .WithMany(f => f.SessoesDoFilme)
            .HasForeignKey(sf => sf.FilmeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<ItemCompra>()
            .HasOne(ci => ci.Compra)
            .WithMany(c => c.Itens)
            .HasForeignKey(ci => ci.CompraId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<Acesso>()
            .HasOne(a => a.Sessao)
            .WithMany(s => s.Acessos)
            .HasForeignKey(a => a.SessaoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<Acesso>()
            .HasOne(a => a.Festival)
            .WithMany(f => f.Acessos)
            .HasForeignKey(a => a.FestivalId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<Acesso>()
            .HasOne(a => a.Filme)
            .WithMany(f => f.Acessos)
            .HasForeignKey(a => a.FilmeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<Acesso>()
            .Property(a => a.Preco)
            .HasConversion<double>()
            .HasColumnType("REAL");

        modelBuilder
            .Entity<Carrinho>()
            .HasOne(c => c.Utilizador)
            .WithOne(u => u.Carrinho)
            .HasForeignKey<Carrinho>(c => c.UtilizadorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Carrinho>().HasIndex(c => c.UtilizadorId).IsUnique();

        modelBuilder.Entity<CarrinhoItem>().ToTable("ItensCarrinho");

        modelBuilder
            .Entity<CarrinhoItem>()
            .HasOne(i => i.Carrinho)
            .WithMany(c => c.Itens)
            .HasForeignKey(i => i.CarrinhoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<CarrinhoItem>()
            .HasOne(i => i.Acesso)
            .WithMany(a => a.CarrinhoItems)
            .HasForeignKey(i => i.AcessoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<CarrinhoItem>()
            .HasIndex(i => new { i.CarrinhoId, i.AcessoId })
            .IsUnique();

        modelBuilder
            .Entity<CarrinhoItem>()
            .Property(i => i.PrecoUnitario)
            .HasConversion<double>()
            .HasColumnType("REAL");

        modelBuilder
            .Entity<Compra>()
            .HasOne(c => c.Utilizador)
            .WithMany(u => u.Compras)
            .HasForeignKey(c => c.UtilizadorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Compra>().HasIndex(c => c.Referencia).IsUnique();

        modelBuilder
            .Entity<Compra>()
            .Property(c => c.ValorTotal)
            .HasConversion<double>()
            .HasColumnType("REAL");

        modelBuilder
            .Entity<Pagamento>()
            .HasOne(p => p.Compra)
            .WithOne(c => c.Pagamento)
            .HasForeignKey<Pagamento>(p => p.CompraId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Pagamento>().HasIndex(p => p.CompraId).IsUnique();

        modelBuilder.Entity<Pagamento>().HasIndex(p => p.Referencia).IsUnique();

        modelBuilder
            .Entity<Pagamento>()
            .Property(p => p.Valor)
            .HasConversion<double>()
            .HasColumnType("REAL");

        modelBuilder
            .Entity<ItemCompra>()
            .HasOne(i => i.Compra)
            .WithMany(c => c.Itens)
            .HasForeignKey(i => i.CompraId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<ItemCompra>()
            .HasOne(i => i.Acesso)
            .WithMany(a => a.ItensCompra)
            .HasForeignKey(i => i.AcessoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<ItemCompra>()
            .Property(i => i.PrecoUnitario)
            .HasConversion<double>()
            .HasColumnType("REAL");

        modelBuilder
            .Entity<ItemCompra>()
            .Property(i => i.Subtotal)
            .HasConversion<double>()
            .HasColumnType("REAL");

        modelBuilder
            .Entity<AcessoUtilizador>()
            .HasOne(a => a.Utilizador)
            .WithMany(u => u.AcessosUtilizador)
            .HasForeignKey(a => a.UtilizadorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<AcessoUtilizador>()
            .HasOne(a => a.Acesso)
            .WithMany(a => a.AcessosUtilizador)
            .HasForeignKey(a => a.AcessoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<AcessoUtilizador>()
            .HasOne(a => a.Compra)
            .WithMany(c => c.AcessosUtilizador)
            .HasForeignKey(a => a.CompraId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<AcessoUtilizador>()
            .HasOne(a => a.Sessao)
            .WithMany(s => s.AcessosUtilizador)
            .HasForeignKey(a => a.SessaoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<AcessoUtilizador>()
            .HasOne(a => a.Festival)
            .WithMany(f => f.AcessosUtilizador)
            .HasForeignKey(a => a.FestivalId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<AcessoUtilizador>()
            .HasOne(a => a.Filme)
            .WithMany(f => f.AcessosUtilizador)
            .HasForeignKey(a => a.FilmeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AcessoUtilizador>().HasIndex(a => new { a.UtilizadorId, a.AcessoId });

        modelBuilder
            .Entity<Visualizacao>()
            .HasOne(v => v.Utilizador)
            .WithMany(u => u.Visualizacoes)
            .HasForeignKey(v => v.UtilizadorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<Visualizacao>()
            .HasOne(v => v.Filme)
            .WithMany(f => f.Visualizacoes)
            .HasForeignKey(v => v.FilmeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<Visualizacao>()
            .HasOne(v => v.Sessao)
            .WithMany(s => s.Visualizacoes)
            .HasForeignKey(v => v.SessaoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<Visualizacao>()
            .HasOne(v => v.Festival)
            .WithMany(f => f.Visualizacoes)
            .HasForeignKey(v => v.FestivalId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Visualizacao>().HasIndex(v => new { v.UtilizadorId, v.VisualizadoEm });
        modelBuilder.Entity<Comunidade>().HasIndex(c => c.PublicId).IsUnique();
        modelBuilder.Entity<Comunidade>().HasIndex(c => c.CodigoConvite).IsUnique();
    }
}
