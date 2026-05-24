using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public interface IVotoPremioObserver
{
    Task NotificarAsync(VotoPremioFestival voto);
}
