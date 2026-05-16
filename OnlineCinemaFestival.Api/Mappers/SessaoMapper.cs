using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Mappers;

public static class SessaoMapper
{
    public static SessaoReadDto MapToReadDto(Sessao sessao)
    {
        var primeiroFilme = sessao.FilmesDaSessao.OrderBy(sf => sf.Ordem).FirstOrDefault();

        return new SessaoReadDto
        {
            Id = sessao.Id,
            FestivalId = sessao.FestivalId,
            FestivalName = sessao.Festival?.Name ?? string.Empty,
            NomeFestival = sessao.Festival?.Name ?? string.Empty,
            FilmeId = primeiroFilme?.FilmeId,
            TituloFilme = primeiroFilme?.Filme?.Titulo ?? string.Empty,
            Tipo = sessao.Tipo,
            TipoNome = sessao.Tipo.ToString(),
            Inicio = sessao.Inicio,
            Fim = sessao.Fim,
            Estado = ObterEstado(sessao.Inicio, sessao.Fim),
            TemChatAoVivo = sessao.TemChatAoVivo,
            Observacoes = sessao.Observacoes,
            Filmes = sessao
                .FilmesDaSessao.OrderBy(sf => sf.Ordem)
                .Select(sf => new FilmeSessaoReadDto
                {
                    Id = sf.FilmeId,
                    Titulo = sf.Filme?.Titulo ?? string.Empty,
                    HoraInicio = sf.HoraInicio ?? sessao.Inicio,
                    HoraFim = sf.HoraFim ?? sessao.Fim,
                    Ordem = sf.Ordem,
                })
                .ToList(),
        };
    }

    public static string ObterEstado(DateTime inicio, DateTime fim)
    {
        var agora = DateTime.UtcNow;

        if (agora < inicio)
            return "Agendada";

        if (agora <= fim)
            return "ADecorrer";

        return "Terminada";
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
            FilmesDaSessao = ObterFilmesSessao(dto.FilmeIds, dto.Filmes),
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

        foreach (var item in ObterFilmesSessao(dto.FilmeIds, dto.Filmes))
        {
            item.SessaoId = sessao.Id;
            sessao.FilmesDaSessao.Add(
                item
            );
        }
    }

    private static List<SessaoFilme> ObterFilmesSessao(
        List<int> filmeIds,
        List<SessaoFilmeCreateDto> filmes
    )
    {
        if (filmes.Any())
        {
            return filmes
                .GroupBy(f => f.FilmeId)
                .Select(g => g.First())
                .Select(
                    (filme, index) => new SessaoFilme
                    {
                        FilmeId = filme.FilmeId,
                        HoraInicio = filme.HoraInicio,
                        HoraFim = filme.HoraFim,
                        Ordem = filme.Ordem ?? index + 1,
                    }
                )
                .OrderBy(sf => sf.Ordem)
                .ToList();
        }

        return filmeIds
            .Distinct()
            .Select((filmeId, index) => new SessaoFilme { FilmeId = filmeId, Ordem = index + 1 })
            .ToList();
    }
}
