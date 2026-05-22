using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Mappers;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class SessaoService : ISessaoService
{
    private readonly ISessaoRepository _sessaoRepository;
    private readonly IFestivalRepository _festivalRepository;
    private readonly IFilmeRepository _filmeRepository;
    private readonly IFestivalFilmeRepository _festivalFilmeRepository;
    private readonly IAcessoAutomaticoService _acessoAutomaticoService;

    public SessaoService(
        ISessaoRepository sessaoRepository,
        IFestivalRepository festivalRepository,
        IFilmeRepository filmeRepository,
        IFestivalFilmeRepository festivalFilmeRepository,
        IAcessoAutomaticoService acessoAutomaticoService
    )
    {
        _sessaoRepository = sessaoRepository;
        _festivalRepository = festivalRepository;
        _filmeRepository = filmeRepository;
        _festivalFilmeRepository = festivalFilmeRepository;
        _acessoAutomaticoService = acessoAutomaticoService;
    }

    public async Task<IEnumerable<SessaoReadDTO>> ObterTodosAsync()
    {
        var sessoes = await _sessaoRepository.ObterTodosAsync();
        return sessoes.Select(SessaoMapper.MapToReadDTO);
    }

    public async Task<SessaoReadDTO?> ObterPorIdAsync(int id)
    {
        var sessao = await _sessaoRepository.ObterPorIdAsync(id);
        return sessao == null ? null : SessaoMapper.MapToReadDTO(sessao);
    }

    public async Task<IEnumerable<SessaoReadDTO>> ObterPorFestivalIdAsync(int festivalId)
    {
        if (await _festivalRepository.ObterPorIdAsync(festivalId) == null)
            throw new KeyNotFoundException("Festival nao encontrado.");

        var sessoes = await _sessaoRepository.ObterPorFestivalIdAsync(festivalId);
        return sessoes.Select(SessaoMapper.MapToReadDTO);
    }

    public async Task<IEnumerable<SessaoReadDTO>> ObterPorFilmeIdAsync(int filmeId)
    {
        if (await _filmeRepository.ObterPorIdAsync(filmeId) == null)
            throw new KeyNotFoundException("Filme nao encontrado.");

        var sessoes = await _sessaoRepository.ObterPorFilmeIdAsync(filmeId);
        return sessoes.Select(SessaoMapper.MapToReadDTO);
    }

    public async Task<IEnumerable<SessaoReadDTO>> ObterDisponiveisAsync()
    {
        var sessoes = await _sessaoRepository.ObterDisponiveisAsync(DateTime.UtcNow);
        return sessoes.Select(SessaoMapper.MapToReadDTO);
    }

    public async Task<SessaoEstadoReadDTO> ObterEstadoAsync(int id)
    {
        var sessao = await _sessaoRepository.ObterPorIdAsync(id);

        if (sessao == null)
            throw new KeyNotFoundException("Sessao nao encontrada.");

        return new SessaoEstadoReadDTO
        {
            SessaoId = sessao.Id,
            Estado = SessaoMapper.ObterEstado(sessao.Inicio, sessao.Fim),
            Inicio = sessao.Inicio,
            Fim = sessao.Fim,
        };
    }

    public async Task<SessaoReadDTO> CriarAsync(SessaoCreateDTO dto)
    {
        await ValidateAsync(dto.FestivalId, dto.FilmeId, dto.Inicio, dto.Fim);

        var sessao = SessaoMapper.MapFromCreateDTO(dto);

        await _sessaoRepository.AddAsync(sessao);
        await _sessaoRepository.SaveChangesAsync();
        await _acessoAutomaticoService.GarantirParaSessaoAsync(sessao.Id);

        var created = await _sessaoRepository.ObterPorIdAsync(sessao.Id);
        return SessaoMapper.MapToReadDTO(created!);
    }

    public async Task AtualizarAsync(int id, SessaoUpdateDTO dto)
    {
        var sessao = await _sessaoRepository.ObterPorIdAsync(id);

        if (sessao == null)
            throw new KeyNotFoundException("Sessao nao encontrada.");

        await ValidateAsync(sessao.FestivalId, dto.FilmeId, dto.Inicio, dto.Fim, id);

        SessaoMapper.MapToExistingSessao(dto, sessao);

        await _sessaoRepository.SaveChangesAsync();
        await _acessoAutomaticoService.GarantirParaSessaoAsync(sessao.Id);
    }

    public async Task EliminarAsync(int id)
    {
        var sessao = await _sessaoRepository.ObterPorIdAsync(id);

        if (sessao == null)
            throw new KeyNotFoundException("Sessao nao encontrada.");

        if (await _sessaoRepository.HasAcessosAssociadosAsync(id))
            throw new InvalidOperationException(
                "Nao e possivel remover uma sessao com acessos ou bilhetes associados."
            );

        _sessaoRepository.Remove(sessao);
        await _sessaoRepository.SaveChangesAsync();
    }

    private async Task ValidateAsync(
        int festivalId,
        int filmeId,
        DateTime inicio,
        DateTime fim,
        int? ignoreSessaoId = null
    )
    {
        if (fim <= inicio)
            throw new ArgumentException("A data de fim da sessao deve ser posterior ao inicio.");

        var festival = await _festivalRepository.ObterPorIdAsync(festivalId);

        if (festival == null)
            throw new KeyNotFoundException("Festival nao encontrado.");

        if (await _filmeRepository.ObterPorIdAsync(filmeId) == null)
            throw new KeyNotFoundException("Filme nao encontrado.");

        if (inicio < festival.StartDate || fim > festival.EndDate)
            throw new ArgumentException("A sessao deve ocorrer dentro do periodo do festival.");

        if (!await _festivalFilmeRepository.ExisteAsync(festivalId, filmeId))
            throw new InvalidOperationException(
                "O filme tem de estar associado ao festival antes de criar a sessao."
            );

        if (await _sessaoRepository.HasOverlapAsync(festivalId, filmeId, inicio, fim, ignoreSessaoId))
            throw new InvalidOperationException(
                "Ja existe uma sessao sobreposta para este filme neste festival."
            );
    }

}
