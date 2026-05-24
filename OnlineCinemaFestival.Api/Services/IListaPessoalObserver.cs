using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public interface IListaPessoalObserver
{
    Task NotificarCriadaAsync(ListaPessoal lista);
}
