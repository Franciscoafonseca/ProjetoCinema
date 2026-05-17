using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Mappers;

public static class SessaoMapper
{
    public static SessaoReadDTO MapToReadDTO(Sessao sessao)
    {
        var primeiroFilme = sessao.FilmesDaSessao.OrderBy(sf => sf.Ordem).FirstOrDefault();

        return new SessaoReadDTO
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
                .Select(sf => MapSessaoFilmeToReadDTO(sf, sessao))
                .ToList(),
        };
    }

    public static FilmeSessaoReadDTO MapSessaoFilmeToReadDTO(SessaoFilme sessaoFilme, Sessao sessao)
    {
        var horaInicio =
            sessaoFilme.HoraInicio ?? sessao.Inicio.AddSeconds(sessaoFilme.InicioOffsetSegundos);
        var horaFim =
            sessaoFilme.HoraFim
            ?? (
                sessaoFilme.DuracaoSegundos.HasValue
                    ? horaInicio.AddSeconds(sessaoFilme.DuracaoSegundos.Value)
                    : sessao.Fim
            );

        return new FilmeSessaoReadDTO
        {
            Id = sessaoFilme.FilmeId,
            Titulo = sessaoFilme.Filme?.Titulo ?? string.Empty,
            HoraInicio = horaInicio,
            HoraFim = horaFim,
            Ordem = sessaoFilme.Ordem,
            InicioOffsetSegundos = sessaoFilme.InicioOffsetSegundos,
            DuracaoSegundos = sessaoFilme.DuracaoSegundos,
            IntervaloAposSegundos = sessaoFilme.IntervaloAposSegundos,
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

    public static Sessao MapFromCreateDTO(SessaoCreateDTO dto)
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

    public static void MapToExistingSessao(SessaoUpdateDTO dto, Sessao sessao)
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
        List<SessaoFilmeCreateDTO> filmes
    )
    {
        if (filmes.Any())
        {
            var filmesDistintos = filmes
                .GroupBy(f => f.FilmeId)
                .Select(g => g.First())
                .ToList();

            var ordensUsadas = filmesDistintos
                .Where(f => f.Ordem.HasValue)
                .Select(f => f.Ordem!.Value)
                .ToHashSet();

            var proximaOrdem = 1;
            var resultado = new List<SessaoFilme>();

            foreach (var filme in filmesDistintos)
            {
                var ordem = filme.Ordem;

                if (!ordem.HasValue)
                {
                    while (ordensUsadas.Contains(proximaOrdem))
                        proximaOrdem++;

                    ordem = proximaOrdem;
                    ordensUsadas.Add(proximaOrdem);
                }

                resultado.Add(
                    new SessaoFilme
                    {
                        FilmeId = filme.FilmeId,
                        HoraInicio = filme.HoraInicio,
                        HoraFim = filme.HoraFim,
                        Ordem = ordem.Value,
                        InicioOffsetSegundos = filme.InicioOffsetSegundos,
                        DuracaoSegundos = filme.DuracaoSegundos,
                        IntervaloAposSegundos = filme.IntervaloAposSegundos,
                    }
                );
            }

            return resultado.OrderBy(sf => sf.Ordem).ToList();
        }

        return filmeIds
            .Distinct()
            .Select((filmeId, index) => new SessaoFilme { FilmeId = filmeId, Ordem = index + 1 })
            .ToList();
    }
}
