using Microsoft.Extensions.Logging;
using OnlineCinemaFestival.Api.Repositories;
using ModelAcesso = OnlineCinemaFestival.Api.Domain.Acesso;
using OnlineCinemaFestival.Api.Domain;


namespace OnlineCinemaFestival.Api.Services;

public class RewardsObserver : ICompraObserver
{
    private readonly ILogger<RewardsObserver> _logger;
    private readonly IRewardsPontuacaoService _rewardsPontuacaoService;

    public RewardsObserver(
        ILogger<RewardsObserver> logger,
        IRewardsPontuacaoService rewardsPontuacaoService)
    {
        _logger = logger;
        _rewardsPontuacaoService = rewardsPontuacaoService;
    }

    public async Task NotificarAsync(int utilizadorId, decimal valorTotal, List<ModelAcesso> acessos)
    {
        // Regra de Negócio: Sistema de Rewards (1 ponto por cada 10€)
        int pontosGanhos = (int)(valorTotal / 10);

        if (pontosGanhos > 0)
        {
            var atribuido = await _rewardsPontuacaoService.AtribuirSeAindaNaoAtribuidoAsync(
                utilizadorId,
                pontosGanhos,
                "Compra",
                $"compra:{string.Join(',', acessos.Select(a => a.Id).OrderBy(id => id))}:{valorTotal:0.00}"
            );

            if (atribuido)
                _logger.LogInformation("[REWARDS] Utilizador {UtilizadorId} ganhou {Pontos} pontos.", utilizadorId, pontosGanhos);
        }
    }
}
