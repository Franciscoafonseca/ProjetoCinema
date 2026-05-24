using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Services.PoliticasAcesso;

namespace OnlineCinemaFestival.Tests;

public class PoliticasAcessoTests
{
    [Fact]
    public void PoliticaAluguerDigital_DevePermitir_AcessoValido()
    {
        var agora = new DateTime(2026, 5, 23, 12, 0, 0, DateTimeKind.Utc);
        var politica = new PoliticaAluguerDigital();
        var acesso = CriarAcesso(TipoAcesso.AluguerDigital, agora.AddHours(1), agora);

        var resultado = politica.TemAcesso(acesso, agora);

        Assert.True(resultado);
    }

    [Fact]
    public void PoliticaAluguerDigital_DeveRecusar_AcessoExpirado()
    {
        var agora = new DateTime(2026, 5, 23, 12, 0, 0, DateTimeKind.Utc);
        var politica = new PoliticaAluguerDigital();
        var acesso = CriarAcesso(TipoAcesso.AluguerDigital, agora.AddMinutes(-1), agora.AddHours(-2));

        var resultado = politica.TemAcesso(acesso, agora);

        Assert.False(resultado);
    }

    [Fact]
    public void PoliticaBilheteSessao_DevePermitir_DentroDoHorario()
    {
        var agora = new DateTime(2026, 5, 23, 14, 0, 0, DateTimeKind.Utc);
        var politica = new PoliticaBilheteSessao();
        var acesso = new Acesso
        {
            Tipo = TipoAcesso.BilheteSessao,
            Sessao = new Sessao
            {
                Inicio = agora.AddMinutes(-15),
                Fim = agora.AddMinutes(15),
            },
        };

        Assert.True(politica.TemAcesso(acesso, agora));
    }

    [Fact]
    public void PoliticaBilheteSessao_DeveRecusar_ForaDoHorario()
    {
        var agora = new DateTime(2026, 5, 23, 14, 0, 0, DateTimeKind.Utc);
        var politica = new PoliticaBilheteSessao();
        var acesso = new Acesso
        {
            Tipo = TipoAcesso.BilheteSessao,
            Sessao = new Sessao
            {
                Inicio = agora.AddHours(-2),
                Fim = agora.AddMinutes(-1),
            },
        };

        Assert.False(politica.TemAcesso(acesso, agora));
    }

    [Fact]
    public void PoliticaPasseDiario_DevePermitir_DiaCorreto()
    {
        var agora = new DateTime(2026, 5, 23, 14, 0, 0, DateTimeKind.Utc);
        var politica = new PoliticaPasseDiario();
        var acesso = new Acesso
        {
            Tipo = TipoAcesso.PasseDiario,
            DataAcesso = agora.Date,
        };

        Assert.True(politica.TemAcesso(acesso, agora));
    }

    [Fact]
    public void PoliticaPasseDiario_DeveRecusar_DiaErrado()
    {
        var agora = new DateTime(2026, 5, 23, 14, 0, 0, DateTimeKind.Utc);
        var politica = new PoliticaPasseDiario();
        var acesso = new Acesso
        {
            Tipo = TipoAcesso.PasseDiario,
            DataAcesso = agora.Date.AddDays(-1),
        };

        Assert.False(politica.TemAcesso(acesso, agora));
    }

    [Fact]
    public void PoliticaPasseCompleto_DevePermitir_DentroDoPeriodo()
    {
        var agora = new DateTime(2026, 5, 23, 14, 0, 0, DateTimeKind.Utc);
        var politica = new PoliticaPasseCompleto();
        var acesso = new Acesso
        {
            Tipo = TipoAcesso.PasseCompleto,
            Festival = new Festival
            {
                StartDate = agora.AddDays(-1),
                EndDate = agora.AddDays(1),
            },
        };

        Assert.True(politica.TemAcesso(acesso, agora));
    }

    [Fact]
    public void PoliticaPasseCompleto_DeveRecusar_ForaDoPeriodo()
    {
        var agora = new DateTime(2026, 5, 23, 14, 0, 0, DateTimeKind.Utc);
        var politica = new PoliticaPasseCompleto();
        var acesso = new Acesso
        {
            Tipo = TipoAcesso.PasseCompleto,
            Festival = new Festival
            {
                StartDate = agora.AddDays(-3),
                EndDate = agora.AddDays(-1),
            },
        };

        Assert.False(politica.TemAcesso(acesso, agora));
    }

    private static Acesso CriarAcesso(TipoAcesso tipo, DateTime validade, DateTime criadoEm)
    {
        return new Acesso
        {
            Tipo = tipo,
            FilmeId = 1,
            Validade = validade,
            CriadoEm = criadoEm,
        };
    }
}

