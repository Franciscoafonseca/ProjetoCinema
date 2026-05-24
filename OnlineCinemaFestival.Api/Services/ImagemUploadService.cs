namespace OnlineCinemaFestival.Api.Services;

public class ImagemUploadService : IImagemUploadService
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

    public ImagemUploadService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> GuardarAsync(IFormFile ficheiro, string subpasta)
    {
        Validar(ficheiro);

        var pastaSegura = subpasta.Trim().Replace('\\', '/').Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (pastaSegura.Length == 0 || pastaSegura.Any(p => p is "." or ".."))
            throw new ArgumentException("Pasta de upload invalida.");

        var extensao = Path.GetExtension(ficheiro.FileName).ToLowerInvariant();
        var webRoot = string.IsNullOrWhiteSpace(_environment.WebRootPath)
            ? Path.Combine(_environment.ContentRootPath, "wwwroot")
            : _environment.WebRootPath;

        var pasta = Path.Combine(new[] { webRoot, "uploads" }.Concat(pastaSegura).ToArray());
        Directory.CreateDirectory(pasta);

        var nomeFicheiro = $"{Guid.NewGuid():N}{extensao}";
        var caminhoCompleto = Path.Combine(pasta, nomeFicheiro);

        await using (var stream = File.Create(caminhoCompleto))
        {
            await ficheiro.CopyToAsync(stream);
        }

        return $"/uploads/{string.Join('/', pastaSegura)}/{nomeFicheiro}";
    }

    private static void Validar(IFormFile ficheiro)
    {
        if (ficheiro == null || ficheiro.Length == 0)
            throw new ArgumentException("Ficheiro obrigatorio.");

        if (ficheiro.Length > MaxBytes)
            throw new ArgumentException("A imagem nao pode exceder 2MB.");

        var nome = Path.GetFileName(ficheiro.FileName);
        if (!string.Equals(nome, ficheiro.FileName, StringComparison.Ordinal))
            throw new ArgumentException("Nome de ficheiro invalido.");

        var extensao = Path.GetExtension(nome).ToLowerInvariant();

        if (
            !ContentTypesValidos.TryGetValue(extensao, out var contentTypeEsperado)
            || !string.Equals(
                ficheiro.ContentType,
                contentTypeEsperado,
                StringComparison.OrdinalIgnoreCase
            )
        )
            throw new ArgumentException("Formato invalido. Usa jpg, jpeg, png ou webp.");
    }
}
