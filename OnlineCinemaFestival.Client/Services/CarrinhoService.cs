using System.Net.Http.Json;
using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public class CarrinhoService
{
    private readonly HttpClient _http;

    public CarrinhoService(HttpClient http)
    {
        _http = http;
    }

    public async Task<CarrinhoDto> ObterAsync()
    {
        return await _http.GetFromJsonAsync<CarrinhoDto>("api/carrinho") ?? new CarrinhoDto();
    }

    public async Task<CarrinhoDto> AdicionarItemAsync(int acessoId)
    {
        var resposta = await _http.PostAsJsonAsync(
            "api/carrinho/itens",
            new AdicionarItemCarrinhoDto { AcessoId = acessoId }
        );

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(await ObterMensagemErroAsync(resposta));

        return await resposta.Content.ReadFromJsonAsync<CarrinhoDto>() ?? new CarrinhoDto();
    }

    public async Task RemoverItemAsync(int itemId)
    {
        var resposta = await _http.DeleteAsync($"api/carrinho/itens/{itemId}");

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(await ObterMensagemErroAsync(resposta));
    }

    public async Task LimparAsync()
    {
        var resposta = await _http.DeleteAsync("api/carrinho");

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(await ObterMensagemErroAsync(resposta));
    }

    private static async Task<string> ObterMensagemErroAsync(HttpResponseMessage resposta)
    {
        var conteudo = await resposta.Content.ReadAsStringAsync();
        return string.IsNullOrWhiteSpace(conteudo)
            ? "Nao foi possivel atualizar o carrinho."
            : conteudo;
    }
}
