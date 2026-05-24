using OnlineCinemaFestival.Api.Infrastructure.Data;
using OnlineCinemaFestival.Api.Domain;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Infrastructure.Data.Seed;

public sealed class SeedContext
{
    public required AppDbContext Db { get; init; }
    public required IPasswordHashingStrategy PasswordHashingStrategy { get; init; }
    public required IConfiguration Configuration { get; init; }

    public Utilizador? Admin { get; set; }
    public List<Utilizador> Utilizadores { get; } = [];
    public List<Genero> Generos { get; } = [];
    public List<Festival> Festivais { get; } = [];
    public List<Filme> Filmes { get; } = [];
    public List<Sessao> Sessoes { get; } = [];
    public Sessao? SessaoChatTeste { get; set; }
    public List<Acesso> Acessos { get; } = [];
    public List<Comunidade> Comunidades { get; } = [];

    public List<Utilizador> TodosUtilizadores
    {
        get
        {
            var todos = Utilizadores.ToList();
            if (Admin != null)
                todos.Add(Admin);
            return todos;
        }
    }
}
