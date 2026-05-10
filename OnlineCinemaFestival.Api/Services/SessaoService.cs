using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Mappers;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class SessaoService : ISessaoService
{
    private readonly ISessaoRepository _sessaoRepository;
    private readonly IFestivalRepository _festivalRepository;
    private readonly IFilmeRepository _filmeRepository;
    private readonly IFestivalFilmeRepository _festivalFilmeRepository;

    public SessaoService(
        ISessaoRepository sessaoRepository,
        IFestivalRepository festivalRepository,
        IFilmeRepository filmeRepository,
        IFestivalFilmeRepository festivalFilmeRepository
    )
    {
        _sessaoRepository = sessaoRepository;
        _festivalRepository = festivalRepository;
        _filmeRepository = filmeRepository;
        _festivalFilmeRepository = festivalFilmeRepository;
    }

    public async Task<IEnumerable<SessaoReadDto>> GetAllAsync()
    {
        var sessoes = await _sessaoRepository.GetAllAsync();

        return sessoes.Select(SessaoMapper.MapToReadDto);
    }

    public async Task<SessaoReadDto?> GetByIdAsync(int id)
    {
        var sessao = await _sessaoRepository.GetByIdAsync(id);

        if (sessao == null)
            return null;

        return SessaoMapper.MapToReadDto(sessao);
    }

    public async Task<IEnumerable<SessaoReadDto>> GetByFestivalIdAsync(int festivalId)
    {
        var festival = await _festivalRepository.GetByIdAsync(festivalId);

        if (festival == null)
            throw new KeyNotFoundException("Festival não encontrado.");

        var sessoes = await _sessaoRepository.GetByFestivalIdAsync(festivalId);

        return sessoes.Select(SessaoMapper.MapToReadDto);
    }

    public async Task<IEnumerable<SessaoReadDto>> GetByFilmeIdAsync(int filmeId)
    {
        var filme = await _filmeRepository.GetByIdAsync(filmeId);

        if (filme == null)
            throw new KeyNotFoundException("Filme não encontrado.");

        var sessoes = await _sessaoRepository.GetByFilmeIdAsync(filmeId);

        return sessoes.Select(SessaoMapper.MapToReadDto);
    }

    public async Task<SessaoReadDto> CreateAsync(SessaoCreateDto dto)
    {
        await ValidateCreateAsync(dto);

        var sessao = SessaoMapper.MapFromCreateDto(dto);

        await _sessaoRepository.AddAsync(sessao);
        await _sessaoRepository.SaveChangesAsync();

        var created = await _sessaoRepository.GetByIdAsync(sessao.Id);

        return SessaoMapper.MapToReadDto(created!);
    }

    public async Task UpdateAsync(int id, SessaoUpdateDto dto)
    {
        ValidateDates(dto.Inicio, dto.Fim);

        var sessao = await _sessaoRepository.GetByIdAsync(id);

        if (sessao == null)
            throw new KeyNotFoundException("Sessão não encontrada.");

        var hasOverlap = await _sessaoRepository.HasOverlapAsync(
            sessao.FestivalId,
            sessao.FilmeId,
            dto.Inicio,
            dto.Fim,
            id
        );

        if (hasOverlap)
            throw new InvalidOperationException(
                "Já existe uma sessão sobreposta para este filme neste festival."
            );

        SessaoMapper.MapToExistingSessao(dto, sessao);

        await _sessaoRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var sessao = await _sessaoRepository.GetByIdAsync(id);

        if (sessao == null)
            throw new KeyNotFoundException("Sessão não encontrada.");

        _sessaoRepository.Remove(sessao);

        await _sessaoRepository.SaveChangesAsync();
    }

    private async Task ValidateCreateAsync(SessaoCreateDto dto)
    {
        ValidateDates(dto.Inicio, dto.Fim);

        var festival = await _festivalRepository.GetByIdAsync(dto.FestivalId);

        if (festival == null)
            throw new KeyNotFoundException("Festival não encontrado.");

        var filme = await _filmeRepository.GetByIdAsync(dto.FilmeId);

        if (filme == null)
            throw new KeyNotFoundException("Filme não encontrado.");

        var filmeEstaNoFestival = await _festivalFilmeRepository.ExistsAsync(
            dto.FestivalId,
            dto.FilmeId
        );

        if (!filmeEstaNoFestival)
            throw new InvalidOperationException(
                "O filme tem de estar associado ao festival antes de criar uma sessão."
            );

        if (dto.Inicio < festival.StartDate || dto.Fim > festival.EndDate)
            throw new ArgumentException("A sessão deve ocorrer dentro do período do festival.");

        var hasOverlap = await _sessaoRepository.HasOverlapAsync(
            dto.FestivalId,
            dto.FilmeId,
            dto.Inicio,
            dto.Fim
        );

        if (hasOverlap)
            throw new InvalidOperationException(
                "Já existe uma sessão sobreposta para este filme neste festival."
            );
    }

    private static void ValidateDates(DateTime inicio, DateTime fim)
    {
        if (fim <= inicio)
            throw new ArgumentException(
                "A data de fim da sessão deve ser posterior à data de início."
            );
    }
}
