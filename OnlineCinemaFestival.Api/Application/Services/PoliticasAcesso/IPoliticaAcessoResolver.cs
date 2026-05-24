using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Services.PoliticasAcesso;

public interface IPoliticaAcessoResolver
{
    IPoliticaAcesso Resolver(TipoAcesso tipoAcesso);
}

