using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Mappers;

public static class ListaPessoalMapper
{
    public static ListaPessoalReadDTO MapToReadDTO(ListaPessoal lista)
    {
        return new ListaPessoalReadDTO
        {
            Id = lista.Id,
            UtilizadorId = lista.UtilizadorId,
            Name = lista.Name,
            Description = lista.Description,
            Tipo = lista.Tipo,
            TipoNome = ObterNomeTipo(lista.Tipo),
            IsPublic = lista.IsPublic,
            CreatedAt = lista.CreatedAt,
            UpdatedAt = lista.UpdatedAt,
            TotalFilmes = lista.Items?.Count ?? 0,
            Items = lista.Items?.Select(MapToItemReadDTO).ToList() ?? new(),
        };
    }

    public static ListaPessoalItemReadDTO MapToItemReadDTO(ListaPessoalItem item)
    {
        return new ListaPessoalItemReadDTO
        {
            FilmeId = item.FilmeId,
            Titulo = item.Filme?.Titulo ?? string.Empty,
            Genero = item.Filme?.Genero,
            CapaUrl = item.Filme?.CapaUrl ?? string.Empty,
            AddedAt = item.AddedAt,
        };
    }

    public static ListaPessoal MapFromCreateDTO(ListaPessoalCreateDTO dto, int utilizadorId)
    {
        return new ListaPessoal
        {
            UtilizadorId = utilizadorId,
            Name = dto.Name.Trim(),
            Description = dto.Description.Trim(),
            Tipo = dto.Tipo,
            IsPublic = dto.IsPublic,
            CreatedAt = DateTime.UtcNow,
        };
    }

    private static string ObterNomeTipo(TipoListaPessoal tipo) =>
        tipo switch
        {
            TipoListaPessoal.Watchlist => "Quero ver",
            TipoListaPessoal.Watched => "Vistos",
            TipoListaPessoal.Favorites => "Favoritos",
            TipoListaPessoal.Custom => "Personalizada",
            _ => tipo.ToString(),
        };
}
