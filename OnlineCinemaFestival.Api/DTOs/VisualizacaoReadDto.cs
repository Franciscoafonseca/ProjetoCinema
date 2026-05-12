namespace OnlineCinemaFestival.Api.DTOs;

public class VisualizacaoReadDto
{
    public string TipoConteudo { get; set; } = string.Empty;

    public int? FilmeId { get; set; }

    public int? SessaoId { get; set; }

    public bool TemChatAoVivo { get; set; }

    public string Mensagem { get; set; } = string.Empty;

    public List<ConteudoVisualizacaoDto> Conteudos { get; set; } = new();
}
