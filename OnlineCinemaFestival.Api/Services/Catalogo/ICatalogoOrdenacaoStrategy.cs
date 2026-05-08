using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services.Catalogo;

public interface ICatalogoOrdenacaoStrategy
{
    CatalogoOrdenacao Ordenacao { get; }

    IEnumerable<Filme> Ordenar(IEnumerable<Filme> filmes, bool descendente);
}
