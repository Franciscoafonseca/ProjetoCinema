using OnlineCinemaFestival.Api.Domain;
using OnlineCinemaFestival.Api.Services;
using OnlineCinemaFestival.Tests.Support;
using OnlineCinemaFestival.Tests.Support.Builders;

namespace OnlineCinemaFestival.Tests.Acessos;

/// <summary>
/// Testa a <see cref="FabricaAcessoUtilizador"/> com cada uma das 4 estratégias de criação
/// de acesso de utilizador (Factory + Strategy pattern).
/// </summary>
public class FabricaAcessoUtilizadorTests
{
    private static readonly DateTime DataCompra = new(2026, 5, 24, 10, 0, 0, DateTimeKind.Utc);

    // ── BilheteSessao ────────────────────────────────────────────────────────

    [Fact]
    public void Criar_BilheteSessao_DefineValidadeComHorarioDaSessao()
    {
        var inicio = new DateTime(2026, 5, 25, 20, 0, 0, DateTimeKind.Utc);
        var fim = new DateTime(2026, 5, 25, 22, 0, 0, DateTimeKind.Utc);
        var festival = new FestivalBuilder().ComId(2).Entre(inicio.Date, fim.Date.AddDays(1)).Build();
        var filme = new FilmeBuilder().ComId(5).ComTitulo("Filme").Build();
        var sessao = new SessaoBuilder().ComId(10).NoFestival(festival).DoFilme(filme).Entre(inicio, fim).Build();
        var acesso = new AcessoBuilder().ComId(1).ComoBilheteSessao(sessao).Build();
        var fabrica = CriarFabrica();
        var compra = new CompraBuilder().ComId(99).DoUtilizador(7).Build();
        var item = new CarrinhoItem { AcessoId = acesso.Id, Acesso = acesso };

        var resultado = fabrica.Criar(7, compra, item, DataCompra);

        Assert.Equal(TipoAcesso.BilheteSessao, resultado.TipoAcesso);
        Assert.Equal(10, resultado.SessaoId);
        Assert.Equal(inicio, resultado.InicioValidade);
        Assert.Equal(fim, resultado.FimValidade);
        Assert.Equal(5, resultado.FilmeId);
        Assert.Equal(2, resultado.FestivalId);
    }

    [Fact]
    public void Criar_BilheteSessao_SemSessao_LancaExcecao()
    {
        var acesso = new Acesso { Id = 1, Tipo = TipoAcesso.BilheteSessao, SessaoId = null };
        var fabrica = CriarFabrica();
        var compra = new CompraBuilder().ComId(99).DoUtilizador(7).Build();
        var item = new CarrinhoItem { AcessoId = acesso.Id, Acesso = acesso };

        Assert.Throws<InvalidOperationException>(() => fabrica.Criar(7, compra, item, DataCompra));
    }

    // ── PasseDiario ───────────────────────────────────────────────────────────

    [Fact]
    public void Criar_PasseDiario_DefineValidadeParaUmDia()
    {
        var dataAcesso = new DateTime(2026, 5, 25, 0, 0, 0, DateTimeKind.Utc);
        var festival = new FestivalBuilder().ComId(3).Entre(dataAcesso, dataAcesso.AddDays(2)).Build();
        var acesso = new AcessoBuilder().ComId(2).ComoPasseDiario(festival, dataAcesso).Build();
        var fabrica = CriarFabrica();
        var compra = new CompraBuilder().ComId(99).DoUtilizador(7).Build();
        var item = new CarrinhoItem { AcessoId = acesso.Id, Acesso = acesso };

        var resultado = fabrica.Criar(7, compra, item, DataCompra);

        Assert.Equal(TipoAcesso.PasseDiario, resultado.TipoAcesso);
        Assert.Equal(dataAcesso.Date, resultado.InicioValidade);
        Assert.Equal(dataAcesso.Date.AddDays(1), resultado.FimValidade);
        Assert.Equal(3, resultado.FestivalId);
        Assert.Null(resultado.SessaoId);
    }

    [Fact]
    public void Criar_PasseDiario_SemData_LancaExcecao()
    {
        var festival = new FestivalBuilder().ComId(3).Build();
        var acesso = new Acesso
        {
            Id = 2,
            Tipo = TipoAcesso.PasseDiario,
            FestivalId = festival.Id,
            Festival = festival,
            DataAcesso = null,
        };
        var fabrica = CriarFabrica();
        var compra = new CompraBuilder().ComId(99).DoUtilizador(7).Build();
        var item = new CarrinhoItem { AcessoId = acesso.Id, Acesso = acesso };

        Assert.Throws<InvalidOperationException>(() => fabrica.Criar(7, compra, item, DataCompra));
    }

    // ── PasseCompleto ─────────────────────────────────────────────────────────

    [Fact]
    public void Criar_PasseCompleto_DefineValidadeComDuraçãoDoFestival()
    {
        var startDate = new DateTime(2026, 5, 20, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2026, 5, 30, 0, 0, 0, DateTimeKind.Utc);
        var festival = new FestivalBuilder().ComId(4).Entre(startDate, endDate).Build();
        var acesso = new AcessoBuilder().ComId(3).ComoPasseCompleto(festival).Build();
        var fabrica = CriarFabrica();
        var compra = new CompraBuilder().ComId(99).DoUtilizador(7).Build();
        var item = new CarrinhoItem { AcessoId = acesso.Id, Acesso = acesso };

        var resultado = fabrica.Criar(7, compra, item, DataCompra);

        Assert.Equal(TipoAcesso.PasseCompleto, resultado.TipoAcesso);
        Assert.Equal(startDate, resultado.InicioValidade);
        Assert.Equal(endDate, resultado.FimValidade);
        Assert.Equal(4, resultado.FestivalId);
        Assert.Null(resultado.SessaoId);
        Assert.Null(resultado.FilmeId);
    }

    [Fact]
    public void Criar_PasseCompleto_SemFestival_LancaExcecao()
    {
        var acesso = new Acesso { Id = 3, Tipo = TipoAcesso.PasseCompleto, FestivalId = null };
        var fabrica = CriarFabrica();
        var compra = new CompraBuilder().ComId(99).DoUtilizador(7).Build();
        var item = new CarrinhoItem { AcessoId = acesso.Id, Acesso = acesso };

        Assert.Throws<InvalidOperationException>(() => fabrica.Criar(7, compra, item, DataCompra));
    }

    // ── AluguerDigital ────────────────────────────────────────────────────────

    [Fact]
    public void Criar_AluguerDigital_UsaDuracaoDefinidaNoAcesso()
    {
        var filme = new FilmeBuilder().ComId(8).ComTitulo("Filme").Build();
        var acesso = new AcessoBuilder().ComId(4).ComoAluguerDigital(filme, duracaoHoras: 24).Build();
        var fabrica = CriarFabrica();
        var compra = new CompraBuilder().ComId(99).DoUtilizador(7).Build();
        var item = new CarrinhoItem { AcessoId = acesso.Id, Acesso = acesso };

        var resultado = fabrica.Criar(7, compra, item, DataCompra);

        Assert.Equal(TipoAcesso.AluguerDigital, resultado.TipoAcesso);
        Assert.Equal(DataCompra, resultado.InicioValidade);
        Assert.Equal(DataCompra.AddHours(24), resultado.FimValidade);
        Assert.Equal(8, resultado.FilmeId);
        Assert.Null(resultado.SessaoId);
    }

    [Fact]
    public void Criar_AluguerDigital_UsaDuracaoPadraoQuandoNaoDefinida()
    {
        var filme = new FilmeBuilder().ComId(8).ComTitulo("Filme").Build();
        // DuracaoHoras = null → usa a config (padrão 48h)
        var acesso = new Acesso
        {
            Id = 4,
            Tipo = TipoAcesso.AluguerDigital,
            FilmeId = filme.Id,
            Filme = filme,
            DuracaoHoras = null,
        };
        var fabrica = CriarFabrica();
        var compra = new CompraBuilder().ComId(99).DoUtilizador(7).Build();
        var item = new CarrinhoItem { AcessoId = acesso.Id, Acesso = acesso };

        var resultado = fabrica.Criar(7, compra, item, DataCompra);

        // Duração padrão é 48h (ver OpcoesTeste.Acessos)
        Assert.Equal(DataCompra.AddHours(48), resultado.FimValidade);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static FabricaAcessoUtilizador CriarFabrica() =>
        new(
            new IEstrategiaCriacaoAcessoUtilizador[]
            {
                new EstrategiaCriacaoBilheteSessao(),
                new EstrategiaCriacaoPasseDiario(),
                new EstrategiaCriacaoPasseCompleto(),
                new EstrategiaCriacaoAluguerDigital(OpcoesTeste.Acessos()),
            }
        );
}
