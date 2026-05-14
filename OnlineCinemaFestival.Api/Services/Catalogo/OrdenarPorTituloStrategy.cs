using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services.Catalogo;

public class OrdenarPorTituloStrategy : ICatalogoOrdenacaoStrategy
{
    public CatalogoOrdenacao Ordenacao => CatalogoOrdenacao.Titulo;

    public IEnumerable<Filme> Ordenar(IEnumerable<Filme> filmes, bool descendente)
    {
        return descendente
            ? filmes.OrderByDescending(f => f.Titulo)
            : filmes.OrderBy(f => f.Titulo);
    }
}
