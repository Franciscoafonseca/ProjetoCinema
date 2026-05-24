using OnlineCinemaFestival.Api.Domain;
using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Tests.Support.Fakes;

public sealed class MensagemChatSessaoRepositoryFalso : IMensagemChatSessaoRepository
{
    private readonly Sessao _sessao;

    public MensagemChatSessaoRepositoryFalso(Sessao sessao)
    {
        _sessao = sessao;
    }

    public Task<Sessao?> ObterSessaoPorIdAsync(int sessaoId) =>
        Task.FromResult(sessaoId == _sessao.Id ? _sessao : null);

    public Task AdicionarAsync(MensagemChatSessao mensagem) => Task.CompletedTask;

    public Task<IReadOnlyList<MensagemChatSessao>> ListarHistoricoRecenteAsync(
        int sessaoId,
        int quantidade
    ) => Task.FromResult<IReadOnlyList<MensagemChatSessao>>(Array.Empty<MensagemChatSessao>());

    public Task<IReadOnlyList<MensagemChatSessao>> ListarMensagensRecentesDoUtilizadorAsync(
        int sessaoId,
        int utilizadorId,
        DateTime desde
    ) => Task.FromResult<IReadOnlyList<MensagemChatSessao>>(Array.Empty<MensagemChatSessao>());

    public Task<MensagemChatSessao?> ObterMensagemPorIdAsync(string mensagemId) =>
        Task.FromResult<MensagemChatSessao?>(null);

    public void MarcarMensagemRemovida(MensagemChatSessao mensagem) { }

    public Task SaveChangesAsync() => Task.CompletedTask;
}

public sealed class AcessoVisualizacaoServiceFalso : IAcessoVisualizacaoService
{
    public Task<AcessoUtilizador?> ObterAcessoValidoParaFilmeAsync(
        int utilizadorId,
        Filme filme,
        int? festivalId
    ) => Task.FromResult<AcessoUtilizador?>(new AcessoUtilizador { UtilizadorId = utilizadorId });

    public Task<AcessoUtilizador?> ObterAcessoValidoParaSessaoAsync(int utilizadorId, Sessao sessao) =>
        Task.FromResult<AcessoUtilizador?>(new AcessoUtilizador { UtilizadorId = utilizadorId });

    public Task<ResultadoAcessoVisualizacao> ObterResultadoParaFilmeAsync(
        int utilizadorId,
        Filme filme,
        int? festivalId
    ) =>
        Task.FromResult(
            ResultadoAcessoVisualizacao.Autorizado(new AcessoUtilizador { UtilizadorId = utilizadorId })
        );

    public Task<ResultadoAcessoVisualizacao> ObterResultadoParaSessaoAsync(
        int utilizadorId,
        Sessao sessao
    ) =>
        Task.FromResult(
            ResultadoAcessoVisualizacao.Autorizado(new AcessoUtilizador { UtilizadorId = utilizadorId })
        );

    public Task<bool> PodeVisualizarFilmeAsync(int utilizadorId, int filmeId, int? festivalId) =>
        Task.FromResult(true);

    public Task<bool> PodeVisualizarSessaoAsync(int utilizadorId, Sessao sessao) =>
        Task.FromResult(true);
}

