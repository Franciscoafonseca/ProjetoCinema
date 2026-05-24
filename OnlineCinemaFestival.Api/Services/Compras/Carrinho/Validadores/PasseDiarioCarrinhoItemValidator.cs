using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Excecoes;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

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
            throw new RegraNegocioException(
                "FestivalId e DataAcesso sao obrigatorios para passe diario."
            );

        GarantirSemAlvosExtras(dto, permiteFestival: true, permiteDataAcesso: true);
    }

    public override async Task ValidarAlvoAsync(CarrinhoItemCreateDTO dto)
    {
        var festival = await _festivalRepository.ObterPorIdAsync(dto.FestivalId!.Value);

        if (festival == null)
            throw new RecursoNaoEncontradoException("Festival nao encontrado.");

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
            throw new ConflitoDominioException(
                "Passe diario sem festival ou data de acesso associada."
            );

        ValidarDataAcesso(acesso.DataAcesso.Value, acesso.Festival);
    }

    private void ValidarDataAcesso(DateTime dataAcesso, Festival festival)
    {
        if (dataAcesso.Date < festival.StartDate.Date || dataAcesso.Date > festival.EndDate.Date)
            throw new ConflitoDominioException(
                "A data do passe diario tem de estar dentro do periodo do festival."
            );

        if (dataAcesso.Date < _timeProvider.GetUtcNow().UtcDateTime.Date)
            throw new ConflitoDominioException("A data do passe diario nao pode estar no passado.");
    }
}
