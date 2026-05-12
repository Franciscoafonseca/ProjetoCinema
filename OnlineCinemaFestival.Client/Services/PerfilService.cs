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

    public async Task<UserProfileResponse?> GetMeuPerfilAsync()
    {
        var resposta = await _http.GetAsync("api/profiles/me");

        if (!resposta.IsSuccessStatusCode)
            return null;

        return await resposta.Content.ReadFromJsonAsync<UserProfileResponse>();
    }

    public async Task<UserProfileResponse?> AtualizarMeuPerfilAsync(UpdateProfileRequest pedido)
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

        return await resposta.Content.ReadFromJsonAsync<UserProfileResponse>();
    }
}
