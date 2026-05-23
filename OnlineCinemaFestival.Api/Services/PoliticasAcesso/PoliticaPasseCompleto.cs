using OnlineCinemaFestival.Api.Models;
using ModelAcesso = OnlineCinemaFestival.Api.Models.Acesso;

namespace OnlineCinemaFestival.Api.Services.PoliticasAcesso;

public sealed class PoliticaPasseCompleto : IPoliticaAcesso
{
    public TipoAcesso TipoSuportado => TipoAcesso.PasseCompleto;

    public bool TemAcesso(ModelAcesso acesso, DateTime agora)
    {
        if (acesso.Tipo != TipoSuportado || acesso.Festival is null)
            return false;

        return acesso.Festival.StartDate <= agora && agora <= acesso.Festival.EndDate;
    }
}

