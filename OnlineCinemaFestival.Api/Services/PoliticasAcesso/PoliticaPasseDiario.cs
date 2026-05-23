using OnlineCinemaFestival.Api.Models;
using ModelAcesso = OnlineCinemaFestival.Api.Models.Acesso;

namespace OnlineCinemaFestival.Api.Services.PoliticasAcesso;

public sealed class PoliticaPasseDiario : IPoliticaAcesso
{
    public TipoAcesso TipoSuportado => TipoAcesso.PasseDiario;

    public bool TemAcesso(ModelAcesso acesso, DateTime agora)
    {
        if (acesso.Tipo != TipoSuportado || !acesso.DataAcesso.HasValue)
            return false;

        return acesso.DataAcesso.Value.Date == agora.Date;
    }
}

