using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Services;

public interface IVotoPremioObserver
{
    Task NotificarAsync(VotoPremioFestival voto);
}
