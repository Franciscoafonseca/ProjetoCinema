namespace OnlineCinemaFestival.Api.Services;

public interface IPerfilFotoUploadService
{
    Task<string> GuardarAsync(IFormFile ficheiro);
}
