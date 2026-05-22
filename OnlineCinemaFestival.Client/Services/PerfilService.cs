using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public class PerfilService
{
    private readonly HttpClient _http;

    public PerfilService(HttpClient http)
    {
        _http = http;
    }

    public async Task<PerfilUtilizadorRespostaDTO?> ObterMeuPerfilAsync()
    {
        var resposta = await _http.GetAsync("api/profiles/me");

        if (!resposta.IsSuccessStatusCode)
            return null;

        return NormalizarFoto(
            await resposta.Content.ReadFromJsonAsync<PerfilUtilizadorRespostaDTO>()
        );
    }

    public async Task<List<PerfilPublicoDTO>> ObterPerfisPublicosAsync()
    {
        var perfis =
            await _http.GetFromJsonAsync<List<PerfilPublicoDTO>>("api/profiles/public") ?? new();

        foreach (var perfil in perfis)
            NormalizarFoto(perfil);

        return perfis;
    }

    public async Task<PerfilPublicoDTO?> ObterPerfilPublicoAsync(int utilizadorId)
    {
        var resposta = await _http.GetAsync($"api/profiles/{utilizadorId}");

        if (!resposta.IsSuccessStatusCode)
            return null;

        return NormalizarFoto(await resposta.Content.ReadFromJsonAsync<PerfilPublicoDTO>());
    }

    public async Task<PerfilUtilizadorRespostaDTO?> AtualizarMeuPerfilAsync(
        PedidoAtualizarPerfilDTO pedido
    )
    {
        var resposta = await _http.PutAsJsonAsync("api/profiles/me", pedido);

        if (!resposta.IsSuccessStatusCode)
        {
            var mensagem = await MensagemErroApi.ObterAsync(
                resposta,
                "Nao foi possivel atualizar o perfil."
            );
            throw new InvalidOperationException(mensagem);
        }

        return NormalizarFoto(
            await resposta.Content.ReadFromJsonAsync<PerfilUtilizadorRespostaDTO>()
        );
    }

    public async Task<PerfilUtilizadorRespostaDTO?> EnviarFotoAsync(IBrowserFile ficheiro)
    {
        const long tamanhoMaximo = 2 * 1024 * 1024;

        using var content = new MultipartFormDataContent();

        var stream = ficheiro.OpenReadStream(tamanhoMaximo);
        var fileContent = new StreamContent(stream);

        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
            ficheiro.ContentType
        );

        content.Add(fileContent, "foto", ficheiro.Name);

        var response = await _http.PostAsync("api/profiles/foto", content);

        if (!response.IsSuccessStatusCode)
        {
            var erro = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(erro) ? "Não foi possível enviar a foto." : erro
            );
        }

        return NormalizarFoto(
            await response.Content.ReadFromJsonAsync<PerfilUtilizadorRespostaDTO>()
        );
    }

    public async Task<PerfilOpcoesDTO> ObterOpcoesAsync()
    {
        return await _http.GetFromJsonAsync<PerfilOpcoesDTO>("api/profiles/opcoes")
            ?? new PerfilOpcoesDTO();
    }

    private T? NormalizarFoto<T>(T? perfil)
        where T : PerfilPublicoDTO
    {
        if (perfil == null || string.IsNullOrWhiteSpace(perfil.ProfileImageUrl))
            return perfil;

        if (Uri.TryCreate(perfil.ProfileImageUrl, UriKind.Absolute, out _))
            return perfil;

        var baseUri = _http.BaseAddress?.GetLeftPart(UriPartial.Authority) ?? string.Empty;
        perfil.ProfileImageUrl = $"{baseUri}{perfil.ProfileImageUrl}";
        return perfil;
    }
}
