namespace OnlineCinemaFestival.Api.Configuracao;

public sealed class CorsOptions
{
    public const string SectionName = "Cors";

    public string[] AllowedOrigins { get; init; } = [];
}
