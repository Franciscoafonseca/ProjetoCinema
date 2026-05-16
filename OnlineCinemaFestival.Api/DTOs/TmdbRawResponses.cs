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

    [JsonPropertyName("original_title")]
    public string? TituloOriginal { get; set; }

    [JsonPropertyName("runtime")]
    public int? DuracaoMinutos { get; set; }
}

public class TmdbGenre
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
}

public class TmdbGenreResponse
{
    [JsonPropertyName("genres")]
    public List<TmdbGenre> Genres { get; set; } = new();
}

public class TmdbCreditsResponse
{
    [JsonPropertyName("cast")]
    public List<TmdbCastMember> Cast { get; set; } = new();

    [JsonPropertyName("crew")]
    public List<TmdbCrewMember> Crew { get; set; } = new();
}

public class TmdbCastMember
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("order")]
    public int Order { get; set; }
}

public class TmdbCrewMember
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("job")]
    public string Job { get; set; } = string.Empty;
}

public class TmdbReviewsResponse
{
    [JsonPropertyName("results")]
    public List<TmdbReviewResult> Results { get; set; } = new();
}

public class TmdbReviewResult
{
    [JsonPropertyName("author")]
    public string Author { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("author_details")]
    public TmdbAuthorDetails? AuthorDetails { get; set; }
}

public class TmdbAuthorDetails
{
    [JsonPropertyName("rating")]
    public double? Rating { get; set; }
}
