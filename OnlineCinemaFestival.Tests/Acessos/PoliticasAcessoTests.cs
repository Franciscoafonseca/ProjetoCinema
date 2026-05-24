using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Services.PoliticasAcesso;

namespace OnlineCinemaFestival.Tests.Acessos;

public class PoliticasAcessoTests
{
    private static readonly DateTime Agora = new(2026, 5, 23, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void PoliticaAluguerDigital_DevePermitir_FilmeCorreto()
    {
        var politica = new PoliticaAluguerDigital();
        var acesso = CriarAcesso(TipoAcesso.AluguerDigital, filmeId: 10);
        var contexto = ContextoFilme(10);

        Assert.True(politica.PermiteVisualizacao(acesso, contexto, Agora));
    }

    [Fact]
    public void PoliticaAluguerDigital_DeveRecusar_AcessoExpirado()
    {
        var politica = new PoliticaAluguerDigital();
        var acesso = CriarAcesso(
            TipoAcesso.AluguerDigital,
            filmeId: 10,
            inicio: Agora.AddHours(-2),
            fim: Agora.AddMinutes(-1)
        );
        var contexto = ContextoFilme(10);

        Assert.False(politica.PermiteVisualizacao(acesso, contexto, Agora));
    }

    [Fact]
    public void PoliticaBilheteSessao_DevePermitir_SessaoCorreta()
    {
        var politica = new PoliticaBilheteSessao();
        var acesso = CriarAcesso(TipoAcesso.BilheteSessao, filmeId: 10, sessaoId: 5, festivalId: 2);
        var contexto = ContextoSessao(filmeId: 10, sessaoId: 5, festivalId: 2);

        Assert.True(politica.PermiteVisualizacao(acesso, contexto, Agora));
    }

    [Fact]
    public void PoliticaBilheteSessao_DeveRecusar_SessaoDiferente()
    {
        var politica = new PoliticaBilheteSessao();
        var acesso = CriarAcesso(TipoAcesso.BilheteSessao, filmeId: 10, sessaoId: 5, festivalId: 2);
        var contexto = ContextoSessao(filmeId: 10, sessaoId: 7, festivalId: 2);

        Assert.False(politica.PermiteVisualizacao(acesso, contexto, Agora));
    }

    [Fact]
    public void PoliticaBilheteSessao_DeveRecusar_AntesDaSessao()
    {
        var politica = new PoliticaBilheteSessao();
        var acesso = CriarAcesso(
            TipoAcesso.BilheteSessao,
            filmeId: 10,
            sessaoId: 5,
            festivalId: 2,
            inicio: Agora.AddHours(-1),
            fim: Agora.AddHours(3)
        );
        var contexto = ContextoSessao(
            filmeId: 10,
            sessaoId: 5,
            festivalId: 2,
            inicioSessao: Agora.AddMinutes(30),
            fimSessao: Agora.AddHours(2)
        );

        Assert.False(politica.PermiteVisualizacao(acesso, contexto, Agora));
    }

    [Fact]
    public void PoliticaPasseDiario_DevePermitir_SessaoNoDiaDoPasse()
    {
        var politica = new PoliticaPasseDiario();
        var acesso = CriarAcesso(
            TipoAcesso.PasseDiario,
            festivalId: 2,
            inicio: Agora.Date,
            fim: Agora.Date.AddDays(1)
        );
        var contexto = ContextoSessao(filmeId: 10, sessaoId: 5, festivalId: 2, inicioSessao: Agora);

        Assert.True(politica.PermiteVisualizacao(acesso, contexto, Agora));
    }

    [Fact]
    public void PoliticaPasseDiario_DeveRecusar_ForaDoDiaDoPasse()
    {
        var politica = new PoliticaPasseDiario();
        var acesso = CriarAcesso(
            TipoAcesso.PasseDiario,
            festivalId: 2,
            inicio: Agora.Date.AddDays(-1),
            fim: Agora.Date
        );
        var contexto = ContextoSessao(filmeId: 10, sessaoId: 5, festivalId: 2, inicioSessao: Agora);

        Assert.False(politica.PermiteVisualizacao(acesso, contexto, Agora));
    }

    [Fact]
    public void PoliticaPasseCompleto_DevePermitir_FilmeDoFestival()
    {
        var politica = new PoliticaPasseCompleto();
        var acesso = CriarAcesso(TipoAcesso.PasseCompleto, festivalId: 2);
        var contexto = ContextoFilme(10, festivalIdsDoFilme: new[] { 2, 3 });

        Assert.True(politica.PermiteVisualizacao(acesso, contexto, Agora));
    }

    [Fact]
    public void PoliticaPasseCompleto_DeveRecusar_FilmeForaDoFestival()
    {
        var politica = new PoliticaPasseCompleto();
        var acesso = CriarAcesso(TipoAcesso.PasseCompleto, festivalId: 4);
        var contexto = ContextoFilme(10, festivalIdsDoFilme: new[] { 2, 3 });

        Assert.False(politica.PermiteVisualizacao(acesso, contexto, Agora));
    }

    private static AcessoUtilizador CriarAcesso(
        TipoAcesso tipo,
        int? filmeId = null,
        int? sessaoId = null,
        int? festivalId = null,
        DateTime? inicio = null,
        DateTime? fim = null
    )
    {
        return new AcessoUtilizador
        {
            TipoAcesso = tipo,
            FilmeId = filmeId,
            SessaoId = sessaoId,
            FestivalId = festivalId,
            InicioValidade = inicio ?? Agora.AddMinutes(-10),
            FimValidade = fim ?? Agora.AddMinutes(10),
            Ativo = true,
        };
    }

    private static ContextoVisualizacao ContextoFilme(
        int filmeId,
        int? festivalId = null,
        IEnumerable<int>? festivalIdsDoFilme = null
    )
    {
        return new ContextoVisualizacao(
            filmeId,
            null,
            festivalId,
            null,
            null,
            (festivalIdsDoFilme ?? Array.Empty<int>()).ToHashSet()
        );
    }

    private static ContextoVisualizacao ContextoSessao(
        int filmeId,
        int sessaoId,
        int festivalId,
        DateTime? inicioSessao = null,
        DateTime? fimSessao = null
    )
    {
        return new ContextoVisualizacao(
            filmeId,
            sessaoId,
            festivalId,
            inicioSessao ?? Agora,
            fimSessao ?? Agora.AddHours(1),
            new HashSet<int> { festivalId }
        );
    }
}
