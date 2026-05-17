namespace OnlineCinemaFestival.Api.Models;

public class MensagemChatSessao
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    public int SessaoId { get; set; }
    public Sessao Sessao { get; set; } = null!;

    public int UtilizadorId { get; set; }
    public Utilizador Utilizador { get; set; } = null!;

    public string Texto { get; set; } = string.Empty;

    public DateTime EnviadaEm { get; set; } = DateTime.UtcNow;

    public bool Removida { get; set; }

    public bool RemovidaPorModeracao { get; set; }
}
