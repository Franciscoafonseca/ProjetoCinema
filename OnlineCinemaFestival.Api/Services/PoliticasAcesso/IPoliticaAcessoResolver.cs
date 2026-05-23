using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services.PoliticasAcesso;

public interface IPoliticaAcessoResolver
{
    IPoliticaAcesso Resolver(TipoAcesso tipoAcesso);
}

