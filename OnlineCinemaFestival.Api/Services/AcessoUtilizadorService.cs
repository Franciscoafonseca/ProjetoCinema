using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Mappers;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class AcessoUtilizadorService : IAcessoUtilizadorService
{
    private readonly IAcessoUtilizadorRepository _acessoUtilizadorRepository;
    private readonly IValidacaoAcessoService _validacaoAcessoService;
    private readonly IVisualizacaoRepository _visualizacaoRepository;

    public AcessoUtilizadorService(
        IAcessoUtilizadorRepository acessoUtilizadorRepository,
        IValidacaoAcessoService validacaoAcessoService,
        IVisualizacaoRepository visualizacaoRepository
    )
    {
        _acessoUtilizadorRepository = acessoUtilizadorRepository;
        _validacaoAcessoService = validacaoAcessoService;
        _visualizacaoRepository = visualizacaoRepository;
    }

    public async Task<IEnumerable<AcessoUtilizadorReadDTO>> ObterAcessosDoUtilizadorAsync(
        int utilizadorId
    )
    {
        var acessos = await _acessoUtilizadorRepository.ObterPorUtilizadorIdAsync(utilizadorId);

        return acessos.Select(AcessoUtilizadorMapper.MapToReadDTO);
    }

    public async Task<IEnumerable<AcessoUtilizadorReadDTO>> ObterAcessosAtivosDoUtilizadorAsync(
        int utilizadorId
    )
    {
        var acessos = await _acessoUtilizadorRepository.ObterAtivosPorUtilizadorIdAsync(
            utilizadorId,
            DateTime.UtcNow
        );

        return acessos.Select(AcessoUtilizadorMapper.MapToReadDTO);
    }

    public async Task<bool> UtilizadorTemAcessoAFilmeAsync(
        int utilizadorId,
        int filmeId,
        int? festivalId
    )
    {
        return await _validacaoAcessoService.PodeVisualizarFilmeAsync(
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

        return await _validacaoAcessoService.PodeVisualizarSessaoAsync(utilizadorId, sessao);
    }
}
