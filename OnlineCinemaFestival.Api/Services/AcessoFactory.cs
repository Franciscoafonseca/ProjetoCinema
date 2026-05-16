using OnlineCinemaFestival.Api.DTOs;
using ModelAcesso = OnlineCinemaFestival.Api.Models.Acesso;

namespace OnlineCinemaFestival.Api.Services;

public class AcessoFactory : IAcessoFactory
{
    public ModelAcesso Criar(string utilizadorId, CompraItemDto item, decimal precoPago, DateTime? validade)
    {
        return new ModelAcesso
        {
            UtilizadorId = utilizadorId,
            Tipo = item.Tipo,
            FilmeId = item.FilmeId,
            SessaoId = item.SessaoId,
            PrecoPago = precoPago,
            Validade = validade
        };
    }
}
