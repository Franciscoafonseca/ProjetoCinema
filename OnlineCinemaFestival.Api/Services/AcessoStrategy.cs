using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

// Regra para bilhete de sessão: válido apenas para uma sessão.
public class BilheteSessaoStrategy : IAcessoStrategy
{
    public decimal CalcularPreco() => 5.00m;
    public DateTime? CalcularExpiracao() => null;
    public string ObterDescricao() => "Bilhete de sessão individual";
}

// Regra para passe diário: válido até ao fim do dia atual.
public class PasseDiarioStrategy : IAcessoStrategy
{
    public decimal CalcularPreco() => 12.50m;
    public DateTime? CalcularExpiracao() => DateTime.UtcNow.Date.AddDays(1).AddTicks(-1);
    public string ObterDescricao() => "Passe diário (todos os filmes do dia)";
}

// Regra para passe completo: válido para todo o catálogo do festival.
public class PasseCompletoStrategy : IAcessoStrategy
{
    public decimal CalcularPreco() => 45.00m;
    public DateTime? CalcularExpiracao() => DateTime.UtcNow.AddDays(15);
    public string ObterDescricao() => "Passe completo (acesso total)";
}

// Regra para aluguer digital: janela temporal de 48 horas.
public class AluguerDigitalStrategy : IAcessoStrategy
{
    public decimal CalcularPreco() => 3.99m;
    public DateTime? CalcularExpiracao() => DateTime.UtcNow.AddHours(48);
    public string ObterDescricao() => "Aluguer digital (acesso por 48h)";
}
