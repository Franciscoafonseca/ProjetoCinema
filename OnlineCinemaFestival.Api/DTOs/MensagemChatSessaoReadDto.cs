namespace OnlineCinemaFestival.Api.DTOs;

public class MensagemChatSessaoReadDTO
{
    public string Id { get; set; } = string.Empty;

    public int SessaoId { get; set; }

    public int UtilizadorId { get; set; }

    public string NomeUtilizador { get; set; } = string.Empty;

    public string Texto { get; set; } = string.Empty;

    public DateTime EnviadaEm { get; set; }

    public bool Removida { get; set; }

    public bool RemovidaPorModeracao { get; set; }
}

public class SessaoChatEntradaDTO
{
    public int SessaoId { get; set; }

    public string Grupo { get; set; } = string.Empty;

    public DateTime ChatAbertoAte { get; set; }
}

public class MensagemChatRemovidaDTO
{
    public int SessaoId { get; set; }

    public string MensagemId { get; set; } = string.Empty;

    public bool Removida { get; set; }

    public bool RemovidaPorModeracao { get; set; }
}
