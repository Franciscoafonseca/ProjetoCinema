using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Mapping;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class AcessoUtilizadorService : IAcessoUtilizadorService
{
    private readonly IAcessoUtilizadorRepository _acessoUtilizadorRepository;
    private readonly IAcessoVisualizacaoService _acessoVisualizacaoService;
    private readonly IVisualizacaoRepository _visualizacaoRepository;
    private readonly TimeProvider _timeProvider;

    public AcessoUtilizadorService(
        IAcessoUtilizadorRepository acessoUtilizadorRepository,
        IAcessoVisualizacaoService acessoVisualizacaoService,
        IVisualizacaoRepository visualizacaoRepository,
        TimeProvider timeProvider
    )
    {
        _acessoUtilizadorRepository = acessoUtilizadorRepository;
        _acessoVisualizacaoService = acessoVisualizacaoService;
        _visualizacaoRepository = visualizacaoRepository;
        _timeProvider = timeProvider;
    }

    public async Task<IEnumerable<AcessoUtilizadorReadDTO>> ObterAcessosDoUtilizadorAsync(
        int utilizadorId
    )
    {
        var acessos = await _acessoUtilizadorRepository.ObterPorUtilizadorIdAsync(utilizadorId);
        var agora = _timeProvider.GetUtcNow().UtcDateTime;

        return acessos.Select(acesso => AcessoUtilizadorMapper.MapToReadDTO(acesso, agora));
    }

    public async Task<IEnumerable<AcessoUtilizadorReadDTO>> ObterAcessosAtivosDoUtilizadorAsync(
        int utilizadorId
    )
    {
        var acessos = await _acessoUtilizadorRepository.ObterAtivosPorUtilizadorIdAsync(
            utilizadorId,
            _timeProvider.GetUtcNow().UtcDateTime
        );
        var agora = _timeProvider.GetUtcNow().UtcDateTime;

        return acessos.Select(acesso => AcessoUtilizadorMapper.MapToReadDTO(acesso, agora));
    }

    public async Task<bool> UtilizadorTemAcessoAFilmeAsync(
        int utilizadorId,
        int filmeId,
        int? festivalId
    )
    {
        return await _acessoVisualizacaoService.PodeVisualizarFilmeAsync(
            utilizadorId,
            filmeId,
            festivalId
        );
    }

    public async Task<bool> UtilizadorTemAcessoASessaoAsync(int utilizadorId, int sessaoId)
    {
        var sessao = await _visualizacaoRepository.ObterSessaoPorIdAsync(sessaoId);

        if (sessao == null)
            throw new KeyNotFoundException("Sessao nao encontrada.");

        return await _acessoVisualizacaoService.PodeVisualizarSessaoAsync(utilizadorId, sessao);
    }
}
