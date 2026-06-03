namespace OnlineCinemaFestival.Client.Services;

public class ImagemUrlService
{
    private readonly Uri _apiBaseUri;

    public ImagemUrlService(IConfiguration configuration)
    {
        var apiBaseUrl =
            configuration["Api:BaseUrl"]
            ?? throw new InvalidOperationException("Api:BaseUrl nao configurado.");

        _apiBaseUri = new Uri(apiBaseUrl, UriKind.Absolute);
    }

    public string Resolver(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return string.Empty;

        var valor = url.Trim();

        if (Uri.TryCreate(valor, UriKind.Absolute, out _))
            return valor;

        if (valor.StartsWith("//", StringComparison.Ordinal))
            return $"{_apiBaseUri.Scheme}:{valor}";

        return new Uri(_apiBaseUri, valor.TrimStart('/')).ToString();
    }
}
