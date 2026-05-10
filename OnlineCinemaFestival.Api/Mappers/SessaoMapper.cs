using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Mappers;

public static class SessaoMapper
{
    public static SessaoReadDto MapToReadDto(Sessao sessao)
    {
        return new SessaoReadDto
        {
            Id = sessao.Id,
            FestivalId = sessao.FestivalId,
            FestivalName = sessao.Festival?.Name ?? string.Empty,
            FilmeId = sessao.FilmeId,
            FilmeTitulo = sessao.Filme?.Titulo ?? string.Empty,
            Tipo = sessao.Tipo,
            Inicio = sessao.Inicio,
            Fim = sessao.Fim,
            Observacoes = sessao.Observacoes,
        };
    }

    public static Sessao MapFromCreateDto(SessaoCreateDto dto)
    {
        return new Sessao
        {
            FestivalId = dto.FestivalId,
            FilmeId = dto.FilmeId,
            Tipo = dto.Tipo,
            Inicio = dto.Inicio,
            Fim = dto.Fim,
            Observacoes = dto.Observacoes?.Trim(),
        };
    }

    public static void MapToExistingSessao(SessaoUpdateDto dto, Sessao sessao)
    {
        sessao.Tipo = dto.Tipo;
        sessao.Inicio = dto.Inicio;
        sessao.Fim = dto.Fim;
        sessao.Observacoes = dto.Observacoes?.Trim();
    }
}
