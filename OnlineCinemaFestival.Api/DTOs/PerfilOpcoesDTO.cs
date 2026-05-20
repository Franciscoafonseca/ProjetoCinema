namespace OnlineCinemaFestival.Api.DTOs;

public class PaisOpcaoDTO
{
    public string Codigo { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;
}

public class PerfilOpcoesDTO
{
    public List<PaisOpcaoDTO> Paises { get; set; } = new();

    public List<string> Localidades { get; set; } = new();
}
