namespace OnlineCinemaFestival.Api.Services;

public interface IImagemUploadService
{
    Task<string> GuardarAsync(IFormFile ficheiro, string subpasta);
}
