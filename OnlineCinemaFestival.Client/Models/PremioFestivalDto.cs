namespace OnlineCinemaFestival.Client.Models;

public class CriarPremioFestivalDTO
{
    public string Nome { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public DateTime DataAberturaVotacao { get; set; }

    public DateTime DataFechoVotacao { get; set; }
}

public class PremioFestivalDTO
{
    public int Id { get; set; }

    public int FestivalId { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public DateTime DataAberturaVotacao { get; set; }

    public DateTime DataFechoVotacao { get; set; }

    public string EstadoPremio { get; set; } = string.Empty;
}

public class VotarPremioFestivalDTO
{
    public int FilmeId { get; set; }
}

public class ResultadoPremioFestivalDTO
{
    public int PremioFestivalId { get; set; }

    public string NomePremio { get; set; } = string.Empty;

    public int FestivalId { get; set; }

    public string FestivalNome { get; set; } = string.Empty;

    public int FilmeIdVencedor { get; set; }

    public string TituloFilmeVencedor { get; set; } = string.Empty;

    public string CapaUrlFilmeVencedor { get; set; } = string.Empty;

    public int TotalVotos { get; set; }

    public DateTime PublicadoEm { get; set; }

    public int PublicadoPorUtilizadorId { get; set; }
}
