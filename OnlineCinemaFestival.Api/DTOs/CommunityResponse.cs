namespace OnlineCinemaFestival.Api.DTOs;

public class CommunityResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public bool IsPublic { get; set; }

    public int CreatedByUserId { get; set; }

    public string CreatedByUserName { get; set; } = string.Empty;

    public int MembersCount { get; set; }

    public DateTime CreatedAt { get; set; }
}
