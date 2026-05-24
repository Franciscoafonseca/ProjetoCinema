namespace OnlineCinemaFestival.Api.Services;

public class PerfilFotoUploadService : IPerfilFotoUploadService
{
    private readonly IImagemUploadService _imagemUploadService;

    public PerfilFotoUploadService(IImagemUploadService imagemUploadService)
    {
        _imagemUploadService = imagemUploadService;
    }

    public async Task<string> GuardarAsync(IFormFile ficheiro)
    {
        return await _imagemUploadService.GuardarAsync(ficheiro, "perfis");
    }
}
