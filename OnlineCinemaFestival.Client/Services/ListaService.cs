using System.Net;
using System.Net.Http.Json;
using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public class ListaService
{
    private readonly HttpClient _http;

    public ListaService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ListaPessoalDTO>> ObterMinhasListasAsync()
    {
        var resposta = await _http.GetAsync("api/listas");

        if (!resposta.IsSuccessStatusCode)
            return new();

        return await resposta.Content.ReadFromJsonAsync<List<ListaPessoalDTO>>() ?? new();
    }

    public async Task<ListaPessoalDTO> CriarAsync(CriarListaRequest pedido)
    {
        var resposta = await _http.PostAsJsonAsync("api/listas", pedido);

        if (!resposta.IsSuccessStatusCode)
        {
            var mensagem = await MensagemErroApi.ObterAsync(
                resposta,
                "Nao foi possivel criar a lista."
            );
            throw new InvalidOperationException(mensagem);
        }

        return await resposta.Content.ReadFromJsonAsync<ListaPessoalDTO>()
            ?? throw new InvalidOperationException("Resposta invalida do servidor.");
    }

    public async Task<bool> AdicionarFilmeAsync(int listaId, int filmeId)
    {
        var resposta = await _http.PostAsync($"api/listas/{listaId}/filmes/{filmeId}", null);

        if (resposta.StatusCode == HttpStatusCode.Conflict)
            return false;

        if (!resposta.IsSuccessStatusCode)
        {
            var mensagem = await MensagemErroApi.ObterAsync(
                resposta,
                "Nao foi possivel adicionar o filme a lista."
            );
            throw new InvalidOperationException(mensagem);
        }

        return true;
    }

    public async Task RemoverFilmeAsync(int listaId, int filmeId)
    {
        var resposta = await _http.DeleteAsync($"api/listas/{listaId}/filmes/{filmeId}");

        if (resposta.StatusCode == HttpStatusCode.NotFound)
            return;

        if (!resposta.IsSuccessStatusCode)
        {
            var mensagem = await MensagemErroApi.ObterAsync(
                resposta,
                "Nao foi possivel remover o filme da lista."
            );
            throw new InvalidOperationException(mensagem);
        }
    }

    public async Task RemoverListaAsync(int listaId)
    {
        var resposta = await _http.DeleteAsync($"api/listas/{listaId}");

        if (!resposta.IsSuccessStatusCode)
        {
            var mensagem = await MensagemErroApi.ObterAsync(
                resposta,
                "Nao foi possivel apagar a lista."
            );
            throw new InvalidOperationException(mensagem);
        }
    }
}
