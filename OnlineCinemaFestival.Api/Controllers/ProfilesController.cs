using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/profiles")]
public class ProfilesController : ControllerBase
{
    private readonly IUserProfileService _profileService;

    public ProfilesController(IUserProfileService profileService)
    {
        _profileService = profileService;
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserProfileResponse>> GetMyProfile()
    {
        var userId = GetCurrentUserId();

        if (userId == null)
            return Unauthorized();

        return Ok(await _profileService.GetMyProfileAsync(userId.Value));
    }

    [Authorize]
    [HttpPut("me")]
    public async Task<ActionResult<UserProfileResponse>> UpdateMyProfile(
        UpdateProfileRequest request
    )
    {
        var userId = GetCurrentUserId();

        if (userId == null)
            return Unauthorized();

        return Ok(await _profileService.UpdateMyProfileAsync(userId.Value, request));
    }

    [HttpGet("public")]
    public async Task<ActionResult<List<UserProfileResponse>>> GetPublicProfiles()
    {
        return Ok(await _profileService.GetPublicProfilesAsync());
    }

    [HttpGet("{userId:int}")]
    public async Task<ActionResult<UserProfileResponse>> GetPublicProfile(int userId)
    {
        try
        {
            return Ok(await _profileService.GetPublicProfileAsync(userId));
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    private int? GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (int.TryParse(value, out var userId))
            return userId;

        return null;
    }
}
