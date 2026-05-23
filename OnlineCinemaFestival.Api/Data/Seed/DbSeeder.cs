using Bogus;
using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Data.Seed;

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
            new UtilizadoresSeedStep(),
            new CatalogoSeedStep(),
            new FestivaisSeedStep(),
            new SessoesSeedStep(),
            new SocialSeedStep(),
            new ComprasSeedStep(),
            new RecomendacoesSeedStep(),
        ];

        foreach (var passo in passos)
            await passo.ExecutarAsync(contexto);

        await db.SaveChangesAsync();
    }
}
