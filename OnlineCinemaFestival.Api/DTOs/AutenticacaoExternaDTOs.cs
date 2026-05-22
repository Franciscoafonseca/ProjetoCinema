namespace OnlineCinemaFestival.Api.DTOs;

public class ProvedorAutenticacaoExternaDTO
{
    public string Provider { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;

    public string ClientId { get; set; } = string.Empty;

    public bool Configurado { get; set; }
}

public class PedidoAutenticacaoExternaDTO
{
    public string Provider { get; set; } = string.Empty;

    public string IdToken { get; set; } = string.Empty;

    public string AccessToken { get; set; } = string.Empty;

    public string ReturnUrl { get; set; } = string.Empty;
}
