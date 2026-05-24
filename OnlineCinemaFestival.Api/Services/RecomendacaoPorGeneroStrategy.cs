using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public class RecomendacaoPorGeneroStrategy : IRecomendacaoStrategy
{
    public IEnumerable<ResultadoRecomendacao> Recomendar(
        IEnumerable<Filme> filmes,
        Utilizador? utilizador
    )
    {
        var generosPreferidos = utilizador
            ?.GenerosFavoritos.Select(g => g.Genero.Name)
            .Where(g => !string.IsNullOrWhiteSpace(g))
            .ToHashSet(StringComparer.OrdinalIgnoreCase) ?? new HashSet<string>();

        if (generosPreferidos.Count == 0)
            yield break;

        foreach (var filme in filmes)
        {
            var correspondencias = GenerosDoFilme(filme).Count(generosPreferidos.Contains);
            if (correspondencias == 0)
                continue;

            yield return new ResultadoRecomendacao(
                filme,
                correspondencias * 10,
                correspondencias == 1
                    ? "Genero preferido"
                    : $"{correspondencias} generos preferidos"
            );
        }
    }

    private static IEnumerable<string> GenerosDoFilme(Filme filme)
    {
        var porTabela = filme.FilmeGeneros.Select(fg => fg.Genero.Name);
        var porTexto = string.IsNullOrWhiteSpace(filme.Genero)
            ? Array.Empty<string>()
            : filme.Genero.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return porTabela.Concat(porTexto).Where(g => !string.IsNullOrWhiteSpace(g));
    }
}
