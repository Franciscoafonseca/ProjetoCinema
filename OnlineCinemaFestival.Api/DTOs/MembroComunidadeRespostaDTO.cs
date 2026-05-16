namespace OnlineCinemaFestival.Api.DTOs;

public class MembroComunidadeRespostaDTO
{
    public int UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string ProfileImageUrl { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public DateTime JoinedAt { get; set; }
}
