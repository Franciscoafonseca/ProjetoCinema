using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Autorizacao;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Hubs;

[Authorize]
public class SessaoChatHub : Hub
{
    private const int MinutosAntesAberturaChat = 15;
    private const int TamanhoMaximoMensagem = 600;

    private readonly AppDbContext _context;
    private readonly IValidacaoAcessoService _validacaoAcessoService;

    public SessaoChatHub(AppDbContext context, IValidacaoAcessoService validacaoAcessoService)
    {
        _context = context;
        _validacaoAcessoService = validacaoAcessoService;
    }

    public async Task EntrarSessao(int sessaoId)
    {
        var sessao = await ObterSessaoAutorizadaAsync(sessaoId);

        ValidarChatAberto(sessao);

        await Groups.AddToGroupAsync(Context.ConnectionId, ObterGrupo(sessaoId));
        await Clients.Caller.SendAsync(
            "EntrouSessao",
            new
            {
                sessaoId,
                grupo = ObterGrupo(sessaoId),
                chatAbertoAte = sessao.Fim,
            }
        );
    }

    public async Task SairSessao(int sessaoId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, ObterGrupo(sessaoId));
    }

    public async Task EnviarMensagem(int sessaoId, string texto)
    {
        var sessao = await ObterSessaoAutorizadaAsync(sessaoId);

        ValidarChatAberto(sessao);

        var textoNormalizado = texto.Trim();

        if (string.IsNullOrWhiteSpace(textoNormalizado))
            throw new HubException("A mensagem nao pode estar vazia.");

        if (textoNormalizado.Length > TamanhoMaximoMensagem)
            throw new HubException($"A mensagem nao pode exceder {TamanhoMaximoMensagem} caracteres.");

        var mensagem = new MensagemChatSessao
        {
            SessaoId = sessaoId,
            UtilizadorId = ObterUtilizadorId(),
            Texto = textoNormalizado,
            EnviadaEm = DateTime.UtcNow,
            Removida = false,
            RemovidaPorModeracao = false,
        };

        await Clients.Group(ObterGrupo(sessaoId)).SendAsync("MensagemRecebida", mensagem);
    }

    [Authorize(Policy = NomesPoliticas.ApenasAdministrador)]
    public async Task RemoverMensagem(int sessaoId, string mensagemId)
    {
        if (string.IsNullOrWhiteSpace(mensagemId))
            throw new HubException("Mensagem invalida.");

        var sessaoExiste = await _context.Sessoes.AsNoTracking().AnyAsync(s => s.Id == sessaoId);

        if (!sessaoExiste)
            throw new HubException("Sessao nao encontrada.");

        await Clients
            .Group(ObterGrupo(sessaoId))
            .SendAsync(
                "MensagemRemovida",
                new
                {
                    sessaoId,
                    mensagemId,
                    removida = true,
                    removidaPorModeracao = true,
                }
            );
    }

    private async Task<Sessao> ObterSessaoAutorizadaAsync(int sessaoId)
    {
        var sessao = await _context
            .Sessoes.AsNoTracking()
            .Include(s => s.Festival)
            .Include(s => s.FilmesDaSessao)
            .FirstOrDefaultAsync(s => s.Id == sessaoId);

        if (sessao == null)
            throw new HubException("Sessao nao encontrada.");

        if (!sessao.TemChatAoVivo)
            throw new HubException("Esta sessao nao tem chat ao vivo.");

        if (Context.User?.IsInRole(NomesPapeis.Administrador) == true)
            return sessao;

        var acesso = await _validacaoAcessoService.ObterAcessoValidoParaSessaoAsync(
            ObterUtilizadorId(),
            sessao
        );

        if (acesso == null)
            throw new HubException("Nao possui acesso valido ao chat desta sessao.");

        return sessao;
    }

    private int ObterUtilizadorId()
    {
        var valor = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(valor, out var utilizadorId))
            throw new HubException("Token invalido.");

        return utilizadorId;
    }

    private static void ValidarChatAberto(Sessao sessao)
    {
        var agora = DateTime.UtcNow;
        var abertura = sessao.Inicio.AddMinutes(-MinutosAntesAberturaChat);

        if (agora < abertura)
            throw new HubException("O chat ainda nao abriu para esta sessao.");

        if (agora > sessao.Fim)
            throw new HubException("O chat ja fechou para esta sessao.");
    }

    private static string ObterGrupo(int sessaoId) => $"sessao-{sessaoId}";
}
