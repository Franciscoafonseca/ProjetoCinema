using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Services;

public interface IVisualizacaoObserver
{
    Task NotificarAsync(Visualizacao visualizacao);
}
