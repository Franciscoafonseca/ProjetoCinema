using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using OnlineCinemaFestival.Api.Autorizacao;
using OnlineCinemaFestival.Api.Extensions;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Hubs;

[Authorize]
public class SessaoChatHub : Hub
{
    private readonly IChatSessaoService _chatSessaoService;

    public SessaoChatHub(IChatSessaoService chatSessaoService)
    {
        _chatSessaoService = chatSessaoService;
    }

    public async Task EntrarSessao(int sessaoId)
    {
        var entrada = await ExecutarComErroHubAsync(() =>
            _chatSessaoService.EntrarNaSessaoAsync(
                sessaoId,
                ObterUtilizadorId(),
                UtilizadorEAdministrador()
            )
        );

        await Groups.AddToGroupAsync(Context.ConnectionId, entrada.Grupo);
        await Clients.Caller.SendAsync("EntrouSessao", entrada);
    }

    public async Task SairSessao(int sessaoId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, ObterGrupo(sessaoId));
    }

    public async Task EnviarMensagem(int sessaoId, string texto)
    {
        var mensagem = await ExecutarComErroHubAsync(() =>
            _chatSessaoService.EnviarMensagemAsync(
                sessaoId,
                ObterUtilizadorId(),
                UtilizadorEAdministrador(),
                texto
            )
        );

        await Clients.Group(ObterGrupo(sessaoId)).SendAsync("MensagemRecebida", mensagem);
    }

    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task RemoverMensagem(int sessaoId, string mensagemId)
    {
        var mensagemRemovida = await ExecutarComErroHubAsync(() =>
            _chatSessaoService.RemoverMensagemAsync(
                sessaoId,
                mensagemId,
                ObterUtilizadorId(),
                UtilizadorEAdministrador()
            )
        );

        await Clients.Group(ObterGrupo(sessaoId)).SendAsync("MensagemRemovida", mensagemRemovida);
    }

    private static string ObterGrupo(int sessaoId) => $"sessao-{sessaoId}";

    private int ObterUtilizadorId()
    {
        return Context.User?.GetUserId() ?? throw new HubException("Token invalido.");
    }

    private bool UtilizadorEAdministrador()
    {
        return Context.User?.IsInRole(NomesPapeis.Administrador) == true;
    }

    private static async Task<T> ExecutarComErroHubAsync<T>(Func<Task<T>> acao)
    {
        try
        {
            return await acao();
        }
        catch (Exception ex)
            when (ex is ArgumentException
                or InvalidOperationException
                or KeyNotFoundException
                or UnauthorizedAccessException)
        {
            throw new HubException(ex.Message);
        }
    }
}
