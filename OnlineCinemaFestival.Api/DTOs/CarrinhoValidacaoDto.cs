namespace OnlineCinemaFestival.Api.DTOs;

public class CarrinhoValidacaoDto
{
    public bool Valido => !Erros.Any();

    public decimal Total { get; set; }

    public List<CarrinhoErroValidacaoDto> Erros { get; set; } = new();

    public List<string> Avisos { get; set; } = new();
}
