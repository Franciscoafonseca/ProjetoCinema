namespace OnlineCinemaFestival.Api.Data.Seed;

public static partial class DbSeeder
{
    private sealed class RecomendacoesSeedStep : ISeedStep
    {
        public async Task ExecutarAsync(SeedContext contexto)
        {
            await CriarAvaliacoesAsync(contexto.Db, contexto.Utilizadores, contexto.Filmes);
            await CriarVisualizacoesAsync(contexto.Db, contexto.Utilizadores, contexto.Filmes);
        }
    }
}
