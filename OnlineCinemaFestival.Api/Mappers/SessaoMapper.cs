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
            Tipo = sessao.Tipo,
            Inicio = sessao.Inicio,
            Fim = sessao.Fim,
            TemChatAoVivo = sessao.TemChatAoVivo,
            Observacoes = sessao.Observacoes,
            Filmes = sessao
                .FilmesDaSessao.OrderBy(sf => sf.Ordem)
                .Select(sf => new FilmeSessaoReadDto
                {
                    Id = sf.FilmeId,
                    Titulo = sf.Filme?.Titulo ?? string.Empty,
                    Ordem = sf.Ordem,
                })
                .ToList(),
        };
    }

    public static Sessao MapFromCreateDto(SessaoCreateDto dto)
    {
        return new Sessao
        {
            FestivalId = dto.FestivalId,
            Tipo = dto.Tipo,
            Inicio = dto.Inicio,
            Fim = dto.Fim,
            TemChatAoVivo = dto.TemChatAoVivo,
            Observacoes = dto.Observacoes?.Trim(),
            FilmesDaSessao = dto
                .FilmeIds.Distinct()
                .Select(
                    (filmeId, index) => new SessaoFilme { FilmeId = filmeId, Ordem = index + 1 }
                )
                .ToList(),
        };
    }

    public static void MapToExistingSessao(SessaoUpdateDto dto, Sessao sessao)
    {
        sessao.Tipo = dto.Tipo;
        sessao.Inicio = dto.Inicio;
        sessao.Fim = dto.Fim;
        sessao.TemChatAoVivo = dto.TemChatAoVivo;
        sessao.Observacoes = dto.Observacoes?.Trim();

        sessao.FilmesDaSessao.Clear();

        foreach (
            var item in dto.FilmeIds.Distinct().Select((filmeId, index) => new { filmeId, index })
        )
        {
            sessao.FilmesDaSessao.Add(
                new SessaoFilme
                {
                    SessaoId = sessao.Id,
                    FilmeId = item.filmeId,
                    Ordem = item.index + 1,
                }
            );
        }
    }
}
