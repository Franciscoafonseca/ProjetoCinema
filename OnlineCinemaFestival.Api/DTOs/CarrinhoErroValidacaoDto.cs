namespace OnlineCinemaFestival.Api.DTOs;

public class CarrinhoErroValidacaoDto
{
    public int? ItemId { get; set; }

    public string Campo { get; set; } = string.Empty;

    public string Mensagem { get; set; } = string.Empty;
}
