using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Mappers;

public static class AcessoUtilizadorMapper
{
    public static AcessoUtilizadorReadDTO MapToReadDTO(AcessoUtilizador acessoUtilizador)
    {
        return new AcessoUtilizadorReadDTO
        {
            Id = acessoUtilizador.Id,
            AcessoId = acessoUtilizador.AcessoId,
            NomeAcesso = acessoUtilizador.Acesso?.Nome ?? string.Empty,
            TipoAcesso = acessoUtilizador.TipoAcesso,
            TipoAcessoNome = acessoUtilizador.TipoAcesso.ToString(),
            SessaoId = acessoUtilizador.SessaoId,
            FestivalId = acessoUtilizador.FestivalId,
            FilmeId = acessoUtilizador.FilmeId,
            TituloFilme =
                acessoUtilizador.Filme?.Titulo
                ?? acessoUtilizador.Sessao?.Filme?.Titulo
                ?? acessoUtilizador.Acesso?.Filme?.Titulo
                ?? string.Empty,
            NomeFestival =
                acessoUtilizador.Festival?.Name
                ?? acessoUtilizador.Sessao?.Festival?.Name
                ?? acessoUtilizador.Acesso?.Festival?.Name
                ?? string.Empty,
            InicioSessao = acessoUtilizador.Sessao?.Inicio,
            FimSessao = acessoUtilizador.Sessao?.Fim,
            DataAcesso = acessoUtilizador.Acesso?.DataAcesso,
            DuracaoHoras = acessoUtilizador.Acesso?.DuracaoHoras,
            InicioValidade = acessoUtilizador.InicioValidade,
            FimValidade = acessoUtilizador.FimValidade,
            Ativo = acessoUtilizador.Ativo,
        };
    }
}
