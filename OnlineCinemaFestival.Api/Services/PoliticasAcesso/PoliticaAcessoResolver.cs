using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services.PoliticasAcesso;

public sealed class PoliticaAcessoResolver : IPoliticaAcessoResolver
{
    private readonly IReadOnlyDictionary<TipoAcesso, IPoliticaAcesso> _politicas;

    public PoliticaAcessoResolver(IEnumerable<IPoliticaAcesso> politicas)
    {
        _politicas = politicas.ToDictionary(politica => politica.TipoSuportado);
    }

    public IPoliticaAcesso Resolver(TipoAcesso tipoAcesso)
    {
        if (_politicas.TryGetValue(tipoAcesso, out var politica))
            return politica;

        throw new ArgumentException(
            $"Nao existe politica de acesso registada para o tipo {tipoAcesso}.",
            nameof(tipoAcesso)
        );
    }
}

