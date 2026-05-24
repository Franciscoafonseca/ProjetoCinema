using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Services.Catalogo;

public class OrdenarPorFestivalStrategy : ICatalogoOrdenacaoStrategy
{
    public CatalogoOrdenacao Ordenacao => CatalogoOrdenacao.Festival;

    public IEnumerable<Filme> Ordenar(IEnumerable<Filme> filmes, bool descendente)
    {
        return descendente
            ? filmes.OrderByDescending(NomeFestival).ThenBy(f => f.Titulo)
            : filmes.OrderBy(NomeFestival).ThenBy(f => f.Titulo);
    }

    private static string NomeFestival(Filme filme)
    {
        return filme.FestivalFilmes
                .Select(ff => ff.Festival?.Name)
                .Where(nome => !string.IsNullOrWhiteSpace(nome))
                .OrderBy(nome => nome)
                .FirstOrDefault()
            ?? string.Empty;
    }
}
