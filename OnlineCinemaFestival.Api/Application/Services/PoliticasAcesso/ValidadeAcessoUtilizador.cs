using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Services.PoliticasAcesso;

internal static class ValidadeAcessoUtilizador
{
    public static bool EstaAtivo(AcessoUtilizador acesso, DateTime agora)
    {
        return acesso.Ativo
            && acesso.InicioValidade <= agora
            && acesso.FimValidade >= agora;
    }
}
