namespace OnlineCinemaFestival.Api.Data.Seed;

public static partial class DbSeeder
{
    private sealed class ComprasSeedStep : ISeedStep
    {
        public async Task ExecutarAsync(SeedContext contexto)
        {
            await CriarCarrinhosAsync(contexto.Db, contexto.Utilizadores, contexto.Acessos);
            await CriarComprasEAcessosUtilizadorAsync(
                contexto.Db,
                contexto.Utilizadores,
                contexto.Acessos
            );

            if (contexto.SessaoChatTeste != null)
            {
                await GarantirAcessoSessaoChatTesteAsync(
                    contexto.Db,
                    contexto.Utilizadores,
                    contexto.SessaoChatTeste
                );
            }
        }
    }
}
