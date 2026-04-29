namespace OnlineCinemaFestival.Api.Mappers;

using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

// Para mapear os dados do filme para o DTO que será enviado para o cliente
public static class FilmeMapper
{
    // Mapear um filme para o DTO de leitura
    // BD -> API
    public static FilmeReadDto MapToReadDto(Filme f) => new FilmeReadDto
    {
        Id = f.Id,
        TmdbId = f.TmdbId,
        Titulo = f.Titulo,
        Sinopse = f.Sinopse,
        DataLancamento = f.DataLancamento,
        Genero = f.Genero,
        Classificacao = f.Classificacao,
        CapaUrl = f.CapaUrl,
        Popularidade = f.Popularidade
    };

    // TMDB -> BD
    public static Filme MapFromTmdbDto(TmdbFilmeDto f) => new Filme
    {
        TmdbId = f.TmdbId,
        Titulo = f.Titulo,
        Sinopse = f.Sinopse,
        DataLancamento = f.DataLancamento,
        Genero = f.Genero,
        Classificacao = f.Classificacao,
        CapaUrl = f.CapaUrl
    };

    // TMDB -> API (para resultados de busca, que são mais leves que os detalhes)
    public static FilmeReadDto MapToReadDtoFromTmdb(TmdbFilmeDto f) => new FilmeReadDto
    {
        TmdbId = f.TmdbId,
        Titulo = f.Titulo,
        Sinopse = f.Sinopse,
        DataLancamento = f.DataLancamento,
        Genero = f.Genero,
        Classificacao = f.Classificacao,
        CapaUrl = f.CapaUrl
    };

    // Genero fica vazio na pesquisa; so e preenchido no detalhe do filme.
    public static TmdbFilmeDto MapFromTmdbResult(TmdbFilmeResult r) => new TmdbFilmeDto
    {
        TmdbId = r.TmdbId,
        Titulo = r.Titulo,
        Sinopse = r.Sinopse,
        DataLancamento = DateTime.TryParse(r.DataLancamento, out var date)
                                ? date
                                : DateTime.MinValue,
        Classificacao = r.Classificacao?.ToString("0.0"),
        CapaUrl = string.IsNullOrWhiteSpace(r.CapaUrl)
            ? ""
            : $"https://image.tmdb.org/t/p/w500{r.CapaUrl}",
        Genero = "" // genero_ids precisa de tabela/lookup
    };
}