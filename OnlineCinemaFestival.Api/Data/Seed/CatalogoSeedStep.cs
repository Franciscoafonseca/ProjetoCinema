namespace OnlineCinemaFestival.Api.Data.Seed;

public static partial class DbSeeder
{
    private sealed class CatalogoSeedStep : ISeedStep
    {
        public async Task ExecutarAsync(SeedContext contexto)
        {
            contexto.Generos.Clear();
            contexto.Generos.AddRange(await CriarGenerosAsync(contexto.Db));

            contexto.Filmes.Clear();
            contexto.Filmes.AddRange(await CriarFilmesAsync(contexto.Db, contexto.Generos));
        }
    }
}
