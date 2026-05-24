namespace OnlineCinemaFestival.Api.Services.PoliticasAcesso;

public sealed record ContextoVisualizacao(
    int? FilmeId,
    int? SessaoId,
    int? FestivalId,
    DateTime? InicioSessao,
    DateTime? FimSessao,
    IReadOnlySet<int> FestivalIdsDoFilme
);
