namespace OnlineCinemaFestival.Api.DTOs;

public class ComentarioDTO
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public string NomeUsuario { get; set; } = string.Empty;

    public int? ComunidadeId { get; set; }

    public string? NomeComunidade { get; set; }

    public int? FilmeId { get; set; }

    public string? TituloFilme { get; set; }

    public string Texto { get; set; } = string.Empty;

    public DateTime CriadoEm { get; set; }

    public bool Visivel { get; set; }

    public bool Reportado { get; set; }
}

public class ComentarioReadDTO : ComentarioDTO { }
