namespace OnlineCinemaFestival.Api.Repositories;

public interface IRewardsRepository
{
    Task AddOrUpdatePointsAsync(string utilizadorId, int pontosGanhos);
}
