using Microsoft.EntityFrameworkCore;

namespace OnlineCinemaFestival.Api.Data.Seed;

public static partial class DbSeeder
{
    private sealed class SessoesSeedStep : ISeedStep
    {
        public async Task ExecutarAsync(SeedContext contexto)
        {
            contexto.Sessoes.Clear();
            contexto.Sessoes.AddRange(
                await CriarSessoesAsync(contexto.Db, contexto.Festivais, contexto.Filmes)
            );

            contexto.SessaoChatTeste = await CriarSessaoChatAoVivoTesteAsync(
                contexto.Db,
                contexto.Festivais,
                contexto.Filmes
            );

            contexto.Sessoes.Clear();
            contexto.Sessoes.AddRange(await contexto.Db.Sessoes.Include(s => s.Filme).ToListAsync());

            contexto.Acessos.Clear();
            contexto.Acessos.AddRange(
                await CriarAcessosAsync(
                    contexto.Db,
                    contexto.Festivais,
                    contexto.Filmes,
                    contexto.Sessoes
                )
            );
        }
    }
}
