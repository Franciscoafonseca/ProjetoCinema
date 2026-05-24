using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class ChatSessaoService : IChatSessaoService
{
    private const int TamanhoMaximoMensagem = 600;
    private const int QuantidadeMaximaHistorico = 100;
    private const int MaximoMensagensJanelaSpam = 5;
    private static readonly TimeSpan JanelaSpam = TimeSpan.FromSeconds(20);
    private static readonly TimeSpan JanelaMensagemRepetida = TimeSpan.FromSeconds(30);

    private readonly IMensagemChatSessaoRepository _mensagemChatSessaoRepository;
    private readonly IAcessoVisualizacaoService _acessoVisualizacaoService;
    private readonly IUtilizadorRepository _utilizadorRepository;
    private readonly TimeProvider _timeProvider;

    public ChatSessaoService(
        IMensagemChatSessaoRepository mensagemChatSessaoRepository,
        IAcessoVisualizacaoService acessoVisualizacaoService,
        IUtilizadorRepository utilizadorRepository,
        TimeProvider timeProvider
    )
    {
        _mensagemChatSessaoRepository = mensagemChatSessaoRepository;
        _acessoVisualizacaoService = acessoVisualizacaoService;
        _utilizadorRepository = utilizadorRepository;
        _timeProvider = timeProvider;
    }

    public async Task<SessaoChatEntradaDTO> EntrarNaSessaoAsync(
        int sessaoId,
        int utilizadorId,
        bool administrador
    )
    {
        var sessao = await ObterSessaoComAcessoAoChatAsync(sessaoId, utilizadorId, administrador);
        ValidarHorarioChat(sessao);

        return new SessaoChatEntradaDTO
        {
            SessaoId = sessao.Id,
            Grupo = ObterGrupo(sessao.Id),
            ChatAbertoAte = sessao.Fim,
        };
    }

    public async Task<MensagemChatSessaoReadDTO> EnviarMensagemAsync(
        int sessaoId,
        int utilizadorId,
        bool administrador,
        string texto
    )
    {
        var sessao = await ObterSessaoComAcessoAoChatAsync(sessaoId, utilizadorId, administrador);
        ValidarHorarioChat(sessao);

        var utilizador = await _utilizadorRepository.ObterPorIdAsync(utilizadorId);

        if (utilizador == null)
            throw new UnauthorizedAccessException("Utilizador nao encontrado.");

        var textoNormalizado = ValidarTexto(texto);
        await ValidarSpamSimplesAsync(sessao.Id, utilizadorId, textoNormalizado);

        var mensagem = new MensagemChatSessao
        {
            SessaoId = sessao.Id,
            UtilizadorId = utilizadorId,
            Utilizador = utilizador,
            Texto = textoNormalizado,
            EnviadaEm = AgoraUtc(),
            Removida = false,
            RemovidaPorModeracao = false,
        };

        await _mensagemChatSessaoRepository.AdicionarAsync(mensagem);
        await _mensagemChatSessaoRepository.SaveChangesAsync();

        return MapToReadDTO(mensagem);
    }

    public async Task<IReadOnlyList<MensagemChatSessaoReadDTO>> ObterHistoricoRecenteAsync(
        int sessaoId,
        int utilizadorId,
        bool administrador,
        int quantidade
    )
    {
        var sessao = await ObterSessaoComAcessoAoChatAsync(sessaoId, utilizadorId, administrador);
        ValidarHorarioChat(sessao);

        var quantidadeNormalizada = Math.Clamp(quantidade, 1, QuantidadeMaximaHistorico);
        var mensagens = await _mensagemChatSessaoRepository.ListarHistoricoRecenteAsync(
            sessao.Id,
            quantidadeNormalizada
        );

        return mensagens.Select(MapToReadDTO).ToList();
    }

    public async Task<MensagemChatRemovidaDTO> RemoverMensagemAsync(
        int sessaoId,
        string mensagemId,
        int utilizadorId,
        bool administrador
    )
    {
        if (!administrador)
            throw new UnauthorizedAccessException("Apenas administradores podem remover mensagens.");

        if (string.IsNullOrWhiteSpace(mensagemId))
            throw new ArgumentException("Mensagem invalida.");

        var sessao = await ObterSessaoComAcessoAoChatAsync(sessaoId, utilizadorId, administrador);
        var mensagem = await _mensagemChatSessaoRepository.ObterMensagemPorIdAsync(mensagemId);

        if (mensagem == null || mensagem.SessaoId != sessao.Id)
            throw new KeyNotFoundException("Mensagem nao encontrada.");

        _mensagemChatSessaoRepository.MarcarMensagemRemovida(mensagem);
        await _mensagemChatSessaoRepository.SaveChangesAsync();

        return new MensagemChatRemovidaDTO
        {
            SessaoId = sessao.Id,
            MensagemId = mensagem.Id,
            Removida = mensagem.Removida,
            RemovidaPorModeracao = mensagem.RemovidaPorModeracao,
            EstadoModeracao = mensagem.RemovidaPorModeracao ? "RemovidaPorModeracao" : "Aprovada",
        };
    }

    private async Task<Sessao> ObterSessaoComAcessoAoChatAsync(
        int sessaoId,
        int utilizadorId,
        bool administrador
    )
    {
        var sessao = await _mensagemChatSessaoRepository.ObterSessaoPorIdAsync(sessaoId);

        if (sessao == null)
            throw new KeyNotFoundException("Sessao nao encontrada.");

        if (!sessao.TemChatAoVivo)
            throw new InvalidOperationException("Esta sessao nao tem chat ao vivo.");

        if (administrador)
            return sessao;

        var acesso = await _acessoVisualizacaoService.ObterAcessoValidoParaSessaoAsync(
            utilizadorId,
            sessao
        );

        if (acesso == null)
            throw new UnauthorizedAccessException("Nao possui acesso valido ao chat desta sessao.");

        return sessao;
    }

    private static string ValidarTexto(string texto)
    {
        var textoNormalizado = texto.Trim();

        if (string.IsNullOrWhiteSpace(textoNormalizado))
            throw new ArgumentException("A mensagem nao pode estar vazia.");

        if (textoNormalizado.Length > TamanhoMaximoMensagem)
            throw new ArgumentException(
                $"A mensagem nao pode exceder {TamanhoMaximoMensagem} caracteres."
            );

        return textoNormalizado;
    }

    private async Task ValidarSpamSimplesAsync(int sessaoId, int utilizadorId, string texto)
    {
        var agora = AgoraUtc();
        var mensagensRecentes =
            await _mensagemChatSessaoRepository.ListarMensagensRecentesDoUtilizadorAsync(
                sessaoId,
                utilizadorId,
                agora.Subtract(JanelaMensagemRepetida)
            );

        if (
            mensagensRecentes.Any(m =>
                agora - m.EnviadaEm <= JanelaMensagemRepetida
                && string.Equals(m.Texto, texto, StringComparison.OrdinalIgnoreCase)
            )
        )
        {
            throw new ArgumentException("Mensagem repetida em pouco tempo. Aguarde antes de reenviar.");
        }

        if (mensagensRecentes.Count(m => agora - m.EnviadaEm <= JanelaSpam) >= MaximoMensagensJanelaSpam)
            throw new ArgumentException("Demasiadas mensagens em pouco tempo. Aguarde alguns segundos.");
    }

    private void ValidarHorarioChat(Sessao sessao)
    {
        var agora = AgoraUtc();

        if (agora < sessao.Inicio)
            throw new InvalidOperationException("Chat ainda nao disponivel.");

        if (agora > sessao.Fim)
            throw new InvalidOperationException("O chat ja fechou para esta sessao.");
    }

    private static MensagemChatSessaoReadDTO MapToReadDTO(MensagemChatSessao mensagem)
    {
        return new MensagemChatSessaoReadDTO
        {
            Id = mensagem.Id,
            SessaoId = mensagem.SessaoId,
            UtilizadorId = mensagem.UtilizadorId,
            NomeUtilizador = mensagem.Utilizador?.Name ?? string.Empty,
            ProfileImageUrl = mensagem.Utilizador?.Perfil?.ProfileImageUrl ?? string.Empty,
            Texto = mensagem.Removida ? string.Empty : mensagem.Texto,
            EnviadaEm = mensagem.EnviadaEm,
            Removida = mensagem.Removida,
            RemovidaPorModeracao = mensagem.RemovidaPorModeracao,
            EstadoModeracao = mensagem.RemovidaPorModeracao ? "RemovidaPorModeracao" : "Aprovada",
        };
    }

    private static string ObterGrupo(int sessaoId) => $"sessao-{sessaoId}";

    private DateTime AgoraUtc() => _timeProvider.GetUtcNow().UtcDateTime;
}
