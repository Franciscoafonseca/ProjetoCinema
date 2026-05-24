using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Tests.Support.Assertions;

public static class PagamentoAssertions
{
    public static void DeveEstarAprovado(this Pagamento pagamento)
    {
        Assert.Equal(EstadoPagamento.Aprovado, pagamento.Estado);
        Assert.NotNull(pagamento.ProcessadoEm);
    }

    public static void DeveEstarPendente(this Pagamento pagamento)
    {
        Assert.Equal(EstadoPagamento.Pendente, pagamento.Estado);
        Assert.False(string.IsNullOrWhiteSpace(pagamento.Referencia));
    }

    public static void DeveEstarRecusado(this Pagamento pagamento)
    {
        Assert.Equal(EstadoPagamento.Recusado, pagamento.Estado);
    }

    public static void DeveEstarExpirado(this Pagamento pagamento)
    {
        Assert.Equal(EstadoPagamento.Expirado, pagamento.Estado);
    }
}
