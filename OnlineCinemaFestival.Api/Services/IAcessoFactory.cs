using OnlineCinemaFestival.Api.DTOs;
using ModelAcesso = OnlineCinemaFestival.Api.Models.Acesso;

namespace OnlineCinemaFestival.Api.Services;

public interface IAcessoFactory
{
    ModelAcesso Criar(int utilizadorId, CompraItemDto item, decimal precoPago, DateTime? validade);
}
