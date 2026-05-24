using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Client.Models;

public class ComunidadeDTO
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public bool IsPublic { get; set; }

    public bool IsOwner { get; set; }

    public int? CreatedByUserId { get; set; }

    public string? CreatedByUserName { get; set; }

    public DateTime CreatedAt { get; set; }

    public int MembersCount { get; set; }

    public int ComentariosCount { get; set; }

    public string? CodigoConvite { get; set; }

    public List<MembroComunidadeDTO> Members { get; set; } = new();
}

public class MembroComunidadeDTO
{
    public int UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string ProfileImageUrl { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public DateTime JoinedAt { get; set; }
}

public class ComunidadeCreateDTO
{
    [Required(ErrorMessage = "O nome da comunidade e obrigatorio.")]
    [StringLength(
        120,
        MinimumLength = 3,
        ErrorMessage = "O nome deve ter entre 3 e 120 caracteres."
    )]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "A descricao nao pode exceder 500 caracteres.")]
    public string Description { get; set; } = string.Empty;

    [StringLength(300, ErrorMessage = "O URL da imagem nao pode exceder 300 caracteres.")]
    public string ImageUrl { get; set; } = string.Empty;

    public bool IsPublic { get; set; } = true;
}

public class ComentarioDTO
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public string NomeUsuario { get; set; } = string.Empty;

    public string? UsuarioFotoUrl { get; set; }

    public bool UsuarioPerfilPublico { get; set; }

    public int? ComunidadeId { get; set; }

    public string? NomeComunidade { get; set; }

    public int? FilmeId { get; set; }

    public string? TituloFilme { get; set; }

    public string? FilmeCapaUrl { get; set; }

    public string Texto { get; set; } = string.Empty;

    public DateTime CriadoEm { get; set; }

    public bool Visivel { get; set; }

    public bool Reportado { get; set; }

    public EstadoModeracaoComentario EstadoModeracao { get; set; }
}

public class ComentarioCreateDTO
{
    [Required(ErrorMessage = "Escreve um comentario.")]
    [StringLength(
        600,
        MinimumLength = 3,
        ErrorMessage = "O comentario deve ter entre 3 e 600 caracteres."
    )]
    public string Texto { get; set; } = string.Empty;

    public int? FilmeId { get; set; }
}

public class ComentarioVisibilidadeDTO
{
    public bool Visivel { get; set; }
}
