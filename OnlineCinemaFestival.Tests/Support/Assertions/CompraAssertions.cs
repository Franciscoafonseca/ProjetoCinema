using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Tests.Support.Assertions;

public static class CompraAssertions
{
    public static void DeveEstarPaga(this Compra compra)
    {
        Assert.Equal(EstadoCompra.Pago, compra.Estado);
        Assert.NotNull(compra.PagaEm);
    }

    public static void DeveEstarPendente(this Compra compra)
    {
        Assert.Equal(EstadoCompra.Pendente, compra.Estado);
        Assert.Null(compra.PagaEm);
    }

    public static void DeveEstarCancelada(this Compra compra)
    {
        Assert.Equal(EstadoCompra.Cancelado, compra.Estado);
    }
}
