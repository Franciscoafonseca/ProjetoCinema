using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Services;

public interface IAcessoAutomaticoService
{
    Task GarantirParaFilmeAsync(int filmeId);

    Task GarantirParaFestivalAsync(Festival festival);

    Task GarantirParaSessaoAsync(int sessaoId);

    Task GarantirParaAssociacaoAsync(Festival festival, Filme filme);
}
