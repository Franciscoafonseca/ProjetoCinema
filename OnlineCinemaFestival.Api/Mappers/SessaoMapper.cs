using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Mappers;

public static class SessaoMapper
{
    public static SessaoReadDTO MapToReadDTO(Sessao sessao)
    {
        return new SessaoReadDTO
        {
            Id = sessao.Id,
            FestivalId = sessao.FestivalId,
            FestivalName = sessao.Festival?.Name ?? string.Empty,
            NomeFestival = sessao.Festival?.Name ?? string.Empty,
            FilmeId = sessao.FilmeId,
            TituloFilme = sessao.Filme?.Titulo ?? string.Empty,
            FilmeTitulo = sessao.Filme?.Titulo ?? string.Empty,
            Tipo = sessao.Tipo,
            TipoNome = sessao.Tipo.ToString(),
            Inicio = sessao.Inicio,
            Fim = sessao.Fim,
            Estado = ObterEstado(sessao.Inicio, sessao.Fim),
            TemChatAoVivo = sessao.TemChatAoVivo,
            PrecoBilhete = sessao
                .Acessos.Where(a => a.IsAtivo && a.Tipo == TipoAcesso.BilheteSessao)
                .OrderBy(a => a.Preco)
                .Select(a => (decimal?)a.Preco)
                .FirstOrDefault(),
            Observacoes = sessao.Observacoes,
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
            FilmeId = dto.FilmeId,
            Tipo = dto.Tipo,
            Inicio = dto.Inicio,
            Fim = dto.Fim,
            TemChatAoVivo = dto.TemChatAoVivo,
            Observacoes = dto.Observacoes?.Trim(),
        };
    }

    public static void MapToExistingSessao(SessaoUpdateDTO dto, Sessao sessao)
    {
        sessao.FilmeId = dto.FilmeId;
        sessao.Tipo = dto.Tipo;
        sessao.Inicio = dto.Inicio;
        sessao.Fim = dto.Fim;
        sessao.TemChatAoVivo = dto.TemChatAoVivo;
        sessao.Observacoes = dto.Observacoes?.Trim();
    }
}
