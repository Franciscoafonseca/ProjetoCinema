using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Mappers;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Services.Catalogo;

namespace OnlineCinemaFestival.Api.Services;

public class CatalogoService : ICatalogoService
{
    private readonly IFilmeRepository _filmeRepository;
    private readonly IFestivalRepository _festivalRepository;
    private readonly IFestivalFilmeRepository _festivalFilmeRepository;
    private readonly ICatalogoOrdenacaoStrategyFactory _ordenacaoFactory;

    public CatalogoService(
        IFilmeRepository filmeRepository,
        IFestivalRepository festivalRepository,
        IFestivalFilmeRepository festivalFilmeRepository,
        ICatalogoOrdenacaoStrategyFactory ordenacaoFactory
    )
    {
        _filmeRepository = filmeRepository;
        _festivalRepository = festivalRepository;
        _festivalFilmeRepository = festivalFilmeRepository;
        _ordenacaoFactory = ordenacaoFactory;
    }

    public async Task<IEnumerable<FilmeReadDTO>> ObterCatalogoAsync(CatalogoQueryDTO query)
    {
        IEnumerable<Filme> filmes;

        if (query.FestivalId.HasValue)
        {
            filmes = await ObterFilmesDoFestivalAsync(query.FestivalId.Value);
        }
        else
        {
            filmes = await _filmeRepository.ObterTodosAsync();
        }

        var generosSelecionados = ObterGenerosSelecionados(query.Genero);

        filmes = AplicarFiltros(filmes, query, generosSelecionados);
        filmes = AplicarOrdenacao(filmes, query, generosSelecionados);

        return filmes.Select(FilmeMapper.MapToReadDTO);
    }

    public async Task<IEnumerable<FilmeReadDTO>> ObterFilmesPorFestivalAsync(
        int festivalId,
        CatalogoQueryDTO query
    )
    {
        query.FestivalId = festivalId;

        return await ObterCatalogoAsync(query);
    }

    public async Task<FilmeReadDTO?> ObterDetalhesFilmeAsync(int filmeId)
    {
        var filme = await _filmeRepository.ObterDetalhePorIdAsync(filmeId);

        if (filme == null)
            return null;

        return FilmeMapper.MapToReadDTO(filme);
    }

    private async Task<IEnumerable<Filme>> ObterFilmesDoFestivalAsync(int festivalId)
    {
        var festival = await _festivalRepository.ObterPorIdAsync(festivalId);

        if (festival == null)
            throw new KeyNotFoundException("Festival não encontrado.");

        return await _festivalFilmeRepository.ObterFilmesPorFestivalIdAsync(festivalId);
    }

    private static IEnumerable<Filme> AplicarFiltros(
        IEnumerable<Filme> filmes,
        CatalogoQueryDTO query,
        IReadOnlyCollection<string> generosSelecionados
    )
    {
        if (!string.IsNullOrWhiteSpace(query.Pesquisa))
        {
            var pesquisa = query.Pesquisa.Trim();

            filmes = filmes.Where(f =>
                (
                    !string.IsNullOrWhiteSpace(f.Titulo)
                    && f.Titulo.Contains(pesquisa, StringComparison.OrdinalIgnoreCase)
                )
                || (
                    !string.IsNullOrWhiteSpace(f.Sinopse)
                    && f.Sinopse.Contains(pesquisa, StringComparison.OrdinalIgnoreCase)
                )
            );
        }

        if (generosSelecionados.Count > 0)
        {
            filmes = filmes.Where(f =>
                ContarGenerosEmComum(f, generosSelecionados) > 0
            );
        }

        return filmes;
    }

    private IEnumerable<Filme> AplicarOrdenacao(
        IEnumerable<Filme> filmes,
        CatalogoQueryDTO query,
        IReadOnlyCollection<string> generosSelecionados
    )
    {
        var strategy = _ordenacaoFactory.GetStrategy(query.OrdenarPor);
        var ordenados = strategy.Ordenar(filmes, query.Descendente);

        if (generosSelecionados.Count == 0)
            return ordenados;

        return ordenados
            .Select((filme, indice) => new
            {
                Filme = filme,
                Indice = indice,
                Correspondencias = ContarGenerosEmComum(filme, generosSelecionados),
            })
            .OrderByDescending(item => item.Correspondencias)
            .ThenBy(item => item.Indice)
            .Select(item => item.Filme);
    }

    private static IReadOnlyCollection<string> ObterGenerosSelecionados(string? genero)
    {
        if (string.IsNullOrWhiteSpace(genero))
            return Array.Empty<string>();

        return genero
            .Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(g => !string.IsNullOrWhiteSpace(g))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static int ContarGenerosEmComum(
        Filme filme,
        IReadOnlyCollection<string> generosSelecionados
    )
    {
        if (generosSelecionados.Count == 0)
            return 0;

        var generosFilme = GenerosDoFilme(filme).ToHashSet(StringComparer.OrdinalIgnoreCase);
        return generosSelecionados.Count(generosFilme.Contains);
    }

    private static IEnumerable<string> GenerosDoFilme(Filme filme)
    {
        var porTabela = filme.FilmeGeneros
            .Select(fg => fg.Genero?.Name)
            .Where(g => !string.IsNullOrWhiteSpace(g))
            .Select(g => g!);

        var porTexto = string.IsNullOrWhiteSpace(filme.Genero)
            ? Array.Empty<string>()
            : filme.Genero.Split(
                ',',
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
            );

        return porTabela.Concat(porTexto).Where(g => !string.IsNullOrWhiteSpace(g));
    }
}
