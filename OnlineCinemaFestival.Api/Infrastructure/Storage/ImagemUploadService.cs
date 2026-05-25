namespace OnlineCinemaFestival.Api.Services;

public class ImagemUploadService : IImagemUploadService
{
    private const long MaxBytes = 2 * 1024 * 1024;
    private readonly IWebHostEnvironment _environment;

    private static readonly Dictionary<string, string[]> ContentTypesValidos = new(
        StringComparer.OrdinalIgnoreCase
    )
    {
        [".jpg"] = ["image/jpeg", "image/jpg", "image/pjpeg"],
        [".jpeg"] = ["image/jpeg", "image/jpg", "image/pjpeg"],
        [".png"] = ["image/png", "image/x-png"],
        [".webp"] = ["image/webp"],
    };

    public ImagemUploadService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> GuardarAsync(IFormFile ficheiro, string subpasta)
    {
        await ValidarAsync(ficheiro);

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

    private static async Task ValidarAsync(IFormFile ficheiro)
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
            !ContentTypesValidos.TryGetValue(extensao, out var contentTypesEsperados)
            || !ContentTypeValido(ficheiro.ContentType, contentTypesEsperados)
        )
            throw new ArgumentException("Formato invalido. Usa jpg, jpeg, png ou webp.");

        if (!await AssinaturaValidaAsync(ficheiro, extensao))
            throw new ArgumentException("O conteudo do ficheiro nao corresponde a uma imagem valida.");
    }

    private static bool ContentTypeValido(string? contentType, string[] contentTypesEsperados)
    {
        if (string.IsNullOrWhiteSpace(contentType))
            return true;

        if (contentType.Equals("application/octet-stream", StringComparison.OrdinalIgnoreCase))
            return true;

        return contentTypesEsperados.Any(esperado =>
            contentType.Equals(esperado, StringComparison.OrdinalIgnoreCase)
        );
    }

    private static async Task<bool> AssinaturaValidaAsync(IFormFile ficheiro, string extensao)
    {
        var buffer = new byte[12];

        await using var stream = ficheiro.OpenReadStream();
        var lidos = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length));

        return extensao switch
        {
            ".jpg" or ".jpeg" => lidos >= 3
                && buffer[0] == 0xFF
                && buffer[1] == 0xD8
                && buffer[2] == 0xFF,
            ".png" => lidos >= 8
                && buffer[0] == 0x89
                && buffer[1] == 0x50
                && buffer[2] == 0x4E
                && buffer[3] == 0x47
                && buffer[4] == 0x0D
                && buffer[5] == 0x0A
                && buffer[6] == 0x1A
                && buffer[7] == 0x0A,
            ".webp" => lidos >= 12
                && buffer[0] == 0x52
                && buffer[1] == 0x49
                && buffer[2] == 0x46
                && buffer[3] == 0x46
                && buffer[8] == 0x57
                && buffer[9] == 0x45
                && buffer[10] == 0x42
                && buffer[11] == 0x50,
            _ => false,
        };
    }
}
