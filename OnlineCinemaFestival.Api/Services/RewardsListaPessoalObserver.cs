using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public class RewardsListaPessoalObserver : IListaPessoalObserver
{
    private readonly IRewardsPontuacaoService _rewardsPontuacaoService;

    public RewardsListaPessoalObserver(IRewardsPontuacaoService rewardsPontuacaoService)
    {
        _rewardsPontuacaoService = rewardsPontuacaoService;
    }

    public Task NotificarCriadaAsync(ListaPessoal lista)
    {
        return _rewardsPontuacaoService.AtribuirSeAindaNaoAtribuidoAsync(
            lista.UtilizadorId,
            1,
            "Criacao de lista",
            $"lista:{lista.Id}"
        );
    }
}
