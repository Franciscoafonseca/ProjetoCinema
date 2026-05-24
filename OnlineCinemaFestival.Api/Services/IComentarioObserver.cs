using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public interface IComentarioObserver
{
    Task NotificarAsync(Comentario comentario);
}
