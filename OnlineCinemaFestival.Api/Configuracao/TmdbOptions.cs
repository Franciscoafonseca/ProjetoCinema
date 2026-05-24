namespace OnlineCinemaFestival.Api.Configuracao;

public sealed class TmdbOptions
{
    public const string SectionName = "Tmdb";

    public string Token { get; init; } = string.Empty;

    public string BaseUrl { get; init; } = "https://api.themoviedb.org/3/";
}
