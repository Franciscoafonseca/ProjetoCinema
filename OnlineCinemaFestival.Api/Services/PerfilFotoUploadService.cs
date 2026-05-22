namespace OnlineCinemaFestival.Api.Services;

public class PerfilFotoUploadService : IPerfilFotoUploadService
{
    private const long MaxBytes = 2 * 1024 * 1024;
    private readonly IWebHostEnvironment _environment;

    private static readonly Dictionary<string, string> ContentTypesValidos = new(
        StringComparer.OrdinalIgnoreCase
    )
    {
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".png"] = "image/png",
        [".webp"] = "image/webp",
    };

    public PerfilFotoUploadService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> GuardarAsync(IFormFile ficheiro)
    {
        Validar(ficheiro);

        var extensao = Path.GetExtension(ficheiro.FileName).ToLowerInvariant();
        var webRoot = string.IsNullOrWhiteSpace(_environment.WebRootPath)
            ? Path.Combine(_environment.ContentRootPath, "wwwroot")
            : _environment.WebRootPath;

        var pasta = Path.Combine(webRoot, "uploads", "perfis");
        Directory.CreateDirectory(pasta);

        var nomeFicheiro = $"{Guid.NewGuid():N}{extensao}";
        var caminhoCompleto = Path.Combine(pasta, nomeFicheiro);

        await using (var stream = File.Create(caminhoCompleto))
        {
            await ficheiro.CopyToAsync(stream);
        }

        return $"/uploads/perfis/{nomeFicheiro}";
    }

    private static void Validar(IFormFile ficheiro)
    {
        if (ficheiro == null || ficheiro.Length == 0)
            throw new ArgumentException("Ficheiro obrigatorio.");

        if (ficheiro.Length > MaxBytes)
            throw new ArgumentException("A foto nao pode exceder 2MB.");

        var extensao = Path.GetExtension(ficheiro.FileName).ToLowerInvariant();

        if (
            !ContentTypesValidos.TryGetValue(extensao, out var contentTypeEsperado)
            || !string.Equals(
                ficheiro.ContentType,
                contentTypeEsperado,
                StringComparison.OrdinalIgnoreCase
            )
        )
            throw new ArgumentException("Formato invalido. Usa jpg, png ou webp.");
    }
}
