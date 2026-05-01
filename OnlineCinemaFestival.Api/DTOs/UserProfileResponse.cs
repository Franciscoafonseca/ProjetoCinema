namespace OnlineCinemaFestival.Api.DTOs;

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
