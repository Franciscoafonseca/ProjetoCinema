using OnlineCinemaFestival.Api.Domain;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class RewardsPontuacaoService : IRewardsPontuacaoService
{
    private readonly IRewardsRepository _rewardsRepository;
    private readonly IRewardTransacaoRepository _transacaoRepository;
    private readonly TimeProvider _timeProvider;

    public RewardsPontuacaoService(
        IRewardsRepository rewardsRepository,
        IRewardTransacaoRepository transacaoRepository,
        TimeProvider timeProvider
    )
    {
        _rewardsRepository = rewardsRepository;
        _transacaoRepository = transacaoRepository;
        _timeProvider = timeProvider;
    }

    public async Task<bool> AtribuirSeAindaNaoAtribuidoAsync(
        int utilizadorId,
        int pontos,
        string motivo,
        string chaveAcao
    )
    {
        var chave = chaveAcao.Trim();
        if (pontos <= 0 || string.IsNullOrWhiteSpace(chave))
            return false;

        if (await _transacaoRepository.ExisteChaveAcaoAsync(utilizadorId, chave))
            return false;

        await _rewardsRepository.AddOrUpdatePointsAsync(utilizadorId, pontos);
        await _transacaoRepository.AddAsync(
            new RewardTransacao
            {
                UtilizadorId = utilizadorId,
                Pontos = pontos,
                Motivo = motivo.Trim(),
                ChaveAcao = chave,
                Data = _timeProvider.GetUtcNow().UtcDateTime,
            }
        );
        await _transacaoRepository.SaveChangesAsync();

        return true;
    }
}
