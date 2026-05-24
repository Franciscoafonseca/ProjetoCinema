using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Common.Errors;
using OnlineCinemaFestival.Api.Domain;

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
            throw new RegraNegocioException("FestivalId nao e valido para este tipo de acesso.");

        if (!permiteFilme && dto.FilmeId is not null)
            throw new RegraNegocioException("FilmeId nao e valido para este tipo de acesso.");

        if (!permiteSessao && dto.SessaoId is not null)
            throw new RegraNegocioException("SessaoId nao e valido para este tipo de acesso.");

        if (!permiteDataAcesso && dto.DataAcesso is not null)
            throw new RegraNegocioException("DataAcesso nao e valida para este tipo de acesso.");
    }
}
