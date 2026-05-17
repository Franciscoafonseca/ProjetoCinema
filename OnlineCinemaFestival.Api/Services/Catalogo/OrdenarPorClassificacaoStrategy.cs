using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services.Catalogo;

public class OrdenarPorClassificacaoStrategy : ICatalogoOrdenacaoStrategy
{
    public CatalogoOrdenacao Ordenacao => CatalogoOrdenacao.Classificacao;

    public IEnumerable<Filme> Ordenar(IEnumerable<Filme> filmes, bool descendente)
    {
        return descendente
            ? filmes.OrderByDescending(f => f.AvaliacaoTmdb ?? 0)
            : filmes.OrderBy(f => f.AvaliacaoTmdb ?? 0);
    }
}
