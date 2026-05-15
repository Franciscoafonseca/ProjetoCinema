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
        return sessao == null ? null : SessaoMapper.MapToReadDto(sessao);
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

    public async Task<IEnumerable<SessaoReadDto>> GetDisponiveisAsync()
    {
        var sessoes = await _sessaoRepository.GetDisponiveisAsync(DateTime.UtcNow);
        return sessoes.Select(SessaoMapper.MapToReadDto);
    }

    public async Task<SessaoEstadoReadDto> GetEstadoAsync(int id)
    {
        var sessao = await _sessaoRepository.GetByIdAsync(id);

        if (sessao == null)
            throw new KeyNotFoundException("Sessao nao encontrada.");

        return new SessaoEstadoReadDto
        {
            SessaoId = sessao.Id,
            Estado = SessaoMapper.ObterEstado(sessao.Inicio, sessao.Fim),
            Inicio = sessao.Inicio,
            Fim = sessao.Fim,
        };
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
        ValidateFilmes(dto.FilmeIds);

        var sessao = await _sessaoRepository.GetByIdAsync(id);

        if (sessao == null)
            throw new KeyNotFoundException("Sessão não encontrada.");

        if (dto.Inicio < sessao.Festival.StartDate || dto.Fim > sessao.Festival.EndDate)
            throw new ArgumentException("A sessao deve ocorrer dentro do periodo do festival.");

        await ValidateFilmesExistemAsync(dto.FilmeIds);
        await ValidateFilmesPertencemAoFestivalAsync(sessao.FestivalId, dto.FilmeIds);

        var hasOverlap = await _sessaoRepository.HasOverlapAsync(
            sessao.FestivalId,
            dto.FilmeIds,
            dto.Inicio,
            dto.Fim,
            id
        );

        if (hasOverlap)
            throw new InvalidOperationException(
                "Já existe uma sessão sobreposta com pelo menos um destes filmes neste festival."
            );

        SessaoMapper.MapToExistingSessao(dto, sessao);

        await _sessaoRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var sessao = await _sessaoRepository.GetByIdAsync(id);

        if (sessao == null)
            throw new KeyNotFoundException("Sessão não encontrada.");

        var temAcessosAssociados = await _sessaoRepository.HasAcessosAssociadosAsync(id);

        if (temAcessosAssociados)
            throw new InvalidOperationException(
                "Nao e possivel remover uma sessao com acessos ou bilhetes associados."
            );

        _sessaoRepository.Remove(sessao);
        await _sessaoRepository.SaveChangesAsync();
    }

    private async Task ValidateCreateAsync(SessaoCreateDto dto)
    {
        ValidateDates(dto.Inicio, dto.Fim);
        ValidateFilmes(dto.FilmeIds);

        var festival = await _festivalRepository.GetByIdAsync(dto.FestivalId);

        if (festival == null)
            throw new KeyNotFoundException("Festival não encontrado.");

        if (dto.Inicio < festival.StartDate || dto.Fim > festival.EndDate)
            throw new ArgumentException("A sessão deve ocorrer dentro do período do festival.");

        await ValidateFilmesExistemAsync(dto.FilmeIds);
        await ValidateFilmesPertencemAoFestivalAsync(dto.FestivalId, dto.FilmeIds);

        var hasOverlap = await _sessaoRepository.HasOverlapAsync(
            dto.FestivalId,
            dto.FilmeIds,
            dto.Inicio,
            dto.Fim
        );

        if (hasOverlap)
            throw new InvalidOperationException(
                "Já existe uma sessão sobreposta com pelo menos um destes filmes neste festival."
            );
    }

    private static void ValidateDates(DateTime inicio, DateTime fim)
    {
        if (fim <= inicio)
            throw new ArgumentException(
                "A data de fim da sessão deve ser posterior à data de início."
            );
    }

    private static void ValidateFilmes(IEnumerable<int> filmeIds)
    {
        if (!filmeIds.Any())
            throw new ArgumentException("A sessão deve ter pelo menos um filme.");

        if (filmeIds.Any(id => id <= 0))
            throw new ArgumentException(
                "Todos os filmes da sessão devem ter identificadores válidos."
            );
    }

    private async Task ValidateFilmesExistemAsync(IEnumerable<int> filmeIds)
    {
        foreach (var filmeId in filmeIds.Distinct())
        {
            var filme = await _filmeRepository.GetByIdAsync(filmeId);

            if (filme == null)
                throw new KeyNotFoundException($"Filme com id {filmeId} não encontrado.");
        }
    }

    private async Task ValidateFilmesPertencemAoFestivalAsync(
        int festivalId,
        IEnumerable<int> filmeIds
    )
    {
        foreach (var filmeId in filmeIds.Distinct())
        {
            var pertenceAoFestival = await _festivalFilmeRepository.ExistsAsync(
                festivalId,
                filmeId
            );

            if (!pertenceAoFestival)
                throw new InvalidOperationException(
                    $"O filme com id {filmeId} tem de estar associado ao festival antes de ser incluído numa sessão."
                );
        }
    }
}
