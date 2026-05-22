using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public interface IAvaliacaoObserver
{
    Task NotificarAsync(Avaliacao avaliacao);
}
