using System.Net.Http.Json;
using OnlineCinemaFestival.Client.Models;

namespace OnlineCinemaFestival.Client.Services;

public class AutenticacaoService : IAutenticacaoService
{
    private readonly HttpClient _http;
    private readonly ArmazenamentoToken _armazenamento;
    private readonly EstadoAutenticacaoCustomizado _estado;
    private readonly PerfilEstadoService _perfilEstado;

    public AutenticacaoService(
        HttpClient http,
        ArmazenamentoToken armazenamento,
        EstadoAutenticacaoCustomizado estado,
        PerfilEstadoService perfilEstado
    )
    {
        _http = http;
        _armazenamento = armazenamento;
        _estado = estado;
        _perfilEstado = perfilEstado;
    }

    public async Task<AutenticacaoRespostaDTO> EntrarAsync(PedidoLoginDTO pedido)
    {
        var resposta = await _http.PostAsJsonAsync("api/auth/login", pedido);

        if (!resposta.IsSuccessStatusCode)
        {
            var mensagem = await MensagemErroApi.ObterAsync(
                resposta,
                "Email ou palavra-passe invalidos."
            );
            throw new InvalidOperationException(mensagem);
        }

        var resultado =
            await resposta.Content.ReadFromJsonAsync<AutenticacaoRespostaDTO>()
            ?? throw new InvalidOperationException("Resposta inválida do servidor.");

        await _armazenamento.GuardarAsync(resultado.Token);
        _estado.NotificarAutenticado();
        return resultado;
    }

    public async Task<AutenticacaoRespostaDTO> RegistarAsync(PedidoRegistoDTO pedido)
    {
        var resposta = await _http.PostAsJsonAsync("api/auth/register", pedido);

        if (!resposta.IsSuccessStatusCode)
        {
            var mensagem = await MensagemErroApi.ObterAsync(
                resposta,
                "Nao foi possivel criar a conta. Reve os dados e tenta novamente."
            );
            throw new InvalidOperationException(mensagem);
        }

        var resultado =
            await resposta.Content.ReadFromJsonAsync<AutenticacaoRespostaDTO>()
            ?? throw new InvalidOperationException("Resposta inválida do servidor.");

        await _armazenamento.GuardarAsync(resultado.Token);
        _estado.NotificarAutenticado();
        return resultado;
    }

    public async Task TerminarSessaoAsync()
    {
        await _armazenamento.RemoverAsync();
        _perfilEstado.Limpar();
        _estado.NotificarTerminouSessao();
    }
}
