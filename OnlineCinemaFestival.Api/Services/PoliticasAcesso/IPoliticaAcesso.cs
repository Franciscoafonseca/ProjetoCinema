using OnlineCinemaFestival.Api.Models;
using ModelAcesso = OnlineCinemaFestival.Api.Models.Acesso;

namespace OnlineCinemaFestival.Api.Services.PoliticasAcesso;

public interface IPoliticaAcesso
{
    TipoAcesso TipoSuportado { get; }

    bool TemAcesso(ModelAcesso acesso, DateTime agora);
}
