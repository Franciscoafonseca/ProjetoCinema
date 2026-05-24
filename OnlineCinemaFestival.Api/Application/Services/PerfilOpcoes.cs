namespace OnlineCinemaFestival.Api.Services;

public sealed record PaisOpcao(string Codigo, string Nome);

public static class PerfilOpcoes
{
    public static readonly IReadOnlyList<PaisOpcao> Paises =
    [
        new("PT", "Portugal"),
        new("BR", "Brasil"),
        new("AO", "Angola"),
        new("CV", "Cabo Verde"),
        new("MZ", "Mocambique"),
        new("ES", "Espanha"),
        new("FR", "Franca"),
        new("IT", "Italia"),
        new("DE", "Alemanha"),
        new("GB", "Reino Unido"),
        new("US", "Estados Unidos"),
    ];

    public static readonly IReadOnlyList<string> Localidades =
    [
        "Funchal",
        "Camara de Lobos",
        "Machico",
        "Santa Cruz",
        "Ribeira Brava",
        "Lisboa",
        "Porto",
        "Coimbra",
        "Braga",
        "Setubal",
        "Madrid",
        "Paris",
        "Roma",
        "Berlim",
        "Londres",
    ];

    public static PaisOpcao ObterPaisValido(string codigoOuNome)
    {
        if (string.IsNullOrWhiteSpace(codigoOuNome))
            throw new ArgumentException("O pais/nacionalidade e obrigatorio.");

        var normalizado = codigoOuNome.Trim();
        var pais = Paises.FirstOrDefault(p =>
            p.Codigo.Equals(normalizado, StringComparison.OrdinalIgnoreCase)
            || p.Nome.Equals(normalizado, StringComparison.OrdinalIgnoreCase)
        );

        if (pais is null)
            throw new ArgumentException("Seleciona um pais/nacionalidade valido.");

        return pais;
    }

    public static string ObterLocalidadeValida(string localidade)
    {
        if (string.IsNullOrWhiteSpace(localidade))
            throw new ArgumentException("A localidade e obrigatoria.");

        var normalizada = localidade.Trim();

        if (!Localidades.Any(l => l.Equals(normalizada, StringComparison.OrdinalIgnoreCase)))
            throw new ArgumentException("Seleciona uma localidade valida.");

        return Localidades.First(l => l.Equals(normalizada, StringComparison.OrdinalIgnoreCase));
    }

    public static string ObterBandeira(string countryCode)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
            return string.Empty;

        var codigo = countryCode.Trim().ToUpperInvariant();

        if (codigo.Length != 2 || codigo.Any(c => c < 'A' || c > 'Z'))
            return string.Empty;

        return string.Concat(codigo.Select(c => char.ConvertFromUtf32(0x1F1E6 + c - 'A')));
    }
}
