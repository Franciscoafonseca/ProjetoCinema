using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Mappers;

public static class AcessoUtilizadorMapper
{
    public static AcessoUtilizadorReadDto MapToReadDto(AcessoUtilizador acessoUtilizador)
    {
        return new AcessoUtilizadorReadDto
        {
            Id = acessoUtilizador.Id,
            AcessoId = acessoUtilizador.AcessoId,
            NomeAcesso = acessoUtilizador.Acesso?.Nome ?? string.Empty,
            TipoAcesso = acessoUtilizador.TipoAcesso,
            TipoAcessoNome = acessoUtilizador.TipoAcesso.ToString(),
            SessaoId = acessoUtilizador.SessaoId,
            FestivalId = acessoUtilizador.FestivalId,
            FilmeId = acessoUtilizador.FilmeId,
            InicioValidade = acessoUtilizador.InicioValidade,
            FimValidade = acessoUtilizador.FimValidade,
            Ativo = acessoUtilizador.Ativo,
        };
    }
}
