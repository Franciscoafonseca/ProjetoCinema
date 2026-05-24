using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Data.Seed;

public static partial class DbSeeder
{
    public sealed class AcessosSeeder : ISeedStep
    {
        public async Task ExecutarAsync(SeedContext contexto)
        {
            contexto.Acessos.Clear();
            contexto.Acessos.AddRange(
                await CriarAcessosAsync(
                    contexto.Db,
                    contexto.Festivais,
                    contexto.Filmes,
                    contexto.Sessoes
                )
            );
        }
    }

    private static async Task<List<Acesso>> CriarAcessosAsync(
        AppDbContext db,
        List<Festival> festivais,
        List<Filme> filmes,
        List<Sessao> sessoes
    )
    {
        var acessosExistentes = await db.Acessos.ToListAsync();
        var nomesExistentes = acessosExistentes.Select(a => a.Nome).ToHashSet();

        var acessos = new List<Acesso>();

        foreach (var sessao in sessoes)
        {
            var nome = $"Bilhete - Sessão {sessao.Id}";

            if (nomesExistentes.Contains(nome))
                continue;

            acessos.Add(
                new Acesso
                {
                    Nome = nome,
                    Descricao =
                        sessao.Inicio <= DateTime.UtcNow && sessao.Fim >= DateTime.UtcNow
                            ? "Bilhete válido para uma sessão que está a decorrer agora."
                            : "Bilhete válido para uma sessão específica.",
                    Tipo = TipoAcesso.BilheteSessao,
                    Preco = sessao.TemChatAoVivo ? 5.99m : 4.99m,
                    SessaoId = sessao.Id,
                    FestivalId = null,
                    FilmeId = sessao.FilmeId,
                    DataAcesso = null,
                    DuracaoHoras = null,
                    IsAtivo = true,
                    CriadoEm = DateTime.UtcNow,
                }
            );
        }

        foreach (var festival in festivais)
        {
            var nomePasseCompleto = $"Passe Completo - {festival.Name}";

            if (!nomesExistentes.Contains(nomePasseCompleto))
            {
                acessos.Add(
                    new Acesso
                    {
                        Nome = nomePasseCompleto,
                        Descricao = "Passe válido para todo o festival.",
                        Tipo = TipoAcesso.PasseCompleto,
                        Preco = 24.99m,
                        SessaoId = null,
                        FestivalId = festival.Id,
                        FilmeId = null,
                        DataAcesso = null,
                        DuracaoHoras = null,
                        IsAtivo = true,
                        CriadoEm = DateTime.UtcNow,
                    }
                );
            }

            var totalDias = Math.Max(1, (festival.EndDate.Date - festival.StartDate.Date).Days + 1);

            for (var i = 0; i < totalDias; i++)
            {
                var dia = festival.StartDate.Date.AddDays(i);
                var nomePasseDiario = $"Passe Diário - {festival.Name} - {dia:dd/MM/yyyy}";

                if (nomesExistentes.Contains(nomePasseDiario))
                    continue;

                acessos.Add(
                    new Acesso
                    {
                        Nome = nomePasseDiario,
                        Descricao = "Passe válido para todas as sessões de um dia do festival.",
                        Tipo = TipoAcesso.PasseDiario,
                        Preco = 9.99m,
                        SessaoId = null,
                        FestivalId = festival.Id,
                        FilmeId = null,
                        DataAcesso = dia,
                        DuracaoHoras = null,
                        IsAtivo = true,
                        CriadoEm = DateTime.UtcNow,
                    }
                );
            }
        }

        var quantidadeAlugueres = Math.Max(40, (int)(filmes.Count * 0.65));
        var filmesComAluguer = EscolherAleatorio(filmes, quantidadeAlugueres);

        foreach (var filme in filmesComAluguer)
        {
            var nome = $"Aluguer Digital - {filme.Titulo}";

            if (nomesExistentes.Contains(nome))
                continue;

            acessos.Add(
                new Acesso
                {
                    Nome = nome,
                    Descricao = "Aluguer individual do filme durante 48 horas.",
                    Tipo = TipoAcesso.AluguerDigital,
                    Preco = 3.99m,
                    SessaoId = null,
                    FestivalId = null,
                    FilmeId = filme.Id,
                    DataAcesso = null,
                    DuracaoHoras = 48,
                    IsAtivo = true,
                    CriadoEm = DateTime.UtcNow,
                }
            );
        }

        if (acessos.Count > 0)
        {
            await db.Acessos.AddRangeAsync(acessos);
            await db.SaveChangesAsync();
        }

        return await db
            .Acessos.Include(a => a.Sessao)
            .Include(a => a.Festival)
            .Include(a => a.Filme)
            .ToListAsync();
    }

    private static (DateTime inicio, DateTime fim) CalcularValidadeAcessoSeed(
        Acesso acesso,
        DateTime dataCompra
    )
    {
        return acesso.Tipo switch
        {
            TipoAcesso.BilheteSessao when acesso.Sessao != null => (
                acesso.Sessao.Inicio,
                acesso.Sessao.Fim
            ),

            TipoAcesso.PasseDiario when acesso.DataAcesso.HasValue => (
                acesso.DataAcesso.Value.Date,
                acesso.DataAcesso.Value.Date.AddDays(1).AddTicks(-1)
            ),

            TipoAcesso.PasseCompleto when acesso.Festival != null => (
                acesso.Festival.StartDate.Date,
                acesso.Festival.EndDate.Date.AddDays(1).AddTicks(-1)
            ),

            TipoAcesso.AluguerDigital => (
                dataCompra,
                dataCompra.AddHours(acesso.DuracaoHoras ?? 48)
            ),

            _ => (dataCompra, dataCompra.AddDays(1)),
        };
    }
}
