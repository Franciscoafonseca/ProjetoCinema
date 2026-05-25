using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Mapping;
using OnlineCinemaFestival.Api.Domain;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class PremioFestivalService : IPremioFestivalService
{
    private readonly IPremioFestivalRepository _repository;
    private readonly TimeProvider _timeProvider;
    private readonly IEnumerable<IVotoPremioObserver> _votoObservers;

    public PremioFestivalService(
        IPremioFestivalRepository repository,
        TimeProvider timeProvider,
        IEnumerable<IVotoPremioObserver> votoObservers
    )
    {
        _repository = repository;
        _timeProvider = timeProvider;
        _votoObservers = votoObservers;
    }

    public async Task<PremioFestivalReadDTO> CriarPremioAsync(
        int festivalId,
        CriarPremioFestivalDTO dto
    )
    {
        ValidarDadosPremio(dto);

        if (!await _repository.FestivalExisteAsync(festivalId))
            throw new KeyNotFoundException("Festival nao encontrado.");

        var premio = new PremioFestival
        {
            FestivalId = festivalId,
            Nome = dto.Nome.Trim(),
            Descricao = dto.Descricao.Trim(),
            DataAberturaVotacao = dto.DataAberturaVotacao.ToUniversalTime(),
            DataFechoVotacao = dto.DataFechoVotacao.ToUniversalTime(),
            EstadoPremio = EstadoPremio.Aberto,
        };

        await _repository.AddPremioAsync(premio);
        await _repository.SaveChangesAsync();

        return PremioFestivalMapper.MapToReadDTO(premio, AgoraUtc());
    }

    public async Task<PremioFestivalReadDTO> AbrirVotacaoAsync(int premioFestivalId)
    {
        var premio = await ObterPremioAsync(premioFestivalId);

        if (premio.EstadoPremio == EstadoPremio.Publicado)
            throw new InvalidOperationException("Resultados ja publicados para este premio.");

        if (premio.DataFechoVotacao <= AgoraUtc())
            throw new InvalidOperationException("Nao e possivel abrir uma votacao ja terminada.");

        premio.EstadoPremio = EstadoPremio.Aberto;
        await _repository.SaveChangesAsync();

        return PremioFestivalMapper.MapToReadDTO(premio, AgoraUtc());
    }

    public async Task VotarAsync(int premioFestivalId, int filmeId, int utilizadorId)
    {
        var premio = await ObterPremioAsync(premioFestivalId);
        ValidarVotacaoAberta(premio);

        if (!await _repository.FilmeElegivelAsync(premio.FestivalId, filmeId))
            throw new InvalidOperationException(
                "O filme nao pertence ao festival ou nao esta elegivel para premios."
            );

        if (await _repository.UtilizadorJaVotouAsync(premioFestivalId, utilizadorId))
            throw new InvalidOperationException("Ja votaste neste premio.");

        if (!await _repository.UtilizadorViuTodosFilmesElegiveisAsync(premio.FestivalId, utilizadorId))
            throw new UnauthorizedAccessException(
                "Para votar, tens de ver pelo menos uma sessao de cada filme elegivel do festival."
            );

        var voto = new VotoPremioFestival
        {
            PremioFestivalId = premioFestivalId,
            FestivalId = premio.FestivalId,
            FilmeId = filmeId,
            UtilizadorId = utilizadorId,
            DataVoto = AgoraUtc(),
        };

        await _repository.AddVotoAsync(voto);

        if (!await _repository.TrySaveChangesAsync())
            throw new InvalidOperationException("Ja votaste neste premio.");

        await Task.WhenAll(_votoObservers.Select(observer => observer.NotificarAsync(voto)));
    }

    public async Task<PremioFestivalReadDTO> FecharVotacaoAsync(int premioFestivalId)
    {
        var premio = await ObterPremioAsync(premioFestivalId);

        if (premio.EstadoPremio == EstadoPremio.Publicado)
            throw new InvalidOperationException("Resultados ja publicados para este premio.");

        premio.EstadoPremio = EstadoPremio.Fechado;
        await _repository.SaveChangesAsync();

        return PremioFestivalMapper.MapToReadDTO(premio, AgoraUtc());
    }

    public async Task<ResultadoPremioFestivalDTO> PublicarResultadosAsync(
        int premioFestivalId,
        int publicadoPorUtilizadorId
    )
    {
        var resultado = await PublicarResultadoInternoAsync(
            premioFestivalId,
            publicadoPorUtilizadorId
        );

        return PremioFestivalMapper.MapResultadoToDTO(resultado);
    }

    public async Task<IEnumerable<ResultadoPremioFestivalDTO>> ObterResultadosPublicosAsync(
        int? festivalId = null,
        int? filmeId = null
    )
    {
        var resultados = await _repository.ObterResultadosPublicosAsync(festivalId, filmeId);
        return resultados.Select(PremioFestivalMapper.MapResultadoToDTO);
    }

    public async Task<IEnumerable<PremioFestivalReadDTO>> ObterPremiosPorFestivalAsync(
        int festivalId,
        bool incluirRascunhos
    )
    {
        if (!await _repository.FestivalExisteAsync(festivalId))
            throw new KeyNotFoundException("Festival nao encontrado.");

        var premios = await _repository.ObterPremiosPorFestivalAsync(festivalId, incluirRascunhos);
        var agora = AgoraUtc();
        return premios.Select(p => PremioFestivalMapper.MapToReadDTO(p, agora));
    }

    private async Task<ResultadoPremioFestival> PublicarResultadoInternoAsync(
        int premioFestivalId,
        int? publicadoPorUtilizadorId
    )
    {
        var premio = await ObterPremioComResultadoAsync(premioFestivalId);

        if (premio.EstadoPremio == EstadoPremio.Publicado)
            throw new InvalidOperationException("Resultados ja publicados para este premio.");

        if (AgoraUtc() <= premio.DataFechoVotacao)
            throw new InvalidOperationException("A votacao ainda nao terminou.");

        var vencedor = await _repository.ObterVencedorPorVotosAsync(premioFestivalId);

        if (vencedor == null)
            throw new InvalidOperationException("Nao existem votos para publicar resultados.");

        var resultado =
            premio.Resultado
            ?? new ResultadoPremioFestival { PremioFestivalId = premioFestivalId };

        resultado.FilmeIdVencedor = vencedor.Value.FilmeId;
        resultado.TotalVotos = vencedor.Value.TotalVotos;
        resultado.PublicadoEm = AgoraUtc();
        resultado.PublicadoPorUtilizadorId = publicadoPorUtilizadorId;

        if (premio.Resultado == null)
            await _repository.AddResultadoAsync(resultado);

        premio.EstadoPremio = EstadoPremio.Publicado;
        await _repository.SaveChangesAsync();

        return await _repository.ObterResultadoCompletoAsync(premioFestivalId)
            ?? throw new InvalidOperationException("Resultado nao encontrado apos publicacao.");
    }

    private async Task<PremioFestival> ObterPremioAsync(int premioFestivalId)
    {
        return await _repository.ObterPremioAsync(premioFestivalId)
            ?? throw new KeyNotFoundException("Premio nao encontrado.");
    }

    private async Task<PremioFestival> ObterPremioComResultadoAsync(int premioFestivalId)
    {
        return await _repository.ObterPremioComResultadoAsync(premioFestivalId)
            ?? throw new KeyNotFoundException("Premio nao encontrado.");
    }

    private static void ValidarDadosPremio(CriarPremioFestivalDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new ArgumentException("O nome do premio e obrigatorio.");

        if (dto.DataFechoVotacao.ToUniversalTime() <= dto.DataAberturaVotacao.ToUniversalTime())
            throw new ArgumentException("A data de fecho deve ser posterior a data de abertura.");
    }

    private void ValidarVotacaoAberta(PremioFestival premio)
    {
        ValidarEstadoVotacaoAberta(premio);
        var agora = AgoraUtc();
        if (agora < premio.DataAberturaVotacao || agora > premio.DataFechoVotacao)
            throw new InvalidOperationException("A votacao esta fora do periodo permitido.");
    }

    private static void ValidarEstadoVotacaoAberta(PremioFestival premio)
    {
        if (premio.EstadoPremio != EstadoPremio.Aberto)
            throw new InvalidOperationException("A votacao nao esta aberta.");
    }

    private DateTime AgoraUtc() => _timeProvider.GetUtcNow().UtcDateTime;
}
