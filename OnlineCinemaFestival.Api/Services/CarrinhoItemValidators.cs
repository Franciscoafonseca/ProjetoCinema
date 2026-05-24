using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public abstract class CarrinhoItemValidatorBase : ICarrinhoItemValidator
{
    public abstract TipoAcesso Tipo { get; }

    public virtual bool PermiteQuantidadeMultipla => false;

    public abstract void ValidarPedido(CarrinhoItemCreateDTO dto);

    public abstract Task ValidarAlvoAsync(CarrinhoItemCreateDTO dto);

    public abstract Task<Acesso?> ObterAcessoAtivoAsync(CarrinhoItemCreateDTO dto);

    public abstract void ValidarAcesso(Acesso acesso);

    protected static void GarantirSemAlvosExtras(
        CarrinhoItemCreateDTO dto,
        bool permiteFestival = false,
        bool permiteFilme = false,
        bool permiteSessao = false,
        bool permiteDataAcesso = false
    )
    {
        if (!permiteFestival && dto.FestivalId is not null)
            throw new ArgumentException("FestivalId nao e valido para este tipo de acesso.");

        if (!permiteFilme && dto.FilmeId is not null)
            throw new ArgumentException("FilmeId nao e valido para este tipo de acesso.");

        if (!permiteSessao && dto.SessaoId is not null)
            throw new ArgumentException("SessaoId nao e valido para este tipo de acesso.");

        if (!permiteDataAcesso && dto.DataAcesso is not null)
            throw new ArgumentException("DataAcesso nao e valida para este tipo de acesso.");
    }
}

public sealed class BilheteSessaoCarrinhoItemValidator : CarrinhoItemValidatorBase
{
    private readonly ISessaoRepository _sessaoRepository;
    private readonly IAcessoRepository _acessoRepository;
    private readonly TimeProvider _timeProvider;

    public BilheteSessaoCarrinhoItemValidator(
        ISessaoRepository sessaoRepository,
        IAcessoRepository acessoRepository,
        TimeProvider timeProvider
    )
    {
        _sessaoRepository = sessaoRepository;
        _acessoRepository = acessoRepository;
        _timeProvider = timeProvider;
    }

    public override TipoAcesso Tipo => TipoAcesso.BilheteSessao;

    public override bool PermiteQuantidadeMultipla => true;

    public override void ValidarPedido(CarrinhoItemCreateDTO dto)
    {
        if (dto.SessaoId is null)
            throw new ArgumentException("SessaoId e obrigatorio para bilhete de sessao.");

        GarantirSemAlvosExtras(dto, permiteSessao: true);
    }

    public override async Task ValidarAlvoAsync(CarrinhoItemCreateDTO dto)
    {
        var sessao = await _sessaoRepository.ObterPorIdAsync(dto.SessaoId!.Value);

        if (sessao == null)
            throw new KeyNotFoundException("Sessao nao encontrada.");

        ValidarSessao(sessao);
    }

    public override Task<Acesso?> ObterAcessoAtivoAsync(CarrinhoItemCreateDTO dto)
    {
        return _acessoRepository.ObterBilheteSessaoAtivoAsync(dto.SessaoId!.Value);
    }

    public override void ValidarAcesso(Acesso acesso)
    {
        if (acesso.SessaoId == null || acesso.Sessao == null)
            throw new InvalidOperationException("Bilhete de sessao sem sessao associada.");

        ValidarSessao(acesso.Sessao);
    }

    private void ValidarSessao(Sessao sessao)
    {
        if (sessao.Fim <= _timeProvider.GetUtcNow().UtcDateTime)
            throw new InvalidOperationException("A sessao ja terminou.");
    }
}

public sealed class PasseDiarioCarrinhoItemValidator : CarrinhoItemValidatorBase
{
    private readonly IFestivalRepository _festivalRepository;
    private readonly IAcessoRepository _acessoRepository;
    private readonly TimeProvider _timeProvider;

    public PasseDiarioCarrinhoItemValidator(
        IFestivalRepository festivalRepository,
        IAcessoRepository acessoRepository,
        TimeProvider timeProvider
    )
    {
        _festivalRepository = festivalRepository;
        _acessoRepository = acessoRepository;
        _timeProvider = timeProvider;
    }

    public override TipoAcesso Tipo => TipoAcesso.PasseDiario;

    public override void ValidarPedido(CarrinhoItemCreateDTO dto)
    {
        if (dto.FestivalId is null || dto.DataAcesso is null)
            throw new ArgumentException("FestivalId e DataAcesso sao obrigatorios para passe diario.");

        GarantirSemAlvosExtras(dto, permiteFestival: true, permiteDataAcesso: true);
    }

    public override async Task ValidarAlvoAsync(CarrinhoItemCreateDTO dto)
    {
        var festival = await _festivalRepository.ObterPorIdAsync(dto.FestivalId!.Value);

        if (festival == null)
            throw new KeyNotFoundException("Festival nao encontrado.");

        ValidarDataAcesso(dto.DataAcesso!.Value, festival);
    }

    public override Task<Acesso?> ObterAcessoAtivoAsync(CarrinhoItemCreateDTO dto)
    {
        return _acessoRepository.ObterPasseDiarioAtivoAsync(
            dto.FestivalId!.Value,
            dto.DataAcesso!.Value
        );
    }

    public override void ValidarAcesso(Acesso acesso)
    {
        if (acesso.FestivalId == null || acesso.Festival == null || acesso.DataAcesso == null)
            throw new InvalidOperationException("Passe diario sem festival ou data de acesso associada.");

        ValidarDataAcesso(acesso.DataAcesso.Value, acesso.Festival);
    }

    private void ValidarDataAcesso(DateTime dataAcesso, Festival festival)
    {
        if (dataAcesso.Date < festival.StartDate.Date || dataAcesso.Date > festival.EndDate.Date)
            throw new InvalidOperationException(
                "A data do passe diario tem de estar dentro do periodo do festival."
            );

        if (dataAcesso.Date < _timeProvider.GetUtcNow().UtcDateTime.Date)
            throw new InvalidOperationException("A data do passe diario nao pode estar no passado.");
    }
}

public sealed class PasseCompletoCarrinhoItemValidator : CarrinhoItemValidatorBase
{
    private readonly IFestivalRepository _festivalRepository;
    private readonly IAcessoRepository _acessoRepository;
    private readonly TimeProvider _timeProvider;

    public PasseCompletoCarrinhoItemValidator(
        IFestivalRepository festivalRepository,
        IAcessoRepository acessoRepository,
        TimeProvider timeProvider
    )
    {
        _festivalRepository = festivalRepository;
        _acessoRepository = acessoRepository;
        _timeProvider = timeProvider;
    }

    public override TipoAcesso Tipo => TipoAcesso.PasseCompleto;

    public override void ValidarPedido(CarrinhoItemCreateDTO dto)
    {
        if (dto.FestivalId is null)
            throw new ArgumentException("FestivalId e obrigatorio para passe completo.");

        GarantirSemAlvosExtras(dto, permiteFestival: true);
    }

    public override async Task ValidarAlvoAsync(CarrinhoItemCreateDTO dto)
    {
        var festival = await _festivalRepository.ObterPorIdAsync(dto.FestivalId!.Value);

        if (festival == null)
            throw new KeyNotFoundException("Festival nao encontrado.");

        ValidarFestival(festival);
    }

    public override Task<Acesso?> ObterAcessoAtivoAsync(CarrinhoItemCreateDTO dto)
    {
        return _acessoRepository.ObterPasseCompletoAtivoAsync(dto.FestivalId!.Value);
    }

    public override void ValidarAcesso(Acesso acesso)
    {
        if (acesso.FestivalId == null || acesso.Festival == null)
            throw new InvalidOperationException("Passe completo sem festival associado.");

        ValidarFestival(acesso.Festival);
    }

    private void ValidarFestival(Festival festival)
    {
        if (festival.EndDate < _timeProvider.GetUtcNow().UtcDateTime)
            throw new InvalidOperationException("O festival ja terminou.");
    }
}

public sealed class AluguerDigitalCarrinhoItemValidator : CarrinhoItemValidatorBase
{
    private readonly IFilmeRepository _filmeRepository;
    private readonly IAcessoRepository _acessoRepository;

    public AluguerDigitalCarrinhoItemValidator(
        IFilmeRepository filmeRepository,
        IAcessoRepository acessoRepository
    )
    {
        _filmeRepository = filmeRepository;
        _acessoRepository = acessoRepository;
    }

    public override TipoAcesso Tipo => TipoAcesso.AluguerDigital;

    public override void ValidarPedido(CarrinhoItemCreateDTO dto)
    {
        if (dto.FilmeId is null)
            throw new ArgumentException("FilmeId e obrigatorio para aluguer digital.");

        GarantirSemAlvosExtras(dto, permiteFilme: true);
    }

    public override async Task ValidarAlvoAsync(CarrinhoItemCreateDTO dto)
    {
        if (await _filmeRepository.ObterPorIdAsync(dto.FilmeId!.Value) == null)
            throw new KeyNotFoundException("Filme nao encontrado.");
    }

    public override Task<Acesso?> ObterAcessoAtivoAsync(CarrinhoItemCreateDTO dto)
    {
        return _acessoRepository.ObterAluguerDigitalAtivoAsync(dto.FilmeId!.Value);
    }

    public override void ValidarAcesso(Acesso acesso)
    {
        if (acesso.FilmeId == null || acesso.Filme == null)
            throw new InvalidOperationException("Aluguer digital sem filme associado.");

        if (acesso.DuracaoHoras is null or <= 0)
            throw new InvalidOperationException("Aluguer digital com duracao invalida.");
    }
}
