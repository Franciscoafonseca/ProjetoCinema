using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public interface IVisualizacaoObserver
{
    Task NotificarAsync(Visualizacao visualizacao);
}
