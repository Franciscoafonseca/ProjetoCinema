using Microsoft.Extensions.Logging;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Repositories;

namespace OnlineCinemaFestival.Api.Services;

public class RewardsObserver : ICompraObserver
{
    private readonly ILogger<RewardsObserver> _logger;
    private readonly RewardsRepository _rewardsRepository;

    public RewardsObserver(ILogger<RewardsObserver> logger, RewardsRepository rewardsRepository)
    {
        _logger = logger;
        _rewardsRepository = rewardsRepository;
    }

    public async void Notificar(string utilizadorId, decimal valorTotal, List<Acesso> acessos)
    {
        // Regra de Negócio: Sistema de Rewards (1 ponto por cada 10€)
        int pontosGanhos = (int)(valorTotal / 10);

        if (pontosGanhos > 0)
        {
            await _rewardsRepository.AddOrUpdatePointsAsync(utilizadorId, pontosGanhos);
            _logger.LogInformation("[REWARDS] Utilizador {UtilizadorId} ganhou {Pontos} pontos.", utilizadorId, pontosGanhos);
        }
    }
}