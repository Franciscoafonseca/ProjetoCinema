using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Repositories;

public interface IMensagemChatSessaoRepository
{
    Task<Sessao?> ObterSessaoPorIdAsync(int sessaoId);

    Task AdicionarAsync(MensagemChatSessao mensagem);

    Task<IReadOnlyList<MensagemChatSessao>> ListarHistoricoRecenteAsync(
        int sessaoId,
        int quantidade
    );

    Task<MensagemChatSessao?> ObterMensagemPorIdAsync(string mensagemId);

    void MarcarMensagemRemovida(MensagemChatSessao mensagem);

    Task SaveChangesAsync();
}
