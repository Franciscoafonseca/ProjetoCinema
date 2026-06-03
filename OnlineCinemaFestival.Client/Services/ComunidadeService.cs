using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public class ComunidadeService
{
    private readonly HttpClient _http;
    private readonly ImagemUrlService _imagemUrlService;

    public ComunidadeService(HttpClient http, ImagemUrlService imagemUrlService)
    {
        _http = http;
        _imagemUrlService = imagemUrlService;
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
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
            ficheiro.ContentType
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
            membro.ProfileImageUrl = _imagemUrlService.Resolver(membro.ProfileImageUrl);
        }

        comunidade.ImageUrl = _imagemUrlService.Resolver(comunidade.ImageUrl);

        return comunidade;
    }
}
