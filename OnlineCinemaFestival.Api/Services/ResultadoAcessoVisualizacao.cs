using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public sealed record ResultadoAcessoVisualizacao(
    bool Permitido,
    AcessoUtilizador? Acesso,
    string Mensagem
)
{
    public static ResultadoAcessoVisualizacao Autorizado(AcessoUtilizador acesso)
    {
        return new ResultadoAcessoVisualizacao(true, acesso, "Acesso autorizado.");
    }

    public static ResultadoAcessoVisualizacao Negado(string mensagem)
    {
        return new ResultadoAcessoVisualizacao(false, null, mensagem);
    }
}
