namespace OnlineCinemaFestival.Api.Data.Seed;

public static partial class DbSeeder
{
    private sealed class UtilizadoresSeedStep : ISeedStep
    {
        public async Task ExecutarAsync(SeedContext contexto)
        {
            contexto.Admin = await CriarAdminAsync(
                contexto.Db,
                contexto.PasswordHashingStrategy,
                contexto.Configuration
            );

            var utilizadores = await CriarUtilizadoresAsync(
                contexto.Db,
                contexto.PasswordHashingStrategy,
                contexto.Configuration
            );

            contexto.Utilizadores.Clear();
            contexto.Utilizadores.AddRange(utilizadores);
        }
    }
}
