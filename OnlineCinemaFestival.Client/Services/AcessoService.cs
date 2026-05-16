using System.Net.Http.Json;
using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public class AcessoService
{
    private readonly HttpClient _http;

    public AcessoService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<AcessoDTO>> ObterAcessosAsync()
    {
        return await _http.GetFromJsonAsync<List<AcessoDTO>>("api/acessos") ?? new();
    }

    public async Task<List<TipoAcessoDTO>> ObterTiposAcessoAsync()
    {
        return await _http.GetFromJsonAsync<List<TipoAcessoDTO>>("api/acessos/tipos") ?? new();
    }

    public async Task<List<AcessoUtilizadorDTO>> ObterMeusAcessosAsync()
    {
        return await _http.GetFromJsonAsync<List<AcessoUtilizadorDTO>>("api/acessos/meus") ?? new();
    }

    public async Task<ValidacaoAcessoDTO> ValidarFilmeAsync(int filmeId, int? festivalId = null)
    {
        var url = festivalId.HasValue
            ? $"api/acessos/validar-filme/{filmeId}?festivalId={festivalId.Value}"
            : $"api/acessos/validar-filme/{filmeId}";

        return await _http.GetFromJsonAsync<ValidacaoAcessoDTO>(url) ?? new ValidacaoAcessoDTO();
    }

    public async Task<ValidacaoAcessoDTO> ValidarSessaoAsync(int sessaoId)
    {
        return await _http.GetFromJsonAsync<ValidacaoAcessoDTO>(
                $"api/acessos/validar-sessao/{sessaoId}"
            ) ?? new ValidacaoAcessoDTO();
    }
}
