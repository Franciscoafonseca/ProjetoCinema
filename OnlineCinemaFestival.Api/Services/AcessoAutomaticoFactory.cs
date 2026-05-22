using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Services;

public class AcessoAutomaticoFactory : IAcessoAutomaticoFactory
{
    public Acesso CriarAluguerDigital(Filme filme) =>
        new()
        {
            Nome = $"Aluguer Digital - {filme.Titulo}",
            Descricao = "Aluguer individual do filme durante 48 horas.",
            Tipo = TipoAcesso.AluguerDigital,
            Preco = 3.99m,
            FilmeId = filme.Id,
            DuracaoHoras = 48,
            IsAtivo = true,
            CriadoEm = DateTime.UtcNow,
        };

    public Acesso CriarPasseCompleto(Festival festival) =>
        new()
        {
            Nome = $"Passe Completo - {festival.Name}",
            Descricao = "Passe valido para todo o festival.",
            Tipo = TipoAcesso.PasseCompleto,
            Preco = 24.99m,
            FestivalId = festival.Id,
            IsAtivo = true,
            CriadoEm = DateTime.UtcNow,
        };

    public Acesso CriarPasseDiario(Festival festival, DateTime dia) =>
        new()
        {
            Nome = $"Passe Diario - {festival.Name} - {dia:dd/MM/yyyy}",
            Descricao = "Passe valido para todas as sessoes de um dia do festival.",
            Tipo = TipoAcesso.PasseDiario,
            Preco = 9.99m,
            FestivalId = festival.Id,
            DataAcesso = dia.Date,
            IsAtivo = true,
            CriadoEm = DateTime.UtcNow,
        };

    public Acesso CriarBilheteSessao(Sessao sessao) =>
        new()
        {
            Nome = $"Bilhete - {sessao.Filme?.Titulo ?? $"Sessao {sessao.Id}"}",
            Descricao = "Bilhete valido para uma sessao especifica.",
            Tipo = TipoAcesso.BilheteSessao,
            Preco = sessao.TemChatAoVivo ? 5.99m : 4.99m,
            SessaoId = sessao.Id,
            FilmeId = sessao.FilmeId,
            IsAtivo = true,
            CriadoEm = DateTime.UtcNow,
        };
}
