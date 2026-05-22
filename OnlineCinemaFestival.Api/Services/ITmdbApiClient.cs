namespace OnlineCinemaFestival.Api.Services;

public interface ITmdbApiClient
{
    Task<T?> GetAsync<T>(string path);
}
