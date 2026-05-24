namespace OnlineCinemaFestival.Api.Services;

public class GeradorReferenciaCompra : IGeradorReferenciaCompra
{
    public string Gerar()
    {
        return $"CMP-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}"[..28];
    }
}
