using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Api.Models;

public class Sessao
{
    public int Id { get; set; }

    public int FestivalId { get; set; }
    public Festival Festival { get; set; } = null!;

    public TipoSessao Tipo { get; set; }

    public DateTime Inicio { get; set; }

    public DateTime Fim { get; set; }

    public bool TemChatAoVivo { get; set; } = true;

    [MaxLength(500)]
    public string? Observacoes { get; set; }

    public ICollection<SessaoFilme> FilmesDaSessao { get; set; } = new List<SessaoFilme>();

    public ICollection<Acesso> Acessos { get; set; } = new List<Acesso>();

    public ICollection<AcessoUtilizador> AcessosUtilizador { get; set; } =
        new List<AcessoUtilizador>();

    public ICollection<Visualizacao> Visualizacoes { get; set; } = new List<Visualizacao>();

    public ICollection<MensagemChatSessao> MensagensChat { get; set; } =
        new List<MensagemChatSessao>();
}
