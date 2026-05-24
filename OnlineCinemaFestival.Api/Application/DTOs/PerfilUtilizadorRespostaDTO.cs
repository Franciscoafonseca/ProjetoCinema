namespace OnlineCinemaFestival.Api.Application.DTOs;

public class PerfilPublicoDTO
{
    public int UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Nationality { get; set; } = string.Empty;

    public string CountryCode { get; set; } = string.Empty;

    public string CountryFlag { get; set; } = string.Empty;

    public string Bio { get; set; } = string.Empty;

    public string ProfileImageUrl { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public bool IsPublic { get; set; }

    public List<string> FavoriteGenres { get; set; } = new();

    public int ReviewsCount { get; set; }

    public int CommunitiesCount { get; set; }

    public int PublicListsCount { get; set; }

    public List<ListaPessoalPublicaDTO> PublicLists { get; set; } = new();

    public List<ReviewPublicaDTO> PublicReviews { get; set; } = new();

    public List<ComunidadePublicaPerfilDTO> PublicCommunities { get; set; } = new();
}

public class PerfilPrivadoDTO : PerfilPublicoDTO
{
    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;
}

public class PerfilUtilizadorRespostaDTO : PerfilPrivadoDTO { }

public class ListaPessoalPublicaDTO
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int TotalFilmes { get; set; }
}

public class ReviewPublicaDTO
{
    public int Id { get; set; }

    public int FilmeId { get; set; }

    public string FilmeTitulo { get; set; } = string.Empty;

    public int Pontuacao { get; set; }

    public string Texto { get; set; } = string.Empty;

    public DateTime Data { get; set; }
}

public class ComunidadePublicaPerfilDTO
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public int MembersCount { get; set; }
}
