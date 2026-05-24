using Microsoft.Extensions.Options;
using OnlineCinemaFestival.Api.Configuracao;
using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Services;

public class AcessoAutomaticoFactory : IAcessoAutomaticoFactory
{
    private readonly AcessosOptions _options;
    private readonly int _duracaoAluguerDigitalHoras;

    public AcessoAutomaticoFactory(IOptions<AcessosOptions> options)
    {
        _options = options.Value;
        _duracaoAluguerDigitalHoras = _options.DuracaoAluguerDigitalHoras;
    }

    public Acesso CriarAluguerDigital(Filme filme) =>
        new()
        {
            Nome = $"Aluguer Digital - {filme.Titulo}",
            Descricao = $"Aluguer individual do filme durante {_duracaoAluguerDigitalHoras} horas.",
            Tipo = TipoAcesso.AluguerDigital,
            Preco = ObterPreco(ChavesPrecosAcesso.AluguerDigital),
            FilmeId = filme.Id,
            DuracaoHoras = _duracaoAluguerDigitalHoras,
            IsAtivo = true,
            CriadoEm = DateTime.UtcNow,
        };

    public Acesso CriarPasseCompleto(Festival festival) =>
        new()
        {
            Nome = $"Passe Completo - {festival.Name}",
            Descricao = "Passe valido para todo o festival.",
            Tipo = TipoAcesso.PasseCompleto,
            Preco = ObterPreco(ChavesPrecosAcesso.PasseCompleto),
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
            Preco = ObterPreco(ChavesPrecosAcesso.PasseDiario),
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
            Preco = sessao.TemChatAoVivo
                ? ObterPreco(ChavesPrecosAcesso.BilheteSessaoComChat)
                : ObterPreco(ChavesPrecosAcesso.BilheteSessao),
            SessaoId = sessao.Id,
            FilmeId = sessao.FilmeId,
            IsAtivo = true,
            CriadoEm = DateTime.UtcNow,
        };

    private decimal ObterPreco(string chave)
    {
        if (!_options.Precos.TryGetValue(chave, out var preco) || preco <= 0)
            throw new InvalidOperationException($"Acessos:Precos:{chave} deve ser maior que zero.");

        return preco;
    }
}
