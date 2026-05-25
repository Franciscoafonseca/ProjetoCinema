using OnlineCinemaFestival.Api.Domain;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class PublicacaoPremiosService : IPublicacaoPremiosService
{
    private readonly IPremioFestivalRepository _repository;
    private readonly TimeProvider _timeProvider;

    public PublicacaoPremiosService(IPremioFestivalRepository repository, TimeProvider timeProvider)
    {
        _repository = repository;
        _timeProvider = timeProvider;
    }

    public async Task<int> PublicarResultadosPendentesAsync(
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var agora = _timeProvider.GetUtcNow().UtcDateTime;
        var premios = await _repository.ObterPremiosPendentesPublicacaoAsync(agora);
        var publicados = 0;

        foreach (var premio in premios)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var vencedor = await _repository.ObterVencedorPorVotosAsync(premio.Id);

            if (vencedor == null)
            {
                premio.EstadoPremio = EstadoPremio.Fechado;
                continue;
            }

            premio.Resultado = new ResultadoPremioFestival
            {
                PremioFestivalId = premio.Id,
                FilmeIdVencedor = vencedor.Value.FilmeId,
                TotalVotos = vencedor.Value.TotalVotos,
                PublicadoEm = agora,
                PublicadoPorUtilizadorId = null,
            };

            premio.EstadoPremio = EstadoPremio.Publicado;
            publicados++;
        }

        if (premios.Count > 0)
            await _repository.SaveChangesAsync();

        return publicados;
    }
}
