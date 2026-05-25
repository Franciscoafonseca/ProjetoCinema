using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Tests.Upload;

/// <summary>
/// Testa a validação de extensão, tamanho e assinatura de ficheiro
/// do <see cref="ImagemUploadService"/> sem aceder ao sistema de ficheiros real.
/// As exceções são lançadas em <c>ValidarAsync</c>, antes de qualquer IO.
/// </summary>
public class ImagemValidacaoTests
{
    private static readonly byte[] JpegBytes = [0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x01];
    private static readonly byte[] PngBytes = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00];
    private static readonly byte[] TextBytes = "Conteudo de texto simples"u8.ToArray();

    // ── Extensão ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task ExtensaoInvalida_Exe_Rejeita()
    {
        var service = CriarServico();
        var ficheiro = new FakeFormFile("foto.exe", "application/octet-stream", JpegBytes);

        var ex = await Assert.ThrowsAsync<ArgumentException>(
            () => service.GuardarAsync(ficheiro, "perfis")
        );
        Assert.Contains("Formato invalido", ex.Message);
    }

    [Theory]
    [InlineData("foto.gif")]
    [InlineData("foto.bmp")]
    [InlineData("foto.tiff")]
    [InlineData("foto.svg")]
    public async Task ExtensaoNaoSuportada_Rejeita(string nomeFile)
    {
        var service = CriarServico();
        var ficheiro = new FakeFormFile(nomeFile, "image/gif", JpegBytes);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.GuardarAsync(ficheiro, "perfis")
        );
    }

    // ── Tamanho ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task FicheiroVazio_Rejeita()
    {
        var service = CriarServico();
        var ficheiro = new FakeFormFile("foto.jpg", "image/jpeg", []);

        var ex = await Assert.ThrowsAsync<ArgumentException>(
            () => service.GuardarAsync(ficheiro, "perfis")
        );
        Assert.Contains("obrigatorio", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task TamanhoExcedido_AcimaDosDoisMB_Rejeita()
    {
        var service = CriarServico();
        // 2 MB + 1 byte
        var conteudo = new byte[2 * 1024 * 1024 + 1];
        Array.Fill(conteudo, (byte)0xFF);
        var ficheiro = new FakeFormFile("foto.jpg", "image/jpeg", conteudo);

        var ex = await Assert.ThrowsAsync<ArgumentException>(
            () => service.GuardarAsync(ficheiro, "perfis")
        );
        Assert.Contains("2MB", ex.Message);
    }

    // ── Assinatura (magic bytes) ──────────────────────────────────────────────

    [Fact]
    public async Task ExtensaoJpg_MagicBytesErrados_Rejeita()
    {
        var service = CriarServico();
        // extensão .jpg mas conteúdo de texto
        var ficheiro = new FakeFormFile("foto.jpg", "image/jpeg", TextBytes);

        var ex = await Assert.ThrowsAsync<ArgumentException>(
            () => service.GuardarAsync(ficheiro, "perfis")
        );
        Assert.Contains("conteudo", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ExtensaoPng_MagicBytesErrados_Rejeita()
    {
        var service = CriarServico();
        var ficheiro = new FakeFormFile("foto.png", "image/png", JpegBytes);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.GuardarAsync(ficheiro, "perfis")
        );
    }

    [Fact]
    public async Task ExtensaoJpg_MagicBytesCorretos_PassaValidacao()
    {
        var service = CriarServico();
        // A validação passa; o erro posterior é de IO (diretório inexistente) — não de validação
        var ficheiro = new FakeFormFile("foto.jpg", "image/jpeg", JpegBytes);

        // Não deve lançar ArgumentException (validação OK); pode lançar IOException de IO
        var ex = await Record.ExceptionAsync(() => service.GuardarAsync(ficheiro, "perfis"));
        Assert.IsNotType<ArgumentException>(ex);
    }

    [Fact]
    public async Task ContentTypeImageJpg_MagicBytesCorretos_PassaValidacao()
    {
        var service = CriarServico();
        var ficheiro = new FakeFormFile("foto.jpg", "image/jpg", JpegBytes);

        var ex = await Record.ExceptionAsync(() => service.GuardarAsync(ficheiro, "perfis"));
        Assert.IsNotType<ArgumentException>(ex);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static ImagemUploadService CriarServico() =>
        new(new FakeWebHostEnvironment());

    // ── Fakes internos ────────────────────────────────────────────────────────

    private sealed class FakeFormFile : IFormFile
    {
        private readonly byte[] _content;

        public FakeFormFile(string fileName, string contentType, byte[] content)
        {
            FileName = fileName;
            ContentType = contentType;
            _content = content;
        }

        public string ContentType { get; }
        public string ContentDisposition => $"form-data; name=\"file\"; filename=\"{FileName}\"";
        public IHeaderDictionary Headers => new HeaderDictionary();
        public long Length => _content.Length;
        public string Name => "file";
        public string FileName { get; }

        public Stream OpenReadStream() => new MemoryStream(_content);

        public void CopyTo(Stream target) => new MemoryStream(_content).CopyTo(target);

        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default) =>
            new MemoryStream(_content).CopyToAsync(target, cancellationToken);
    }

    private sealed class FakeWebHostEnvironment : IWebHostEnvironment
    {
        // GuardarAsync usa estas propriedades só após ValidarAsync passar
        public string WebRootPath { get; set; } = Path.GetTempPath();
        public string ContentRootPath { get; set; } = Path.GetTempPath();
        public string ApplicationName { get; set; } = "Test";
        public string EnvironmentName { get; set; } = "Development";
        public IFileProvider WebRootFileProvider { get; set; } = null!;
        public IFileProvider ContentRootFileProvider { get; set; } = null!;
    }
}
