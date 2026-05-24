using OnlineCinemaFestival.Api.Mapping;
using OnlineCinemaFestival.Api.Domain;
using OnlineCinemaFestival.Api.Services;
using OnlineCinemaFestival.Tests.Support;
using OnlineCinemaFestival.Tests.Support.Builders;
using OnlineCinemaFestival.Tests.Support.Fakes;

namespace OnlineCinemaFestival.Tests.Premios;

public class VencedorAutomaticoTests
{
    private static readonly DateTimeOffset Agora = new(2026, 5, 24, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task PublicarResultadosPendentes_ComPremioTerminado_PublicaVencedor()
    {
        var premio = CriarPremio(Agora.AddHours(-2), Agora.AddHours(-1));
        var repo = new PremioFestivalRepositoryFalso(premio)
        {
            Votos =
            {
                new VotoPremioFestival { PremioFestivalId = premio.Id, FilmeId = 10, UtilizadorId = 1 },
                new VotoPremioFestival { PremioFestivalId = premio.Id, FilmeId = 10, UtilizadorId = 2 },
                new VotoPremioFestival { PremioFestivalId = premio.Id, FilmeId = 11, UtilizadorId = 3 },
            },
        };
        var service = new PublicacaoPremiosService(repo, new FakeTimeProvider(Agora));

        var publicados = await service.PublicarResultadosPendentesAsync();

        Assert.Equal(1, publicados);
        Assert.Equal(EstadoPremio.Publicado, premio.EstadoPremio);
        Assert.Equal(10, premio.Resultado?.FilmeIdVencedor);
    }

    [Fact]
    public void MapearFilmeVencedor_ParaDto_IncluiPremioPublicado()
    {
        var premio = CriarPremio(Agora.AddHours(-2), Agora.AddHours(-1));
        premio.EstadoPremio = EstadoPremio.Publicado;
        var resultado = new ResultadoPremioFestival
        {
            PremioFestival = premio,
            PremioFestivalId = premio.Id,
            FilmeIdVencedor = 10,
            TotalVotos = 4,
            PublicadoEm = Agora.UtcDateTime,
        };
        var filme = new FilmeBuilder().ComId(10).ComTitulo("Filme vencedor").Build();
        filme.ResultadosPremiosFestival.Add(resultado);
        resultado.FilmeVencedor = filme;

        var dto = FilmeMapper.MapToReadDTO(filme);

        var premioDto = Assert.Single(dto.ResultadosPremiosPublicados);
        Assert.Equal(premio.Nome, premioDto.NomePremio);
    }

    private static PremioFestival CriarPremio(DateTimeOffset inicio, DateTimeOffset fim)
    {
        var festival = new FestivalBuilder().ComId(99).ComNome("Festival").Build();
        return new PremioBuilder()
            .ComId(1)
            .NoFestival(festival)
            .ComNome("Premio do publico")
            .ComVotacao(inicio.UtcDateTime, fim.UtcDateTime)
            .NoEstado(EstadoPremio.Aberto)
            .Build();
    }
}
