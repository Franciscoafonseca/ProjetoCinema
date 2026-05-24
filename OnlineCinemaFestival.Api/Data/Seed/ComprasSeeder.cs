using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Data.Seed;

public static partial class DbSeeder
{
    public sealed class ComprasSeeder : ISeedStep
    {
        public async Task ExecutarAsync(SeedContext contexto)
        {
            await CriarCarrinhosAsync(contexto.Db, contexto.Utilizadores, contexto.Acessos);
            await CriarComprasEAcessosUtilizadorAsync(
                contexto.Db,
                contexto.Utilizadores,
                contexto.Acessos
            );

            if (contexto.SessaoChatTeste != null)
            {
                await GarantirAcessoSessaoChatTesteAsync(
                    contexto.Db,
                    contexto.Utilizadores,
                    contexto.SessaoChatTeste
                );
            }
        }
    }

    private static async Task CriarCarrinhosAsync(
        AppDbContext db,
        List<Utilizador> utilizadores,
        List<Acesso> acessos
    )
    {
        var quantidadeExistente = await db.Carrinhos.CountAsync();

        if (quantidadeExistente >= 12)
            return;

        var utilizadoresComCarrinho = await db.Carrinhos.Select(c => c.UtilizadorId).ToListAsync();

        var candidatos = utilizadores.Where(u => !utilizadoresComCarrinho.Contains(u.Id)).ToList();

        var quantidadeEmFalta = 12 - quantidadeExistente;
        var utilizadoresEscolhidos = EscolherAleatorio(candidatos, quantidadeEmFalta);

        var carrinhos = new List<Carrinho>();

        foreach (var utilizador in utilizadoresEscolhidos)
        {
            var carrinho = new Carrinho
            {
                UtilizadorId = utilizador.Id,
                DataCriacao = DateTime.UtcNow.AddDays(-SeedRandom.Next(1, 12)),
                AtualizadoEm = DateTime.UtcNow.AddHours(-SeedRandom.Next(1, 72)),
            };

            var tiposPretendidos = new[]
            {
                TipoAcesso.BilheteSessao,
                TipoAcesso.PasseDiario,
                TipoAcesso.PasseCompleto,
                TipoAcesso.AluguerDigital,
            };

            var tiposEscolhidos = tiposPretendidos
                .OrderBy(_ => SeedRandom.Next())
                .Take(SeedRandom.Next(2, 5))
                .ToList();

            foreach (var tipo in tiposEscolhidos)
            {
                var acessosDoTipo = acessos.Where(a => a.Tipo == tipo && a.IsAtivo).ToList();

                if (acessosDoTipo.Count == 0)
                    continue;

                var acesso = EscolherAleatorio(acessosDoTipo, 1).Single();

                carrinho.Itens.Add(
                    new CarrinhoItem
                    {
                        AcessoId = acesso.Id,
                        PrecoUnitario = acesso.Preco,
                        Quantidade = 1,
                        DataAdicao = DateTime.UtcNow.AddHours(-SeedRandom.Next(1, 72)),
                    }
                );
            }

            if (carrinho.Itens.Any())
                carrinhos.Add(carrinho);
        }

        if (carrinhos.Count > 0)
        {
            await db.Carrinhos.AddRangeAsync(carrinhos);
            await db.SaveChangesAsync();
        }
    }

    private static async Task CriarComprasEAcessosUtilizadorAsync(
        AppDbContext db,
        List<Utilizador> utilizadores,
        List<Acesso> acessos
    )
    {
        var comprasExistentes = await db.Compras.CountAsync();

        if (comprasExistentes >= NumeroCompras)
            return;

        var acessosComNavegacoes = await db
            .Acessos.Include(a => a.Sessao)
                .ThenInclude(s => s!.Filme)
            .Include(a => a.Festival)
            .Include(a => a.Filme)
            .Where(a => a.IsAtivo)
            .ToListAsync();

        if (acessosComNavegacoes.Count == 0)
            return;

        var compras = new List<Compra>();
        var acessosUtilizador = new List<AcessoUtilizador>();

        var quantidadeEmFalta = NumeroCompras - comprasExistentes;

        for (var i = 0; i < quantidadeEmFalta; i++)
        {
            var utilizador = EscolherAleatorio(utilizadores, 1).Single();
            var acessosComprados = EscolherAleatorio(acessosComNavegacoes, SeedRandom.Next(1, 5));

            var dataCompra = DateTime
                .UtcNow.AddDays(-SeedRandom.Next(0, 100))
                .AddMinutes(-SeedRandom.Next(1, 600));

            var compra = new Compra
            {
                UtilizadorId = utilizador.Id,
                Referencia =
                    $"CMP-SEED-{utilizador.Id}-{comprasExistentes + i + 1}-{DateTime.UtcNow:yyyyMMddHHmmssfff}",
                Estado = EstadoCompra.Pago,
                CriadaEm = dataCompra,
                PagaEm = dataCompra.AddMinutes(SeedRandom.Next(1, 20)),
            };

            foreach (var acesso in acessosComprados)
            {
                compra.Itens.Add(
                    new ItemCompra
                    {
                        AcessoId = acesso.Id,
                        NomeAcesso = acesso.Nome,
                        TipoAcesso = acesso.Tipo,
                        PrecoUnitario = acesso.Preco,
                        Quantidade = 1,
                        Subtotal = acesso.Preco,
                    }
                );

                var validade = CalcularValidadeAcessoSeed(acesso, compra.PagaEm ?? compra.CriadaEm);

                acessosUtilizador.Add(
                    new AcessoUtilizador
                    {
                        UtilizadorId = utilizador.Id,
                        AcessoId = acesso.Id,
                        Compra = compra,
                        TipoAcesso = acesso.Tipo,
                        SessaoId = acesso.SessaoId,
                        FestivalId = acesso.FestivalId ?? acesso.Sessao?.FestivalId,
                        FilmeId = acesso.FilmeId ?? acesso.Sessao?.FilmeId,
                        InicioValidade = validade.inicio,
                        FimValidade = validade.fim,
                        Ativo = validade.fim > DateTime.UtcNow,
                        CriadoEm = compra.CriadaEm,
                    }
                );
            }

            compra.ValorTotal = compra.Itens.Sum(i => i.Subtotal);

            compra.Pagamento = new Pagamento
            {
                Referencia = $"PG-{compra.Referencia}",
                Valor = compra.ValorTotal,
                Metodo = "Simulado",
                Estado = EstadoPagamento.Aprovado,
                CriadoEm = compra.CriadaEm,
                ProcessadoEm = compra.PagaEm,
                Mensagem = "Pagamento seed aprovado automaticamente.",
            };

            compras.Add(compra);
        }

        if (compras.Count > 0)
        {
            await db.Compras.AddRangeAsync(compras);
            await db.AcessosUtilizador.AddRangeAsync(acessosUtilizador);
            await db.SaveChangesAsync();
        }
    }

    private static async Task GarantirAcessoSessaoChatTesteAsync(
        AppDbContext db,
        List<Utilizador> utilizadores,
        Sessao sessaoChatTeste
    )
    {
        var utilizadorTeste =
            utilizadores.FirstOrDefault(u => u.Email == "utilizador1@teste.pt")
            ?? utilizadores.OrderBy(u => u.Id).FirstOrDefault();

        if (utilizadorTeste == null)
            return;

        var sessao = await db.Sessoes.AsNoTracking().FirstAsync(s => s.Id == sessaoChatTeste.Id);

        var acesso = await db.Acessos.FirstOrDefaultAsync(a =>
            a.Nome == "Bilhete - Chat Ao Vivo Teste"
        );

        if (acesso == null)
        {
            acesso = new Acesso
            {
                Nome = "Bilhete - Chat Ao Vivo Teste",
                CriadoEm = DateTime.UtcNow,
            };
            await db.Acessos.AddAsync(acesso);
        }

        acesso.Descricao = "Bilhete seed para testar o chat ao vivo da sessao ativa.";
        acesso.Tipo = TipoAcesso.BilheteSessao;
        acesso.Preco = 0m;
        acesso.PrecoPago = 0m;
        acesso.IsAtivo = true;
        acesso.SessaoId = sessao.Id;
        acesso.FestivalId = null;
        acesso.FilmeId = sessao.FilmeId;
        acesso.DataAcesso = null;
        acesso.DuracaoHoras = null;
        acesso.Validade = sessao.Fim;

        await db.SaveChangesAsync();

        var referenciaCompra = $"CMP-CHAT-ATIVO-TESTE-{utilizadorTeste.Id}";
        var referenciaPagamento = $"PG-{referenciaCompra}";
        var compra = await db
            .Compras.Include(c => c.Itens)
            .Include(c => c.Pagamento)
            .FirstOrDefaultAsync(c => c.Referencia == referenciaCompra);

        if (compra == null)
        {
            compra = new Compra
            {
                Referencia = referenciaCompra,
                UtilizadorId = utilizadorTeste.Id,
            };

            await db.Compras.AddAsync(compra);
        }

        compra.Estado = EstadoCompra.Pago;
        compra.CriadaEm = DateTime.UtcNow.AddMinutes(-35);
        compra.PagaEm = DateTime.UtcNow.AddMinutes(-34);
        compra.ValorTotal = 0m;

        var item = compra.Itens.FirstOrDefault(i => i.AcessoId == acesso.Id);

        if (item == null)
        {
            compra.Itens.Add(
                new ItemCompra
                {
                    AcessoId = acesso.Id,
                    NomeAcesso = acesso.Nome,
                    TipoAcesso = acesso.Tipo,
                    PrecoUnitario = 0m,
                    Quantidade = 1,
                    Subtotal = 0m,
                }
            );
        }
        else
        {
            item.NomeAcesso = acesso.Nome;
            item.TipoAcesso = acesso.Tipo;
            item.PrecoUnitario = 0m;
            item.Quantidade = 1;
            item.Subtotal = 0m;
        }

        compra.Pagamento ??= new Pagamento { Referencia = referenciaPagamento };
        compra.Pagamento.Referencia = referenciaPagamento;
        compra.Pagamento.Valor = 0m;
        compra.Pagamento.Metodo = "Seed";
        compra.Pagamento.Estado = EstadoPagamento.Aprovado;
        compra.Pagamento.CriadoEm = compra.CriadaEm;
        compra.Pagamento.ProcessadoEm = compra.PagaEm;
        compra.Pagamento.Mensagem = "Acesso seed para teste do chat ao vivo.";

        await db.SaveChangesAsync();

        var acessoUtilizador = await db.AcessosUtilizador.FirstOrDefaultAsync(a =>
            a.UtilizadorId == utilizadorTeste.Id && a.AcessoId == acesso.Id
        );

        if (acessoUtilizador == null)
        {
            acessoUtilizador = new AcessoUtilizador
            {
                UtilizadorId = utilizadorTeste.Id,
                AcessoId = acesso.Id,
                CompraId = compra.Id,
                CriadoEm = DateTime.UtcNow,
            };

            await db.AcessosUtilizador.AddAsync(acessoUtilizador);
        }

        acessoUtilizador.CompraId = compra.Id;
        acessoUtilizador.TipoAcesso = TipoAcesso.BilheteSessao;
        acessoUtilizador.SessaoId = sessao.Id;
        acessoUtilizador.FestivalId = sessao.FestivalId;
        acessoUtilizador.FilmeId = sessao.FilmeId;
        acessoUtilizador.InicioValidade = sessao.Inicio.AddMinutes(-15);
        acessoUtilizador.FimValidade = sessao.Fim;
        acessoUtilizador.Ativo = true;

        await db.SaveChangesAsync();
    }
}
