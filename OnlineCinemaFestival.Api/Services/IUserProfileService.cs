using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IUserProfileService
{
    Task<PerfilPrivadoDto> GetMyProfileAsync(int userId);

    Task<PerfilPrivadoDto> UpdateMyProfileAsync(int userId, UpdateProfileRequest request);

    Task<PerfilPrivadoDto> UploadProfilePhotoAsync(int userId, IFormFile ficheiro);

    Task<List<PerfilPublicoDto>> GetPublicProfilesAsync();

    Task<PerfilPublicoDto> GetPublicProfileAsync(int userId);
}
