namespace OnlineCinemaFestival.Api.Infrastructure.Data.Seed;

public static partial class DbSeeder
{
    /// <summary>
    /// Seeder reservado para premios de festivais.
    /// Atualmente nao gera dados — os premios sao criados manualmente pelo organizador
    /// e publicados pelo <c>PublicacaoPremiosService</c>.
    /// </summary>
    public sealed class PremiosSeeder : ISeedStep
    {
        public Task ExecutarAsync(SeedContext contexto) => Task.CompletedTask;
    }
}
