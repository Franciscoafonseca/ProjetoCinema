namespace OnlineCinemaFestival.Api.DTOs;

public class FilmeRecomendadoDTO
{
    public FilmeReadDTO Filme { get; set; } = new();

    public string Motivo { get; set; } = string.Empty;

    public decimal Pontuacao { get; set; }
}
