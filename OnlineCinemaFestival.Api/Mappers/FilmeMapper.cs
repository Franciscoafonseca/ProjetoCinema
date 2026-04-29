namespace OnlineCinemaFestival.Api.Mappers;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

// Para mapear os dados do filme para o DTO que será enviado para o cliente
public static class FilmeMapper
{
    // Mapear um filme para o DTO de leitura
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
    };

    public static Filme MapFromTmdbDto(FilmeTmdbDto f) => new Filme
    {
        TmdbId = f.TmdbId,
        Titulo = f.Titulo,
        Sinopse = f.Sinopse,
        DataLancamento = f.DataLancamento,
        Genero = f.Genero,
        Classificacao = f.Classificacao,
        CapaUrl = f.CapaUrl
    };
}