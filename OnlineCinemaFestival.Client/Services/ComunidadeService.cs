using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public class ComunidadeService
{
    private readonly HttpClient _http;

    public ComunidadeService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ComunidadeDTO>> ObterPublicasAsync()
    {
        var resposta = await _http.GetAsync("api/comunidades");

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await MensagemErroApi.ObterAsync(
                    resposta,
                    "Nao foi possivel carregar comunidades publicas."
                )
            );

        var comunidades = await resposta.Content.ReadFromJsonAsync<List<ComunidadeDTO>>() ?? new();
        foreach (var comunidade in comunidades)
            NormalizarFotosMembros(comunidade);
        return comunidades;
    }

    public async Task<List<ComunidadeDTO>> ObterMinhasAsync()
    {
        var resposta = await _http.GetAsync("api/comunidades/minhas");

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await MensagemErroApi.ObterAsync(
                    resposta,
                    "Nao foi possivel carregar as tuas comunidades."
                )
            );

        var comunidades = await resposta.Content.ReadFromJsonAsync<List<ComunidadeDTO>>() ?? new();
        foreach (var comunidade in comunidades)
            NormalizarFotosMembros(comunidade);
        return comunidades;
    }

    public async Task<ComunidadeDTO> ObterPorIdAsync(Guid id)
    {
        var resposta = await _http.GetAsync($"api/comunidades/{id}");

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await MensagemErroApi.ObterAsync(resposta, "Nao foi possivel abrir a comunidade.")
            );

        return NormalizarFotosMembros(await resposta.Content.ReadFromJsonAsync<ComunidadeDTO>())
            ?? throw new InvalidOperationException("Resposta invalida do servidor.");
    }

    public async Task<ComunidadeDTO> CriarAsync(ComunidadeCreateDTO dto)
    {
        var resposta = await _http.PostAsJsonAsync("api/comunidades", dto);

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await MensagemErroApi.ObterAsync(resposta, "Nao foi possivel criar a comunidade.")
            );

        return NormalizarFotosMembros(await resposta.Content.ReadFromJsonAsync<ComunidadeDTO>())
            ?? throw new InvalidOperationException("Resposta invalida do servidor.");
    }

    public async Task<ComunidadeDTO> EnviarImagemAsync(Guid id, IBrowserFile ficheiro)
    {
        const long tamanhoMaximo = 2 * 1024 * 1024;

        using var content = new MultipartFormDataContent();
        var stream = ficheiro.OpenReadStream(tamanhoMaximo);
        var fileContent = new StreamContent(stream);
        var contentType = ObterContentTypeImagem(ficheiro);
        if (!string.IsNullOrWhiteSpace(contentType))
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
                contentType
            );

        content.Add(fileContent, "imagem", ficheiro.Name);

        var resposta = await _http.PostAsync($"api/comunidades/{id}/imagem", content);

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await MensagemErroApi.ObterAsync(resposta, "Nao foi possivel enviar a imagem.")
            );

        return NormalizarFotosMembros(await resposta.Content.ReadFromJsonAsync<ComunidadeDTO>())
            ?? throw new InvalidOperationException("Resposta invalida do servidor.");
    }

    public async Task AderirAsync(Guid id)
    {
        var resposta = await _http.PostAsync($"api/comunidades/{id}/aderir", null);

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await MensagemErroApi.ObterAsync(resposta, "Nao foi possivel aderir a comunidade.")
            );
    }

    public async Task<ComunidadeDTO> ObterPorConviteAsync(string codigo)
    {
        var resposta = await _http.GetAsync(
            $"api/comunidades/convite/{Uri.EscapeDataString(codigo.Trim())}"
        );

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await MensagemErroApi.ObterAsync(resposta, "Convite nao encontrado.")
            );

        return NormalizarFotosMembros(await resposta.Content.ReadFromJsonAsync<ComunidadeDTO>())
            ?? throw new InvalidOperationException("Resposta invalida do servidor.");
    }

    public async Task AderirPorConviteAsync(string codigo)
    {
        var resposta = await _http.PostAsync(
            $"api/comunidades/convite/{Uri.EscapeDataString(codigo.Trim())}/aderir",
            null
        );

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await MensagemErroApi.ObterAsync(resposta, "Nao foi possivel aceitar o convite.")
            );
    }

    public async Task ApagarAsync(Guid id)
    {
        var resposta = await _http.DeleteAsync($"api/comunidades/{id}");

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await MensagemErroApi.ObterAsync(resposta, "Nao foi possivel apagar a comunidade.")
            );
    }

    public async Task SairAsync(Guid id)
    {
        var resposta = await _http.PostAsync($"api/comunidades/{id}/sair", null);

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await MensagemErroApi.ObterAsync(resposta, "Nao foi possivel sair da comunidade.")
            );
    }

    private ComunidadeDTO? NormalizarFotosMembros(ComunidadeDTO? comunidade)
    {
        if (comunidade == null)
            return comunidade;

        foreach (var membro in comunidade.Members)
        {
            if (string.IsNullOrWhiteSpace(membro.ProfileImageUrl))
                continue;

            membro.ProfileImageUrl = NormalizarUrl(membro.ProfileImageUrl);
        }

        comunidade.ImageUrl = NormalizarUrl(comunidade.ImageUrl);

        return comunidade;
    }

    private string NormalizarUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return string.Empty;

        if (Uri.TryCreate(url, UriKind.Absolute, out _))
            return url;

        return _http.BaseAddress is null ? url : new Uri(_http.BaseAddress, url).ToString();
    }

    private static string? ObterContentTypeImagem(IBrowserFile ficheiro)
    {
        if (!string.IsNullOrWhiteSpace(ficheiro.ContentType))
            return ficheiro.ContentType.Equals("image/jpg", StringComparison.OrdinalIgnoreCase)
                ? "image/jpeg"
                : ficheiro.ContentType;

        return Path.GetExtension(ficheiro.Name).ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => null,
        };
    }
}
