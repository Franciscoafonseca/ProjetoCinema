namespace OnlineCinemaFestival.Api.Application.DTOs;

public class CarrinhoErroValidacaoDTO
{
    public int? ItemId { get; set; }

    public string Campo { get; set; } = string.Empty;

    public string Mensagem { get; set; } = string.Empty;
}
