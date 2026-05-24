using Bogus;
using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Infrastructure.Data.Seed;

/// <summary>
/// Orquestrador de seed — delega cada domínio ao respetivo <see cref="ISeedStep"/>.
/// A ordem importa: cada passo pode depender de dados criados pelos anteriores.
/// </summary>
public static partial class DbSeeder
{
    public static async Task SeedAsync(
        AppDbContext db,
        IPasswordHashingStrategy passwordHashingStrategy,
        IConfiguration configuration
    )
    {
        await db.Database.MigrateAsync();

        Randomizer.Seed = new Random(2120622);

        var contexto = new SeedContext
        {
            Db = db,
            PasswordHashingStrategy = passwordHashingStrategy,
            Configuration = configuration,
        };

        ISeedStep[] passos =
        [
            new UtilizadoresSeeder(),   // 1. utilizadores + perfis + géneros favoritos
            new FilmesSeeder(),          // 2. géneros + filmes TMDB
            new FestivaisSeeder(),       // 3. festivais + associação filmes↔festivais
            new SessoesSeeder(),         // 4. sessões + sessão de chat ativo para testes
            new AcessosSeeder(),         // 5. bilhetes, passes e alugueres
            new ComprasSeeder(),         // 6. carrinhos + compras + acessos de utilizador
            new ComunidadesSeeder(),     // 7. comunidades + membros + comentários
            new ReviewsSeeder(),         // 8. avaliações + visualizações
            new ListasSeeder(),          // 9. listas pessoais (watchlist, favoritos, vistos)
            new PremiosSeeder(),         // 10. prémios (geridos manualmente em runtime)
            new RewardsSeeder(),         // 11. rewards (atribuídos em runtime)
        ];

        foreach (var passo in passos)
            await passo.ExecutarAsync(contexto);

        await db.SaveChangesAsync();
    }
}
