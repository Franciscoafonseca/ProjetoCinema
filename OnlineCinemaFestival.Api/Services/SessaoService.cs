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
        var festival = await _festivalRepository.ObterPorIdAsync(festivalId);

        if (festival == null)
            throw new KeyNotFoundException("Festival não encontrado.");

        var sessoes = await _sessaoRepository.ObterPorFestivalIdAsync(festivalId);
        return sessoes.Select(SessaoMapper.MapToReadDTO);
    }

    public async Task<IEnumerable<SessaoReadDTO>> ObterPorFilmeIdAsync(int filmeId)
    {
        var filme = await _filmeRepository.ObterPorIdAsync(filmeId);

        if (filme == null)
            throw new KeyNotFoundException("Filme não encontrado.");

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
        await ValidateCreateAsync(dto);

        var sessao = SessaoMapper.MapFromCreateDTO(dto);

        await _sessaoRepository.AddAsync(sessao);
        await _sessaoRepository.SaveChangesAsync();

        var created = await _sessaoRepository.ObterPorIdAsync(sessao.Id);
        return SessaoMapper.MapToReadDTO(created!);
    }

    public async Task AtualizarAsync(int id, SessaoUpdateDTO dto)
    {
        ValidateDates(dto.Inicio, dto.Fim);
        var filmeIds = ObterFilmeIds(dto.FilmeIds, dto.Filmes);
        ValidateFilmes(filmeIds);

        var sessao = await _sessaoRepository.ObterPorIdAsync(id);

        if (sessao == null)
            throw new KeyNotFoundException("Sessão não encontrada.");

        if (dto.Inicio < sessao.Festival.StartDate || dto.Fim > sessao.Festival.EndDate)
            throw new ArgumentException("A sessao deve ocorrer dentro do periodo do festival.");

        ValidateHorariosFilmes(dto.Filmes, dto.Inicio, dto.Fim);
        await ValidateFilmesExistemAsync(filmeIds);
        await ValidateFilmesPertencemAoFestivalAsync(sessao.FestivalId, filmeIds);

        var hasOverlap = await _sessaoRepository.HasOverlapAsync(
            sessao.FestivalId,
            filmeIds,
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

    public async Task EliminarAsync(int id)
    {
        var sessao = await _sessaoRepository.ObterPorIdAsync(id);

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

    private async Task ValidateCreateAsync(SessaoCreateDTO dto)
    {
        ValidateDates(dto.Inicio, dto.Fim);
        var filmeIds = ObterFilmeIds(dto.FilmeIds, dto.Filmes);
        ValidateFilmes(filmeIds);

        var festival = await _festivalRepository.ObterPorIdAsync(dto.FestivalId);

        if (festival == null)
            throw new KeyNotFoundException("Festival não encontrado.");

        if (dto.Inicio < festival.StartDate || dto.Fim > festival.EndDate)
            throw new ArgumentException("A sessão deve ocorrer dentro do período do festival.");

        ValidateHorariosFilmes(dto.Filmes, dto.Inicio, dto.Fim);
        await ValidateFilmesExistemAsync(filmeIds);
        await ValidateFilmesPertencemAoFestivalAsync(dto.FestivalId, filmeIds);

        var hasOverlap = await _sessaoRepository.HasOverlapAsync(
            dto.FestivalId,
            filmeIds,
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

    private static List<int> ObterFilmeIds(
        IEnumerable<int> filmeIds,
        IEnumerable<SessaoFilmeCreateDTO> filmes
    )
    {
        var idsComHorario = filmes.Select(f => f.FilmeId).Where(id => id > 0).ToList();
        return idsComHorario.Any() ? idsComHorario : filmeIds.ToList();
    }

    private static void ValidateHorariosFilmes(
        IEnumerable<SessaoFilmeCreateDTO> filmes,
        DateTime inicioSessao,
        DateTime fimSessao
    )
    {
        foreach (var filme in filmes)
        {
            if (filme.HoraInicio.HasValue && filme.HoraFim.HasValue && filme.HoraFim <= filme.HoraInicio)
                throw new ArgumentException("A hora de fim do filme deve ser posterior a hora de inicio.");

            if (filme.HoraInicio.HasValue && filme.HoraInicio < inicioSessao)
                throw new ArgumentException("A hora de inicio do filme nao pode ser anterior ao inicio da sessao.");

            if (filme.HoraFim.HasValue && filme.HoraFim > fimSessao)
                throw new ArgumentException("A hora de fim do filme nao pode ultrapassar o fim da sessao.");
        }
    }

    private async Task ValidateFilmesExistemAsync(IEnumerable<int> filmeIds)
    {
        foreach (var filmeId in filmeIds.Distinct())
        {
            var filme = await _filmeRepository.ObterPorIdAsync(filmeId);

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
