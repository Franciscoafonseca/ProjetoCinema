using OnlineCinemaFestival.Api.Domain;
using OnlineCinemaFestival.Api.Services;
using OnlineCinemaFestival.Tests.Support;
using OnlineCinemaFestival.Tests.Support.Builders;
using OnlineCinemaFestival.Tests.Support.Fakes;

namespace OnlineCinemaFestival.Tests.Visualizacoes;

public class ChatTemporalTests
{
    private static readonly DateTimeOffset Agora = new(2026, 5, 24, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task EntrarNaSessao_DuranteJanelaAtiva_DevolveSessao()
    {
        var service = CriarChatService(CriarSessao(Agora.AddMinutes(-5), Agora.AddMinutes(5)), Agora);

        var entrada = await service.EntrarNaSessaoAsync(20, 7, administrador: false);

        Assert.Equal(20, entrada.SessaoId);
    }

    [Fact]
    public async Task EntrarNaSessao_AposFimDaSessao_Rejeita()
    {
        var service = CriarChatService(CriarSessao(Agora.AddMinutes(-20), Agora.AddMinutes(-1)), Agora);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.EntrarNaSessaoAsync(20, 7, administrador: false)
        );
    }

    [Fact]
    public async Task EnviarMensagem_AposFimDaSessao_Rejeita()
    {
        var service = CriarChatService(CriarSessao(Agora.AddMinutes(-20), Agora.AddMinutes(-1)), Agora);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.EnviarMensagemAsync(20, 7, administrador: false, "Ola comunidade")
        );
    }

    private static ChatSessaoService CriarChatService(Sessao sessao, DateTimeOffset agora)
    {
        var utilizador = new UtilizadorBuilder().ComId(7).ComNome("Utilizador").Build();

        return new ChatSessaoService(
            new MensagemChatSessaoRepositoryFalso(sessao),
            new AcessoVisualizacaoServiceFalso(),
            new UtilizadorRepositoryFalso(utilizador),
            new FakeTimeProvider(agora)
        );
    }

    private static Sessao CriarSessao(DateTimeOffset inicio, DateTimeOffset fim)
    {
        var filme = new FilmeBuilder().ComId(10).ComTitulo("Filme").Build();
        var festival = new FestivalBuilder().ComId(99).ComNome("Festival").Build();
        return new SessaoBuilder()
            .ComId(20)
            .DoFilme(filme)
            .NoFestival(festival)
            .Entre(inicio.UtcDateTime, fim.UtcDateTime)
            .ComChatAoVivo()
            .Build();
    }
}
