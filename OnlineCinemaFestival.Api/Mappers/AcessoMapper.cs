using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Mappers;

public static class AcessoMapper
{
    public static Acesso MapFromCreateDTO(AcessoCreateDTO dto)
    {
        return new Acesso
        {
            Nome = dto.Nome.Trim(),
            Descricao = dto.Descricao?.Trim(),
            Tipo = dto.Tipo,
            Preco = dto.Preco,
            SessaoId = dto.SessaoId,
            FestivalId = dto.FestivalId,
            FilmeId = dto.FilmeId,
            DataAcesso = dto.DataAcesso,
            DuracaoHoras =
                dto.Tipo == TipoAcesso.AluguerDigital ? dto.DuracaoHoras ?? 48 : dto.DuracaoHoras,
            IsAtivo = true,
            CriadoEm = DateTime.UtcNow,
        };
    }

    public static AcessoReadDTO MapToReadDTO(Acesso acesso)
    {
        return new AcessoReadDTO
        {
            Id = acesso.Id,
            Nome = acesso.Nome,
            Descricao = acesso.Descricao,
            Tipo = acesso.Tipo,
            TipoNome = acesso.Tipo.ToString(),
            Preco = acesso.Preco,
            IsAtivo = acesso.IsAtivo,

            SessaoId = acesso.SessaoId,

            FestivalId = acesso.FestivalId ?? acesso.Sessao?.FestivalId,
            FestivalNome = acesso.Festival?.Name ?? acesso.Sessao?.Festival?.Name ?? string.Empty,

            // FilmeId e FilmeTitulo só representam aluguer digital individual.
            // Para bilhete de sessão, os filmes vêm da própria sessão.
            FilmeId = acesso.FilmeId,
            FilmeTitulo = acesso.Filme?.Titulo ?? string.Empty,

            DataAcesso = acesso.DataAcesso,
            DuracaoHoras = acesso.DuracaoHoras,
            CriadoEm = acesso.CriadoEm,
        };
    }

    public static void MapToExistingAcesso(AcessoUpdateDTO dto, Acesso acesso)
    {
        acesso.Nome = dto.Nome.Trim();
        acesso.Descricao = dto.Descricao?.Trim();
        acesso.Preco = dto.Preco;
        acesso.IsAtivo = dto.IsAtivo;
    }
}
