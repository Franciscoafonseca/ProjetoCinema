using OnlineCinemaFestival.Api.Models;
using ModelAcesso = OnlineCinemaFestival.Api.Models.Acesso;

namespace OnlineCinemaFestival.Api.Services.PoliticasAcesso;

public sealed class PoliticaBilheteSessao : IPoliticaAcesso
{
    public TipoAcesso TipoSuportado => TipoAcesso.BilheteSessao;

    public bool TemAcesso(ModelAcesso acesso, DateTime agora)
    {
        if (acesso.Tipo != TipoSuportado || acesso.Sessao is null)
            return false;

        return acesso.Sessao.Inicio <= agora && agora <= acesso.Sessao.Fim;
    }
}

