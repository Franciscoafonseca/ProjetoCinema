using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services.Catalogo;

public class OrdenarPorVisualizacoesStrategy : ICatalogoOrdenacaoStrategy
{
    public CatalogoOrdenacao Ordenacao => CatalogoOrdenacao.Visualizacoes;

    public IEnumerable<Filme> Ordenar(IEnumerable<Filme> filmes, bool descendente)
    {
        return descendente
            ? filmes.OrderByDescending(f => f.Visualizacoes.Count).ThenBy(f => f.Titulo)
            : filmes.OrderBy(f => f.Visualizacoes.Count).ThenBy(f => f.Titulo);
    }
}
