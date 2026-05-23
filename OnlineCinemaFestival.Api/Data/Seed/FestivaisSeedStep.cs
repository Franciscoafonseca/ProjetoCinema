namespace OnlineCinemaFestival.Api.Data.Seed;

public static partial class DbSeeder
{
    private sealed class FestivaisSeedStep : ISeedStep
    {
        public async Task ExecutarAsync(SeedContext contexto)
        {
            contexto.Festivais.Clear();
            contexto.Festivais.AddRange(await CriarFestivaisAsync(contexto.Db));

            await AssociarFilmesAFestivaisAsync(
                contexto.Db,
                contexto.Festivais,
                contexto.Filmes
            );
        }
    }
}
