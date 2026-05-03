using OnlineCinemaFestival.Api.Models;
namespace OnlineCinemaFestival.Api.Services;

// Regra para Bilhete Único: Válido apenas para uma sessão
public class BilheteUnicoStrategy : IAcessoStrategy
{
    public decimal CalcularPreco() => 5.00m;
    public DateTime? CalcularExpiracao() => null;
    public string ObterDescricao() => "Bilhete de Sessão Individual";
}

// Regra para Passe Diário: Válido até ao fim do dia atual[cite: 2]
public class PasseDiarioStrategy : IAcessoStrategy
{
    public decimal CalcularPreco() => 12.50m;
    public DateTime? CalcularExpiracao() => DateTime.UtcNow.Date.AddDays(1).AddTicks(-1);
    public string ObterDescricao() => "Passe Diário (Todos os filmes do dia)";
}

// Regra para Passe Completo: Válido para todo o catálogo do festival[cite: 2]
public class PasseCompletoStrategy : IAcessoStrategy
{
    public decimal CalcularPreco() => 45.00m;
    public DateTime? CalcularExpiracao() => DateTime.UtcNow.AddDays(15); // Ex: Duração do festival[cite: 2]
    public string ObterDescricao() => "Passe Completo (Acesso total)";
}

// Regra para Aluguer Digital: Janela temporal de 48 horas[cite: 2]
public class AluguerDigitalStrategy : IAcessoStrategy
{
    public decimal CalcularPreco() => 3.99m;
    public DateTime? CalcularExpiracao() => DateTime.UtcNow.AddHours(48);
    public string ObterDescricao() => "Aluguer Digital (Acesso por 48h)";
}