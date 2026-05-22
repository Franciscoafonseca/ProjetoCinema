using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public abstract class CarrinhoAcessoStrategyBase : ICarrinhoAcessoStrategy
{
    public abstract TipoAcesso Tipo { get; }

    public virtual bool PermiteQuantidadeMultipla => false;

    public abstract void ValidarPedido(CarrinhoItemCreateDTO dto);

    public abstract Task ValidarAlvoAsync(CarrinhoItemCreateDTO dto);

    public abstract void ValidarAcesso(Acesso acesso);

    protected static void GarantirSemAlvosExtras(
        CarrinhoItemCreateDTO dto,
        bool permiteFestival = false,
        bool permiteFilme = false,
        bool permiteSessao = false,
        bool permiteDataPasse = false
    )
    {
        if (!permiteFestival && dto.FestivalId is not null)
            throw new ArgumentException("FestivalId nao e valido para este tipo de acesso.");

        if (!permiteFilme && dto.FilmeId is not null)
            throw new ArgumentException("FilmeId nao e valido para este tipo de acesso.");

        if (!permiteSessao && dto.SessaoId is not null)
            throw new ArgumentException("SessaoId nao e valido para este tipo de acesso.");

        if (!permiteDataPasse && dto.DataPasse is not null)
            throw new ArgumentException("DataPasse nao e valida para este tipo de acesso.");
    }
}

public class CarrinhoBilheteSessaoStrategy : CarrinhoAcessoStrategyBase
{
    private readonly ISessaoRepository _sessaoRepository;

    public CarrinhoBilheteSessaoStrategy(ISessaoRepository sessaoRepository)
    {
        _sessaoRepository = sessaoRepository;
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

        if (sessao.Fim <= DateTime.UtcNow)
            throw new InvalidOperationException("A sessao ja terminou.");
    }

    public override void ValidarAcesso(Acesso acesso)
    {
        if (acesso.SessaoId == null || acesso.Sessao == null)
            throw new InvalidOperationException("Bilhete de sessao sem sessao associada.");

        if (acesso.Sessao.Fim <= DateTime.UtcNow)
            throw new InvalidOperationException("A sessao ja terminou.");
    }
}

public class CarrinhoPasseDiarioStrategy : CarrinhoAcessoStrategyBase
{
    private readonly IFestivalRepository _festivalRepository;

    public CarrinhoPasseDiarioStrategy(IFestivalRepository festivalRepository)
    {
        _festivalRepository = festivalRepository;
    }

    public override TipoAcesso Tipo => TipoAcesso.PasseDiario;

    public override void ValidarPedido(CarrinhoItemCreateDTO dto)
    {
        if (dto.FestivalId is null || dto.DataPasse is null)
            throw new ArgumentException("FestivalId e DataPasse sao obrigatorios para passe diario.");

        GarantirSemAlvosExtras(dto, permiteFestival: true, permiteDataPasse: true);
    }

    public override async Task ValidarAlvoAsync(CarrinhoItemCreateDTO dto)
    {
        var festival = await _festivalRepository.ObterPorIdAsync(dto.FestivalId!.Value);

        if (festival == null)
            throw new KeyNotFoundException("Festival nao encontrado.");

        ValidarDataPasse(dto.DataPasse!.Value, festival);
    }

    public override void ValidarAcesso(Acesso acesso)
    {
        if (acesso.FestivalId == null || acesso.Festival == null || acesso.DataAcesso == null)
            throw new InvalidOperationException("Passe diario sem festival ou data de acesso associada.");

        ValidarDataPasse(acesso.DataAcesso.Value, acesso.Festival);
    }

    private static void ValidarDataPasse(DateTime dataPasse, Festival festival)
    {
        if (dataPasse.Date < festival.StartDate.Date || dataPasse.Date > festival.EndDate.Date)
            throw new InvalidOperationException(
                "A data do passe diario tem de estar dentro do periodo do festival."
            );

        if (dataPasse.Date < DateTime.UtcNow.Date)
            throw new InvalidOperationException("A data do passe diario nao pode estar no passado.");
    }
}

public class CarrinhoPasseCompletoStrategy : CarrinhoAcessoStrategyBase
{
    private readonly IFestivalRepository _festivalRepository;

    public CarrinhoPasseCompletoStrategy(IFestivalRepository festivalRepository)
    {
        _festivalRepository = festivalRepository;
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

        if (festival.EndDate < DateTime.UtcNow)
            throw new InvalidOperationException("O festival ja terminou.");
    }

    public override void ValidarAcesso(Acesso acesso)
    {
        if (acesso.FestivalId == null || acesso.Festival == null)
            throw new InvalidOperationException("Passe completo sem festival associado.");

        if (acesso.Festival.EndDate < DateTime.UtcNow)
            throw new InvalidOperationException("O festival ja terminou.");
    }
}

public class CarrinhoAluguerDigitalStrategy : CarrinhoAcessoStrategyBase
{
    private readonly IFilmeRepository _filmeRepository;

    public CarrinhoAluguerDigitalStrategy(IFilmeRepository filmeRepository)
    {
        _filmeRepository = filmeRepository;
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

    public override void ValidarAcesso(Acesso acesso)
    {
        if (acesso.FilmeId == null || acesso.Filme == null)
            throw new InvalidOperationException("Aluguer digital sem filme associado.");

        if (acesso.DuracaoHoras.GetValueOrDefault(48) <= 0)
            throw new InvalidOperationException("Aluguer digital com duracao invalida.");
    }
}
