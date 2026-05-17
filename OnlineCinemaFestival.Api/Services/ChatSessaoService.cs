using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class ChatSessaoService : IChatSessaoService
{
    private const int MinutosAntesAberturaChat = 15;
    private const int TamanhoMaximoMensagem = 600;
    private const int QuantidadeMaximaHistorico = 100;

    private readonly IMensagemChatSessaoRepository _mensagemChatSessaoRepository;
    private readonly IValidacaoAcessoService _validacaoAcessoService;
    private readonly IUtilizadorRepository _utilizadorRepository;

    public ChatSessaoService(
        IMensagemChatSessaoRepository mensagemChatSessaoRepository,
        IValidacaoAcessoService validacaoAcessoService,
        IUtilizadorRepository utilizadorRepository
    )
    {
        _mensagemChatSessaoRepository = mensagemChatSessaoRepository;
        _validacaoAcessoService = validacaoAcessoService;
        _utilizadorRepository = utilizadorRepository;
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
        var mensagem = new MensagemChatSessao
        {
            SessaoId = sessao.Id,
            UtilizadorId = utilizadorId,
            Utilizador = utilizador,
            Texto = textoNormalizado,
            EnviadaEm = DateTime.UtcNow,
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

        var acesso = await _validacaoAcessoService.ObterAcessoValidoParaSessaoAsync(
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

    private static void ValidarHorarioChat(Sessao sessao)
    {
        var agora = DateTime.UtcNow;
        var abertura = sessao.Inicio.AddMinutes(-MinutosAntesAberturaChat);

        if (agora < abertura)
            throw new InvalidOperationException("O chat ainda nao abriu para esta sessao.");

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
            Texto = mensagem.Removida ? string.Empty : mensagem.Texto,
            EnviadaEm = mensagem.EnviadaEm,
            Removida = mensagem.Removida,
            RemovidaPorModeracao = mensagem.RemovidaPorModeracao,
        };
    }

    private static string ObterGrupo(int sessaoId) => $"sessao-{sessaoId}";
}
