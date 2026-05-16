namespace OnlineCinemaFestival.Api.Mappers;

using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

// Para mapear os dados do filme para o DTO que será enviado para o cliente
public static class FilmeMapper
{
    // Mapear um filme para o DTO de leitura
    // BD -> API
    public static FilmeReadDTO MapToReadDTO(Filme f) =>
        new FilmeReadDTO
        {
            Id = f.Id,
            TmdbId = f.TmdbId,
            Titulo = f.Titulo,
            TituloOriginal = f.TituloOriginal,
            Sinopse = f.Sinopse,
            DataLancamento = f.DataLancamento,
            DuracaoMinutos = f.DuracaoMinutos,
            Genero = f.Genero,
            Generos = f.FilmeGeneros.Select(fg => fg.Genero.Name).OrderBy(g => g).ToList(),
            Classificacao = f.Classificacao,
            AvaliacaoTmdb = f.AvaliacaoTmdb,
            CapaUrl = f.CapaUrl,
            TrailerUrl = f.TrailerUrl,
            VideoUrl = f.VideoUrl,
            Popularidade = f.Popularidade,
            Realizador = f.Realizador,
            Atores = SepararLista(f.AtoresPrincipais),
            ReviewsTmdb = DesserializarReviews(f.TmdbReviewsJson),
            Premios = f.Premios,
            ReviewsAplicacao = f.Avaliacoes.Select(a => new AvaliacaoDTO
            {
                Id = a.Id,
                FilmeId = a.FilmeId,
                TituloFilme = f.Titulo,
                UsuarioId = a.UsuarioId,
                NomeUsuario = a.Usuario?.Name ?? string.Empty,
                Pontuacao = a.Pontuacao,
                Texto = a.Texto,
                Data = a.Data,
            }).OrderByDescending(a => a.Data).ToList(),
            Festivais = f.FestivalFilmes.Select(ff => new FestivalResumoDTO
            {
                Id = ff.FestivalId,
                Name = ff.Festival?.Name ?? string.Empty,
                StartDate = ff.Festival?.StartDate ?? default,
                EndDate = ff.Festival?.EndDate ?? default,
            }).ToList(),
            Sessoes = f.SessoesDoFilme.Select(sf => new FilmeSessaoReadDTO
            {
                Id = sf.SessaoId,
                Titulo = sf.Sessao?.Festival?.Name ?? f.Titulo,
                Ordem = sf.Ordem,
                HoraInicio = sf.HoraInicio ?? sf.Sessao?.Inicio,
                HoraFim = sf.HoraFim ?? sf.Sessao?.Fim,
            }).OrderBy(s => s.HoraInicio).ToList(),
            AcessosDisponiveis = f.Acessos.Where(a => a.IsAtivo).Select(AcessoMapper.MapToReadDTO).ToList(),
        };

    // TMDB -> BD
    public static Filme MapFromTmdbDTO(TmdbFilmeDTO f) =>
        new Filme
        {
            TmdbId = f.TmdbId,
            Titulo = f.Titulo,
            TituloOriginal = f.TituloOriginal,
            Sinopse = f.Sinopse,
            DataLancamento = f.DataLancamento,
            DuracaoMinutos = f.DuracaoMinutos,
            Genero = f.Genero,
            AvaliacaoTmdb = f.AvaliacaoTmdb,
            Classificacao = f.Classificacao,
            CapaUrl = f.CapaUrl,
            TrailerUrl = f.TrailerUrl,
            Realizador = f.Realizador,
            AtoresPrincipais = string.Join(", ", f.Atores),
            TmdbReviewsJson = System.Text.Json.JsonSerializer.Serialize(f.Reviews),
        };

    // TMDB -> API (para resultados de busca, que são mais leves que os detalhes)
    public static FilmeReadDTO MapToReadDTOFromTmdb(TmdbFilmeDTO f) =>
        new FilmeReadDTO
        {
            TmdbId = f.TmdbId,
            Titulo = f.Titulo,
            TituloOriginal = f.TituloOriginal,
            Sinopse = f.Sinopse,
            DataLancamento = f.DataLancamento,
            Genero = f.Genero,
            Generos = f.Generos,
            Classificacao = f.Classificacao,
            AvaliacaoTmdb = f.AvaliacaoTmdb,
            CapaUrl = f.CapaUrl,
            TrailerUrl = f.TrailerUrl,
            DuracaoMinutos = f.DuracaoMinutos,
        };

    // Genero fica vazio na pesquisa; so e preenchido no detalhe do filme.
    public static TmdbFilmeDTO MapFromTmdbResult(TmdbFilmeResult r) =>
        new TmdbFilmeDTO
        {
            TmdbId = r.TmdbId,
            Titulo = r.Titulo,
            Sinopse = r.Sinopse,
            DataLancamento = DateTime.TryParse(r.DataLancamento, out var date)
                ? date
                : DateTime.MinValue,
            Classificacao = r.Classificacao?.ToString("0.0"),
            AvaliacaoTmdb = r.Classificacao,
            CapaUrl = string.IsNullOrWhiteSpace(r.CapaUrl)
                ? ""
                : $"https://image.tmdb.org/t/p/w500{r.CapaUrl}",
            Genero = "", // genero_ids precisa de tabela/lookup
        };

    private static List<string> SepararLista(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor)
            ? new List<string>()
            : valor.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
    }

    private static List<TmdbReviewDTO> DesserializarReviews(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new List<TmdbReviewDTO>();

        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<TmdbReviewDTO>>(json)
                ?? new List<TmdbReviewDTO>();
        }
        catch
        {
            return new List<TmdbReviewDTO>();
        }
    }
}
