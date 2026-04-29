namespace OnlineCinemaFestival.Api.Models;

public class Avaliacao
{
    public int Id {get; set;}
    public int FilmeId { get; set; }
    public Filme Filme {get; set;} // para obter os outros dados do filme
    public int UsuarioId { get; set; }
    public int Pontuacao { get; set; }
    public DateTime Data { get; set; } = DateTime.Now;

}
