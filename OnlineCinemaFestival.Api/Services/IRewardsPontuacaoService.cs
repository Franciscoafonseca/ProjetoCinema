namespace OnlineCinemaFestival.Api.Services;

public interface IRewardsPontuacaoService
{
    Task<bool> AtribuirSeAindaNaoAtribuidoAsync(
        int utilizadorId,
        int pontos,
        string motivo,
        string chaveAcao
    );
}
