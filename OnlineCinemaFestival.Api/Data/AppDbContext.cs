using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Acesso> Acessos => Set<Acesso>();

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
    public DbSet<MensagemChatSessao> MensagensChatSessao => Set<MensagemChatSessao>();
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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
