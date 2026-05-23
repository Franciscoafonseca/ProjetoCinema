namespace OnlineCinemaFestival.Api.Data.Seed;

public static partial class DbSeeder
{
    private sealed class SocialSeedStep : ISeedStep
    {
        public async Task ExecutarAsync(SeedContext contexto)
        {
            await CriarPerfisAsync(contexto.Db, contexto.TodosUtilizadores);
            await CriarGenerosFavoritosAsync(
                contexto.Db,
                contexto.Utilizadores,
                contexto.Generos
            );

            contexto.Comunidades.Clear();
            contexto.Comunidades.AddRange(
                await CriarComunidadesAsync(contexto.Db, contexto.Utilizadores)
            );

            await CriarMembrosComunidadesAsync(
                contexto.Db,
                contexto.Comunidades,
                contexto.Utilizadores
            );

            await CriarComentariosAsync(
                contexto.Db,
                contexto.Comunidades,
                contexto.Utilizadores
            );

            await CriarListasPessoaisAsync(contexto.Db, contexto.Utilizadores, contexto.Filmes);
        }
    }
}
