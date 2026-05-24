namespace OnlineCinemaFestival.Api.Services;

public interface IPublicacaoPremiosService
{
    Task<int> PublicarResultadosPendentesAsync(CancellationToken cancellationToken = default);
}
