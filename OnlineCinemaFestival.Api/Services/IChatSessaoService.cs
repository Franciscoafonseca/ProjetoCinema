using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IChatSessaoService
{
    Task<SessaoChatEntradaDTO> EntrarNaSessaoAsync(
        int sessaoId,
        int utilizadorId,
        bool administrador
    );

    Task<MensagemChatSessaoReadDTO> EnviarMensagemAsync(
        int sessaoId,
        int utilizadorId,
        bool administrador,
        string texto
    );

    Task<IReadOnlyList<MensagemChatSessaoReadDTO>> ObterHistoricoRecenteAsync(
        int sessaoId,
        int utilizadorId,
        bool administrador,
        int quantidade
    );

    Task<MensagemChatRemovidaDTO> RemoverMensagemAsync(
        int sessaoId,
        string mensagemId,
        int utilizadorId,
        bool administrador
    );
}
