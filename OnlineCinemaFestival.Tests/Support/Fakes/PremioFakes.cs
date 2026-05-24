using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Tests.Support.Fakes;

public sealed class PremioFestivalRepositoryFalso : IPremioFestivalRepository
{
    private readonly PremioFestival _premio;

    public PremioFestivalRepositoryFalso(PremioFestival premio)
    {
        _premio = premio;
    }

    public bool ViuTodosFilmesElegiveis { get; init; } = true;

    public List<VotoPremioFestival> Votos { get; init; } = new();

    public Task<bool> FestivalExisteAsync(int festivalId) =>
        Task.FromResult(festivalId == _premio.FestivalId);

    public Task<PremioFestival?> ObterPremioAsync(int premioFestivalId) =>
        Task.FromResult(premioFestivalId == _premio.Id ? _premio : null);

    public Task<PremioFestival?> ObterPremioComResultadoAsync(int premioFestivalId) =>
        Task.FromResult(premioFestivalId == _premio.Id ? _premio : null);

    public Task AddPremioAsync(PremioFestival premio) => Task.CompletedTask;

    public Task<bool> FilmeElegivelAsync(int festivalId, int filmeId) =>
        Task.FromResult(filmeId is 10 or 11);

    public Task<bool> UtilizadorJaVotouAsync(int premioFestivalId, int utilizadorId) =>
        Task.FromResult(
            Votos.Any(v => v.PremioFestivalId == premioFestivalId && v.UtilizadorId == utilizadorId)
        );

    public Task<bool> UtilizadorViuTodosFilmesElegiveisAsync(int festivalId, int utilizadorId) =>
        Task.FromResult(ViuTodosFilmesElegiveis);

    public Task AddVotoAsync(VotoPremioFestival voto)
    {
        Votos.Add(voto);
        return Task.CompletedTask;
    }

    public Task<(int FilmeId, int TotalVotos)?> ObterVencedorPorVotosAsync(int premioFestivalId)
    {
        var vencedor = Votos
            .Where(v => v.PremioFestivalId == premioFestivalId)
            .GroupBy(v => v.FilmeId)
            .Select(g => (FilmeId: g.Key, TotalVotos: g.Count()))
            .OrderByDescending(v => v.TotalVotos)
            .ThenBy(v => v.FilmeId)
            .FirstOrDefault();

        return Task.FromResult<(int FilmeId, int TotalVotos)?>(
            vencedor == default ? null : vencedor
        );
    }

    public Task AddResultadoAsync(ResultadoPremioFestival resultado)
    {
        _premio.Resultado = resultado;
        return Task.CompletedTask;
    }

    public Task<ResultadoPremioFestival?> ObterResultadoCompletoAsync(int premioFestivalId) =>
        Task.FromResult(_premio.Resultado);

    public Task<List<ResultadoPremioFestival>> ObterResultadosPublicosAsync(
        int? festivalId,
        int? filmeId
    ) =>
        Task.FromResult(
            _premio.Resultado is null
                ? new List<ResultadoPremioFestival>()
                : new List<ResultadoPremioFestival> { _premio.Resultado }
        );

    public Task<List<PremioFestival>> ObterPremiosPorFestivalAsync(
        int festivalId,
        bool incluirRascunhos
    ) => Task.FromResult(new List<PremioFestival> { _premio });

    public Task<List<PremioFestival>> ObterPremiosPendentesPublicacaoAsync(DateTime dataAtual) =>
        Task.FromResult(
            _premio.Resultado == null && _premio.DataFechoVotacao <= dataAtual
                ? new List<PremioFestival> { _premio }
                : new List<PremioFestival>()
        );

    public Task SaveChangesAsync() => Task.CompletedTask;

    public Task<bool> TrySaveChangesAsync() => Task.FromResult(true);
}
