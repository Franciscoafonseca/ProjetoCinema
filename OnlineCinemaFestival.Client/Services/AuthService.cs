using System.Net.Http.Json;
using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly ArmazenamentoToken _armazenamento;
    private readonly EstadoAutenticacaoCustomizado _estado;

    public AuthService(
        HttpClient http,
        ArmazenamentoToken armazenamento,
        EstadoAutenticacaoCustomizado estado)
    {
        _http = http;
        _armazenamento = armazenamento;
        _estado = estado;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest pedido)
    {
        var resposta = await _http.PostAsJsonAsync("api/auth/login", pedido);

        if (!resposta.IsSuccessStatusCode)
        {
            var mensagem = await resposta.Content.ReadAsStringAsync();
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(mensagem)
                ? "Credenciais inválidas."
                : mensagem);
        }

        var resultado = await resposta.Content.ReadFromJsonAsync<AuthResponse>()
            ?? throw new InvalidOperationException("Resposta inválida do servidor.");

        await _armazenamento.GuardarAsync(resultado.Token);
        _estado.NotificarAutenticado();
        return resultado;
    }

    public async Task<AuthResponse> RegistarAsync(RegisterRequest pedido)
    {
        var resposta = await _http.PostAsJsonAsync("api/auth/register", pedido);

        if (!resposta.IsSuccessStatusCode)
        {
            var mensagem = await resposta.Content.ReadAsStringAsync();
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(mensagem)
                ? "Não foi possível criar a conta."
                : mensagem);
        }

        var resultado = await resposta.Content.ReadFromJsonAsync<AuthResponse>()
            ?? throw new InvalidOperationException("Resposta inválida do servidor.");

        await _armazenamento.GuardarAsync(resultado.Token);
        _estado.NotificarAutenticado();
        return resultado;
    }

    public async Task TerminarSessaoAsync()
    {
        await _armazenamento.RemoverAsync();
        _estado.NotificarTerminouSessao();
    }
}
