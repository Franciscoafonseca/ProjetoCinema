using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public interface IAcessoStrategy
{
    // Calcula o preço específico para este tipo de acesso
    decimal CalcularPreco();

    // Calcula a data de expiração (ex: janela de 48h para aluguer)
    DateTime? CalcularExpiracao();

    // Devolve a descrição amigável do item
    string ObterDescricao();
}