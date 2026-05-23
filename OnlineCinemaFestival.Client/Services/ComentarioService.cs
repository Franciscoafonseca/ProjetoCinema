using System.Net.Http.Json;
using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public class ComentarioService
{
    private readonly HttpClient _http;

    public ComentarioService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ComentarioDTO>> ObterDaComunidadeAsync(Guid comunidadeId)
    {
        var resposta = await _http.GetAsync($"api/comunidades/{comunidadeId}/comentarios");

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await MensagemErroApi.ObterAsync(resposta, "Nao foi possivel carregar comentarios.")
            );

        var comentarios = await resposta.Content.ReadFromJsonAsync<List<ComentarioDTO>>() ?? new();
        foreach (var comentario in comentarios)
            NormalizarFotoAutor(comentario);
        return comentarios;
    }

    public async Task<ComentarioDTO> CriarNaComunidadeAsync(Guid comunidadeId, ComentarioCreateDTO dto)
    {
        var resposta = await _http.PostAsJsonAsync(
            $"api/comunidades/{comunidadeId}/comentarios",
            dto
        );

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await MensagemErroApi.ObterAsync(resposta, "Nao foi possivel publicar o comentario.")
            );

        return NormalizarFotoAutor(await resposta.Content.ReadFromJsonAsync<ComentarioDTO>())
            ?? throw new InvalidOperationException("Resposta invalida do servidor.");
    }

    public async Task<List<ComentarioDTO>> ObterDoFilmeAsync(int filmeId)
    {
        var resposta = await _http.GetAsync($"api/filmes/{filmeId}/comentarios");

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await MensagemErroApi.ObterAsync(resposta, "Nao foi possivel carregar comentarios.")
            );

        var comentarios = await resposta.Content.ReadFromJsonAsync<List<ComentarioDTO>>() ?? new();
        foreach (var comentario in comentarios)
            NormalizarFotoAutor(comentario);
        return comentarios;
    }

    public async Task<ComentarioDTO> CriarNoFilmeAsync(int filmeId, ComentarioCreateDTO dto)
    {
        var resposta = await _http.PostAsJsonAsync($"api/filmes/{filmeId}/comentarios", dto);

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await MensagemErroApi.ObterAsync(resposta, "Nao foi possivel publicar o comentario.")
            );

        return NormalizarFotoAutor(await resposta.Content.ReadFromJsonAsync<ComentarioDTO>())
            ?? throw new InvalidOperationException("Resposta invalida do servidor.");
    }

    public async Task ReportarAsync(Guid comunidadeId, int comentarioId)
    {
        var resposta = await _http.PostAsync(
            $"api/comunidades/{comunidadeId}/comentarios/{comentarioId}/reportar",
            null
        );

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await MensagemErroApi.ObterAsync(resposta, "Nao foi possivel reportar o comentario.")
            );
    }

    public async Task<List<ComentarioDTO>> ObterReportadosDaComunidadeAsync(Guid comunidadeId)
    {
        var resposta = await _http.GetAsync($"api/comunidades/{comunidadeId}/comentarios/reportados");

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await MensagemErroApi.ObterAsync(resposta, "Nao foi possivel carregar comentarios reportados.")
            );

        var comentarios = await resposta.Content.ReadFromJsonAsync<List<ComentarioDTO>>() ?? new();
        foreach (var comentario in comentarios)
            NormalizarFotoAutor(comentario);
        return comentarios;
    }

    public async Task AtualizarVisibilidadeAsync(
        Guid comunidadeId,
        int comentarioId,
        ComentarioVisibilidadeDTO dto
    )
    {
        var resposta = await _http.PutAsJsonAsync(
            $"api/comunidades/{comunidadeId}/comentarios/{comentarioId}/visibilidade",
            dto
        );

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await MensagemErroApi.ObterAsync(resposta, "Nao foi possivel atualizar a visibilidade.")
            );
    }

    private ComentarioDTO? NormalizarFotoAutor(ComentarioDTO? comentario)
    {
        if (comentario == null || string.IsNullOrWhiteSpace(comentario.UsuarioFotoUrl))
            return comentario;

        if (Uri.TryCreate(comentario.UsuarioFotoUrl, UriKind.Absolute, out _))
            return comentario;

        var baseUri = _http.BaseAddress?.GetLeftPart(UriPartial.Authority) ?? string.Empty;
        comentario.UsuarioFotoUrl = $"{baseUri}{comentario.UsuarioFotoUrl}";
        return comentario;
    }
}
