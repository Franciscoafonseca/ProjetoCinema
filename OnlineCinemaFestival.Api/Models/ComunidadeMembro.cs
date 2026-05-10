namespace OnlineCinemaFestival.Api.Models;

public class ComunidadeMembro
{
    public int ComunidadeId { get; set; }

    public Comunidade Comunidade { get; set; } = null!;

    public int UtilizadorId { get; set; }

    public Utilizador Utilizador { get; set; } = null!;

    public CommunityMemberRole Role { get; set; } = CommunityMemberRole.Member;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
