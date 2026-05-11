namespace OnlineCinemaFestival.Api.Models;

public class SessaoFilme
{
    public int SessaoId { get; set; }
    public Sessao Sessao { get; set; } = null!;

    public int FilmeId { get; set; }
    public Filme Filme { get; set; } = null!;

    public int Ordem { get; set; }
}
