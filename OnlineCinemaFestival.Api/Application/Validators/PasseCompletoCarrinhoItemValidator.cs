using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Common.Errors;
using OnlineCinemaFestival.Api.Domain;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

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
            throw new RegraNegocioException("FestivalId e obrigatorio para passe completo.");

        GarantirSemAlvosExtras(dto, permiteFestival: true);
    }

    public override async Task ValidarAlvoAsync(CarrinhoItemCreateDTO dto)
    {
        var festival = await _festivalRepository.ObterPorIdAsync(dto.FestivalId!.Value);

        if (festival == null)
            throw new RecursoNaoEncontradoException("Festival nao encontrado.");

        ValidarFestival(festival);
    }

    public override Task<Acesso?> ObterAcessoAtivoAsync(CarrinhoItemCreateDTO dto)
    {
        return _acessoRepository.ObterPasseCompletoAtivoAsync(dto.FestivalId!.Value);
    }

    public override void ValidarAcesso(Acesso acesso)
    {
        if (acesso.FestivalId == null || acesso.Festival == null)
            throw new ConflitoDominioException("Passe completo sem festival associado.");

        ValidarFestival(acesso.Festival);
    }

    private void ValidarFestival(Festival festival)
    {
        if (festival.EndDate < _timeProvider.GetUtcNow().UtcDateTime)
            throw new ConflitoDominioException("O festival ja terminou.");
    }
}
