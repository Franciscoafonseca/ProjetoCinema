using Microsoft.Extensions.Logging;
using OnlineCinemaFestival.Api.Repositories;
using ModelAcesso = OnlineCinemaFestival.Api.Models.Acesso;
using OnlineCinemaFestival.Api.Models;


namespace OnlineCinemaFestival.Api.Services;

public class RewardsObserver : ICompraObserver
{
    private readonly ILogger<RewardsObserver> _logger;
    private readonly IRewardsRepository _rewardsRepository;
    private readonly IRewardTransacaoRepository _transacaoRepository;

    public RewardsObserver(
        ILogger<RewardsObserver> logger,
        IRewardsRepository rewardsRepository,
        IRewardTransacaoRepository transacaoRepository)
    {
        _logger = logger;
        _rewardsRepository = rewardsRepository;
        _transacaoRepository = transacaoRepository;
    }

    public async Task NotificarAsync(int utilizadorId, decimal valorTotal, List<ModelAcesso> acessos)
    {
        // Regra de Negócio: Sistema de Rewards (1 ponto por cada 10€)
        int pontosGanhos = (int)(valorTotal / 10);

        if (pontosGanhos > 0)
        {
            await _rewardsRepository.AddOrUpdatePointsAsync(utilizadorId, pontosGanhos);
            await _transacaoRepository.AddAsync(new RewardTransacao
            {
                UtilizadorId = utilizadorId,
                Pontos = pontosGanhos,
                Motivo = "Compra"
            });
            await _transacaoRepository.SaveChangesAsync();
            _logger.LogInformation("[REWARDS] Utilizador {UtilizadorId} ganhou {Pontos} pontos.", utilizadorId, pontosGanhos);
        }
    }
}
