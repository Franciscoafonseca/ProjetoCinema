using System.ComponentModel.DataAnnotations;
using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Application.DTOs;

public class CarrinhoItemCreateDTO
{
    private DateTime? _dataAcesso;

    [Required]
    public TipoAcesso TipoAcesso { get; set; }

    public int? FestivalId { get; set; }

    public int? FilmeId { get; set; }

    public int? SessaoId { get; set; }

    public DateTime? DataAcesso
    {
        get => _dataAcesso;
        set => _dataAcesso = value;
    }

    public DateTime? DataPasse
    {
        get => _dataAcesso;
        set => _dataAcesso = value;
    }

    [Range(1, 99, ErrorMessage = "A quantidade deve estar entre 1 e 99.")]
    public int Quantidade { get; set; } = 1;
}
