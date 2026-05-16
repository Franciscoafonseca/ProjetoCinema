using System.Net.Http.Json;
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

        return await resposta.Content.ReadFromJsonAsync<PerfilUtilizadorRespostaDTO>();
    }

    public async Task<List<PerfilPublicoDTO>> ObterPerfisPublicosAsync()
    {
        return await _http.GetFromJsonAsync<List<PerfilPublicoDTO>>("api/profiles/public") ?? new();
    }

    public async Task<PerfilPublicoDTO?> ObterPerfilPublicoAsync(int utilizadorId)
    {
        var resposta = await _http.GetAsync($"api/profiles/{utilizadorId}");

        if (!resposta.IsSuccessStatusCode)
            return null;

        return await resposta.Content.ReadFromJsonAsync<PerfilPublicoDTO>();
    }

    public async Task<PerfilUtilizadorRespostaDTO?> AtualizarMeuPerfilAsync(PedidoAtualizarPerfilDTO pedido)
    {
        var resposta = await _http.PutAsJsonAsync("api/profiles/me", pedido);

        if (!resposta.IsSuccessStatusCode)
        {
            var mensagem = await resposta.Content.ReadAsStringAsync();
            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(mensagem)
                    ? "Não foi possível atualizar o perfil."
                    : mensagem
            );
        }

        return await resposta.Content.ReadFromJsonAsync<PerfilUtilizadorRespostaDTO>();
    }
}
