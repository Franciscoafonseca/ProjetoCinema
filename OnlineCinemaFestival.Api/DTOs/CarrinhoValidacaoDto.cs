namespace OnlineCinemaFestival.Api.DTOs;

public class CarrinhoValidacaoDTO
{
    public bool Valido => !Erros.Any();

    public decimal Total { get; set; }

    public List<CarrinhoErroValidacaoDTO> Erros { get; set; } = new();

    public List<string> Avisos { get; set; } = new();
}
