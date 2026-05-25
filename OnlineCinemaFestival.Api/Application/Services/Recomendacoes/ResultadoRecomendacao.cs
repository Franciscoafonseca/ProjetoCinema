using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Services;

public sealed record ResultadoRecomendacao(Filme Filme, decimal Pontuacao, string Motivo);
