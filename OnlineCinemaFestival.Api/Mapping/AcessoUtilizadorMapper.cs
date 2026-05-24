using OnlineCinemaFestival.Api.Application.DTOs;
using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Mapping;

public static class AcessoUtilizadorMapper
{
    public static AcessoUtilizadorReadDTO MapToReadDTO(
        AcessoUtilizador acessoUtilizador,
        DateTime? agora = null
    )
    {
        var referenciaTemporal = agora ?? DateTime.UtcNow;
        var podeVisualizar =
            acessoUtilizador.Ativo
            && acessoUtilizador.InicioValidade <= referenciaTemporal
            && acessoUtilizador.FimValidade >= referenciaTemporal;

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
            PodeVisualizarAgora = podeVisualizar,
            EstadoAcesso = ObterEstadoAcesso(acessoUtilizador, referenciaTemporal),
        };
    }

    private static string ObterEstadoAcesso(AcessoUtilizador acessoUtilizador, DateTime agora)
    {
        if (!acessoUtilizador.Ativo)
            return "Inativo";

        if (acessoUtilizador.InicioValidade > agora)
            return "Ainda nao comecou";

        if (acessoUtilizador.FimValidade < agora)
            return "Expirado";

        return "Ativo";
    }
}
