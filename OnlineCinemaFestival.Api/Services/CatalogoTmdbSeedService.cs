namespace OnlineCinemaFestival.Api.Services;

public class CatalogoTmdbSeedService
{
    private const int QuantidadeCatalogoInterno = 20;
    private readonly IConfiguration _configuration;
    private readonly IFilmeService _filmeService;
    private readonly ITmdbService _tmdbService;
    private readonly ILogger<CatalogoTmdbSeedService> _logger;

    public CatalogoTmdbSeedService(
        IConfiguration configuration,
        IFilmeService filmeService,
        ITmdbService tmdbService,
        ILogger<CatalogoTmdbSeedService> logger
    )
    {
        _configuration = configuration;
        _filmeService = filmeService;
        _tmdbService = tmdbService;
        _logger = logger;
    }

    public async Task GarantirCatalogoPopularAsync()
    {
        if (string.IsNullOrWhiteSpace(_configuration["Tmdb:Token"]))
            return;

        var existentes = (await _filmeService.ObterTodosFilmesAsync()).ToList();
        var tmdbIdsExistentes = existentes
            .Where(f =>
                f.TmdbId > 0
                && (
                    !string.IsNullOrWhiteSpace(f.VideoProvider)
                    || f.CapaUrl.Contains("image.tmdb.org", StringComparison.OrdinalIgnoreCase)
                )
            )
            .Select(f => f.TmdbId)
            .ToHashSet();

        if (tmdbIdsExistentes.Count >= QuantidadeCatalogoInterno)
            return;

        var populares = (await _tmdbService.ObterFilmesIniciaisAsync())
            .Where(f => f.TmdbId > 0)
            .OrderBy(_ => Guid.NewGuid())
            .Take(QuantidadeCatalogoInterno)
            .ToList();

        foreach (var filme in populares)
        {
            if (tmdbIdsExistentes.Contains(filme.TmdbId))
                continue;

            try
            {
                await _filmeService.ImportFilmeFromTmdbAsync(filme.TmdbId);
                tmdbIdsExistentes.Add(filme.TmdbId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Nao foi possivel importar o filme TMDB {TmdbId} durante o seed.",
                    filme.TmdbId
                );
            }

            if (tmdbIdsExistentes.Count >= QuantidadeCatalogoInterno)
                break;
        }
    }
}
