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
    private readonly CatalogoOrdenacaoStrategyFactory _ordenacaoFactory;

    public CatalogoService(
        IFilmeRepository filmeRepository,
        IFestivalRepository festivalRepository,
        IFestivalFilmeRepository festivalFilmeRepository,
        CatalogoOrdenacaoStrategyFactory ordenacaoFactory
    )
    {
        _filmeRepository = filmeRepository;
        _festivalRepository = festivalRepository;
        _festivalFilmeRepository = festivalFilmeRepository;
        _ordenacaoFactory = ordenacaoFactory;
    }

    public async Task<IEnumerable<FilmeReadDto>> GetCatalogoAsync(CatalogoQueryDto query)
    {
        IEnumerable<Filme> filmes;

        if (query.FestivalId.HasValue)
        {
            filmes = await GetFilmesDoFestivalAsync(query.FestivalId.Value);
        }
        else
        {
            filmes = await _filmeRepository.GetAllAsync();
        }

        filmes = AplicarFiltros(filmes, query);
        filmes = AplicarOrdenacao(filmes, query);

        return filmes.Select(FilmeMapper.MapToReadDto);
    }

    public async Task<IEnumerable<FilmeReadDto>> GetFilmesByFestivalAsync(
        int festivalId,
        CatalogoQueryDto query
    )
    {
        query.FestivalId = festivalId;

        return await GetCatalogoAsync(query);
    }

    public async Task<FilmeReadDto?> GetFilmeDetalhesAsync(int filmeId)
    {
        var filme = await _filmeRepository.GetByIdAsync(filmeId);

        if (filme == null)
            return null;

        return FilmeMapper.MapToReadDto(filme);
    }

    private async Task<IEnumerable<Filme>> GetFilmesDoFestivalAsync(int festivalId)
    {
        var festival = await _festivalRepository.GetByIdAsync(festivalId);

        if (festival == null)
            throw new KeyNotFoundException("Festival não encontrado.");

        return await _festivalFilmeRepository.GetFilmesByFestivalIdAsync(festivalId);
    }

    private static IEnumerable<Filme> AplicarFiltros(
        IEnumerable<Filme> filmes,
        CatalogoQueryDto query
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

        if (!string.IsNullOrWhiteSpace(query.Genero))
        {
            var genero = query.Genero.Trim();

            filmes = filmes.Where(f =>
                !string.IsNullOrWhiteSpace(f.Genero)
                && f.Genero.Contains(genero, StringComparison.OrdinalIgnoreCase)
            );
        }

        return filmes;
    }

    private IEnumerable<Filme> AplicarOrdenacao(IEnumerable<Filme> filmes, CatalogoQueryDto query)
    {
        var strategy = _ordenacaoFactory.GetStrategy(query.OrdenarPor);

        return strategy.Ordenar(filmes, query.Descendente);
    }
}
