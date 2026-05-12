using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Client.Models;

public class UserProfileResponse
{
    public int UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Nationality { get; set; } = string.Empty;

    public string Bio { get; set; } = string.Empty;

    public string ProfileImageUrl { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public bool IsPublic { get; set; }

    public List<string> FavoriteGenres { get; set; } = new();

    public int ReviewsCount { get; set; }

    public int CommunitiesCount { get; set; }

    public int PublicListsCount { get; set; }
}

public class UpdateProfileRequest
{
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(80)]
    public string Nationality { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Bio { get; set; } = string.Empty;

    [MaxLength(300)]
    public string ProfileImageUrl { get; set; } = string.Empty;

    [MaxLength(120)]
    public string Location { get; set; } = string.Empty;

    public bool IsPublic { get; set; } = true;

    public List<int> FavoriteGenreIds { get; set; } = new();
}
