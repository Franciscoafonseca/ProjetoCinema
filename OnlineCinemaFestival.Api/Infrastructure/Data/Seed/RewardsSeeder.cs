namespace OnlineCinemaFestival.Api.Infrastructure.Data.Seed;

public static partial class DbSeeder
{
    /// <summary>
    /// Seeder reservado para rewards/pontuacao.
    /// Atualmente nao gera dados — os rewards sao atribuidos em runtime pelo <c>RewardsPontuacaoService</c>
    /// em resposta a acoes dos utilizadores (compras, comentarios, etc.).
    /// </summary>
    public sealed class RewardsSeeder : ISeedStep
    {
        public Task ExecutarAsync(SeedContext contexto) => Task.CompletedTask;
    }
}
