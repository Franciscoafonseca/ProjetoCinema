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

    public async Task<List<AcessoDto>> GetAcessosAsync()
    {
        return await _http.GetFromJsonAsync<List<AcessoDto>>("api/acessos") ?? new();
    }

    public async Task<List<TipoAcessoDto>> GetTiposAcessoAsync()
    {
        return await _http.GetFromJsonAsync<List<TipoAcessoDto>>("api/acessos/tipos") ?? new();
    }

    public async Task<List<AcessoUtilizadorDto>> GetMeusAcessosAsync()
    {
        return await _http.GetFromJsonAsync<List<AcessoUtilizadorDto>>("api/acessos/meus") ?? new();
    }

    public async Task<ValidacaoAcessoDto> ValidarFilmeAsync(int filmeId, int? festivalId = null)
    {
        var url = festivalId.HasValue
            ? $"api/acessos/validar-filme/{filmeId}?festivalId={festivalId.Value}"
            : $"api/acessos/validar-filme/{filmeId}";

        return await _http.GetFromJsonAsync<ValidacaoAcessoDto>(url) ?? new ValidacaoAcessoDto();
    }

    public async Task<ValidacaoAcessoDto> ValidarSessaoAsync(int sessaoId)
    {
        return await _http.GetFromJsonAsync<ValidacaoAcessoDto>(
                $"api/acessos/validar-sessao/{sessaoId}"
            ) ?? new ValidacaoAcessoDto();
    }
}
