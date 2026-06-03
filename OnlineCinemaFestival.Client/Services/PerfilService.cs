using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public class PerfilService
{
    private readonly HttpClient _http;
    private readonly PerfilEstadoService _perfilEstado;
    private readonly ImagemUrlService _imagemUrlService;

    public PerfilService(
        HttpClient http,
        PerfilEstadoService perfilEstado,
        ImagemUrlService imagemUrlService
    )
    {
        _http = http;
        _perfilEstado = perfilEstado;
        _imagemUrlService = imagemUrlService;
    }

    public async Task<PerfilUtilizadorRespostaDTO?> ObterMeuPerfilAsync()
    {
        var resposta = await _http.GetAsync("api/profiles/me");

        if (!resposta.IsSuccessStatusCode)
            return null;

        var perfil = NormalizarFoto(
            await resposta.Content.ReadFromJsonAsync<PerfilUtilizadorRespostaDTO>()
        );
        _perfilEstado.Atualizar(perfil);
        return perfil;
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

        var perfil = NormalizarFoto(
            await resposta.Content.ReadFromJsonAsync<PerfilUtilizadorRespostaDTO>()
        );
        _perfilEstado.Atualizar(perfil);
        return perfil;
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

        var perfil = NormalizarFoto(
            await response.Content.ReadFromJsonAsync<PerfilUtilizadorRespostaDTO>()
        );
        _perfilEstado.Atualizar(perfil);
        return perfil;
    }

    public async Task<PerfilOpcoesDTO> ObterOpcoesAsync()
    {
        return await _http.GetFromJsonAsync<PerfilOpcoesDTO>("api/profiles/opcoes")
            ?? new PerfilOpcoesDTO();
    }

    public async Task ReportarUtilizadorAsync(int utilizadorId, CriarReporteUtilizadorDTO dto)
    {
        var resposta = await _http.PostAsJsonAsync($"api/profiles/{utilizadorId}/reportes", dto);

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await MensagemErroApi.ObterAsync(resposta, "Nao foi possivel enviar o reporte.")
            );
    }

    public async Task<List<ReporteUtilizadorDTO>> ObterReportesUtilizadoresAsync()
    {
        var resposta = await _http.GetAsync("api/admin/reportes-utilizadores");

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await MensagemErroApi.ObterAsync(resposta, "Nao foi possivel carregar reportes.")
            );

        return await resposta.Content.ReadFromJsonAsync<List<ReporteUtilizadorDTO>>() ?? new();
    }

    public async Task<ReporteUtilizadorDTO> AtualizarEstadoReporteAsync(
        int reporteId,
        EstadoReporteUtilizador estado
    )
    {
        var resposta = await _http.PatchAsJsonAsync(
            $"api/admin/reportes-utilizadores/{reporteId}/estado",
            new AtualizarEstadoReporteUtilizadorDTO { Estado = estado }
        );

        if (!resposta.IsSuccessStatusCode)
            throw new InvalidOperationException(
                await MensagemErroApi.ObterAsync(resposta, "Nao foi possivel atualizar o reporte.")
            );

        return await resposta.Content.ReadFromJsonAsync<ReporteUtilizadorDTO>()
            ?? throw new InvalidOperationException("Resposta invalida do servidor.");
    }

    private T? NormalizarFoto<T>(T? perfil)
        where T : PerfilPublicoDTO
    {
        if (perfil == null)
            return perfil;

        perfil.ProfileImageUrl = _imagemUrlService.Resolver(perfil.ProfileImageUrl);

        foreach (var comunidade in perfil.PublicCommunities)
            comunidade.ImageUrl = _imagemUrlService.Resolver(comunidade.ImageUrl);

        return perfil;
    }
}
