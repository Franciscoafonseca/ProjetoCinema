using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public class RewardsComentarioObserver : IComentarioObserver
{
    private readonly IRewardsPontuacaoService _rewardsPontuacaoService;

    public RewardsComentarioObserver(IRewardsPontuacaoService rewardsPontuacaoService)
    {
        _rewardsPontuacaoService = rewardsPontuacaoService;
    }

    public Task NotificarAsync(Comentario comentario)
    {
        return _rewardsPontuacaoService.AtribuirSeAindaNaoAtribuidoAsync(
            comentario.UsuarioId,
            1,
            "Comentario",
            $"comentario:{comentario.Id}"
        );
    }
}
