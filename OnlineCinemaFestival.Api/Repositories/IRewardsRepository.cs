namespace OnlineCinemaFestival.Api.Repositories;

public interface IRewardsRepository
{
    Task AddOrUpdatePointsAsync(int utilizadorId, int pontosGanhos);
}
