using OnlineCinemaFestival.Api.Application.DTOs;

namespace OnlineCinemaFestival.Api.Services.Catalogo;

public interface ICatalogoOrdenacaoStrategyFactory
{
    ICatalogoOrdenacaoStrategy GetStrategy(CatalogoOrdenacao ordenacao);
}
