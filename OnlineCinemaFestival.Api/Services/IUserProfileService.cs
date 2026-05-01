using OnlineCinemaFestival.Api.DTOs;

namespace OnlineCinemaFestival.Api.Services;

public interface IUserProfileService
{
    Task<UserProfileResponse> GetMyProfileAsync(int userId);

    Task<UserProfileResponse> UpdateMyProfileAsync(int userId, UpdateProfileRequest request);

    Task<List<UserProfileResponse>> GetPublicProfilesAsync();

    Task<UserProfileResponse> GetPublicProfileAsync(int userId);
}
