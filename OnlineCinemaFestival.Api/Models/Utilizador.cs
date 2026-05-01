using System.ComponentModel.DataAnnotations;

namespace OnlineCinemaFestival.Api.Models;

public class Utilizador
{
    public int Id { get; set; }

    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(180)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.User;

    public bool IsActive { get; set; } = true;

    [MaxLength(80)]
    public string Nationality { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public PerfilUtilizador? Perfil { get; set; }

    public ICollection<UtilizadorGeneroFavorito> GenerosFavoritos { get; set; } =
        new List<UtilizadorGeneroFavorito>();

    public ICollection<Avaliacao> Avaliacoes { get; set; } = new List<Avaliacao>();

    public ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();

    public ICollection<ListaPessoal> ListasPessoais { get; set; } = new List<ListaPessoal>();

    public ICollection<ComunidadeMembro> Comunidades { get; set; } = new List<ComunidadeMembro>();
}
