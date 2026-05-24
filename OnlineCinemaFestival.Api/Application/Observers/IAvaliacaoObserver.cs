using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Services;

public interface IAvaliacaoObserver
{
    Task NotificarAsync(Avaliacao avaliacao);
}
