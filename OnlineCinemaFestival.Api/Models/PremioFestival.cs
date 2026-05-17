using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Api.Models;

public class PremioFestival
{
    public int Id { get; set; }

    public int FestivalId { get; set; }
    public Festival Festival { get; set; } = null!;

    [Required]
    [MaxLength(150)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Descricao { get; set; } = string.Empty;

    public DateTime DataAberturaVotacao { get; set; }

    public DateTime DataFechoVotacao { get; set; }

    public EstadoPremio EstadoPremio { get; set; } = EstadoPremio.Rascunho;

    public ICollection<VotoPremioFestival> Votos { get; set; } =
        new List<VotoPremioFestival>();

    public ResultadoPremioFestival? Resultado { get; set; }
}
