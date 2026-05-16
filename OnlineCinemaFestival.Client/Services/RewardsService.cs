using System.Net.Http.Json;
using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public class RewardsService
{
    private readonly HttpClient _http;

    public RewardsService(HttpClient http)
    {
        _http = http;
    }

    public async Task<int> GetSaldoAsync(string utilizadorId)
    {
        var response = await _http.GetFromJsonAsync<RewardsSaldoDto>($"api/rewards/{utilizadorId}");
        return response?.Pontos ?? 0;
    }

    public async Task<List<RewardTransacaoDto>> GetHistoricoAsync(string utilizadorId)
    {
        var response = await _http.GetFromJsonAsync<List<RewardTransacaoDto>>(
            $"api/rewards/{utilizadorId}/historico");
        return response ?? new List<RewardTransacaoDto>();
    }

    private sealed class RewardsSaldoDto
    {
        public string UtilizadorId { get; set; } = string.Empty;
        public int Pontos { get; set; }
    }
}
