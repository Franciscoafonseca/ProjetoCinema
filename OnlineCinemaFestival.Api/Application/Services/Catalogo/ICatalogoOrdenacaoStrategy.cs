using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Services.Catalogo;

public interface ICatalogoOrdenacaoStrategy
{
    CatalogoOrdenacao Ordenacao { get; }

    IEnumerable<Filme> Ordenar(IEnumerable<Filme> filmes, bool descendente);
}
