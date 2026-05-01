namespace OnlineCinemaFestival.Api.Models;

public class ListaPessoalItem
{
    public int ListaPessoalId { get; set; }

    public ListaPessoal ListaPessoal { get; set; } = null!;

    public int FilmeId { get; set; }

    public Filme Filme { get; set; } = null!;

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
