using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Api.Models;

public class Festival
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    [MaxLength(1000)]
    public string? Premios { get; set; }

    public ICollection<FestivalFilme> FestivalFilmes { get; set; } = new List<FestivalFilme>();

    public ICollection<Sessao> Sessoes { get; set; } = new List<Sessao>();

    public ICollection<Acesso> Acessos { get; set; } = new List<Acesso>();

    public ICollection<AcessoUtilizador> AcessosUtilizador { get; set; } =
        new List<AcessoUtilizador>();

    public ICollection<Visualizacao> Visualizacoes { get; set; } = new List<Visualizacao>();

    public ICollection<PremioFestival> PremiosFestival { get; set; } =
        new List<PremioFestival>();

    public ICollection<VotoPremioFestival> VotosPremiosFestival { get; set; } =
        new List<VotoPremioFestival>();
}
