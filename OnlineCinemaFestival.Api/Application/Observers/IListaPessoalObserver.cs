using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Services;

public interface IListaPessoalObserver
{
    Task NotificarCriadaAsync(ListaPessoal lista);
}
