namespace OnlineCinemaFestival.Api.Configuracao;

public sealed class YouTubeOptions
{
    public const string SectionName = "YouTube";

    public string ApiKey { get; init; } = string.Empty;

    public string BaseUrl { get; init; } = "https://www.googleapis.com/youtube/v3/";
}
