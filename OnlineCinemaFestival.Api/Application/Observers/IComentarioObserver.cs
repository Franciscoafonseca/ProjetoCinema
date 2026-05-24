using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Services;

public interface IComentarioObserver
{
    Task NotificarAsync(Comentario comentario);
}
