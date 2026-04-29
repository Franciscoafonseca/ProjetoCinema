using System.Text.Json.Serialization;

namespace OnlineCinemaFestival.Api.DTOs;

public class TmdbSearchResponse
{
    [JsonPropertyName("results")]
    public List<TmdbFilmeResult> Results { get; set; } = new();
}

public class TmdbFilmeResult
{
    [JsonPropertyName("id")]
    public int TmdbId { get; set; }

    [JsonPropertyName("title")]
    public string Titulo { get; set; } = "";

    [JsonPropertyName("overview")]
    public string? Sinopse { get; set; }

    [JsonPropertyName("release_date")]
    public string? DataLancamento { get; set; }

    [JsonPropertyName("genre_ids")]
    public List<int> GenreIds { get; set; } = new();

    [JsonPropertyName("vote_average")]
    public double? Classificacao { get; set; }

    [JsonPropertyName("poster_path")]
    public string? CapaUrl { get; set; }
}

// Para pesquisar detalhes sobre o filme
public class TmdbMovieDetails : TmdbFilmeResult
{
    [JsonPropertyName("genres")]
    public List<TmdbGenre> Genres { get; set; } = new();
}

public class TmdbGenre
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
}