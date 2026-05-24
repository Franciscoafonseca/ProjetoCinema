using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Infrastructure.Data.Seed;

public static partial class DbSeeder
{
    public sealed class SessoesSeeder : ISeedStep
    {
        public async Task ExecutarAsync(SeedContext contexto)
        {
            contexto.Sessoes.Clear();
            contexto.Sessoes.AddRange(
                await CriarSessoesAsync(contexto.Db, contexto.Festivais, contexto.Filmes)
            );

            contexto.SessaoChatTeste = await CriarSessaoChatAoVivoTesteAsync(
                contexto.Db,
                contexto.Festivais,
                contexto.Filmes
            );

            contexto.Sessoes.Clear();
            contexto.Sessoes.AddRange(await contexto.Db.Sessoes.Include(s => s.Filme).ToListAsync());
        }
    }

    private static async Task<List<Sessao>> CriarSessoesAsync(
        AppDbContext db,
        List<Festival> festivais,
        List<Filme> filmes
    )
    {
        var sessoesExistentesPorFestival = await db
            .Sessoes.GroupBy(s => s.FestivalId)
            .Select(g => new { FestivalId = g.Key, Quantidade = g.Count() })
            .ToDictionaryAsync(x => x.FestivalId, x => x.Quantidade);

        var sessoesNovas = new List<Sessao>();

        foreach (var festival in festivais)
        {
            sessoesExistentesPorFestival.TryGetValue(festival.Id, out var quantidadeExistente);

            if (quantidadeExistente >= 6)
                continue;

            var filmeIdsDoFestival = await db
                .FestivalFilmes.Where(ff => ff.FestivalId == festival.Id)
                .Select(ff => ff.FilmeId)
                .ToListAsync();

            var filmesDoFestival = filmes.Where(f => filmeIdsDoFestival.Contains(f.Id)).ToList();

            if (filmesDoFestival.Count == 0)
                continue;

            var planos = CriarPlanoSessoes(festival);
            var quantidadeEmFalta = 6 - quantidadeExistente;

            foreach (var plano in planos.Take(quantidadeEmFalta))
            {
                var sessao = new Sessao
                {
                    FestivalId = festival.Id,
                    FilmeId = EscolherAleatorio(filmesDoFestival, 1).First().Id,
                    Tipo = TipoSessao.HorarioFixo,
                    Inicio = plano.Inicio,
                    Fim = plano.Fim,
                    TemChatAoVivo = plano.TemChatAoVivo,
                    Observacoes = plano.Observacoes,
                };

                sessoesNovas.Add(sessao);
            }
        }

        if (sessoesNovas.Count > 0)
        {
            await db.Sessoes.AddRangeAsync(sessoesNovas);
            await db.SaveChangesAsync();
        }

        return await db.Sessoes.Include(s => s.Filme).ToListAsync();
    }

    private static async Task<Sessao> CriarSessaoChatAoVivoTesteAsync(
        AppDbContext db,
        List<Festival> festivais,
        List<Filme> filmes
    )
    {
        var agora = DateTime.UtcNow;
        var inicio = agora.AddMinutes(-30);
        var fim = agora.AddHours(2);

        var festival =
            festivais.FirstOrDefault(f => f.StartDate <= agora && f.EndDate >= agora)
            ?? festivais.OrderBy(f => f.StartDate).First();

        if (festival.StartDate > inicio.Date)
            festival.StartDate = inicio.Date;

        if (festival.EndDate < fim.Date)
            festival.EndDate = fim.Date;

        var filmeId = await db
            .FestivalFilmes.Where(ff => ff.FestivalId == festival.Id)
            .Select(ff => (int?)ff.FilmeId)
            .FirstOrDefaultAsync();

        if (!filmeId.HasValue)
        {
            var filme = filmes.OrderBy(f => f.Id).First();

            await db.FestivalFilmes.AddAsync(
                new FestivalFilme
                {
                    FestivalId = festival.Id,
                    FilmeId = filme.Id,
                    ElegivelPremiosPublico = true,
                }
            );

            await db.SaveChangesAsync();
            filmeId = filme.Id;
        }

        var sessao = await db
            .Sessoes.Include(s => s.Filme)
            .FirstOrDefaultAsync(s =>
                s.Observacoes != null && s.Observacoes.Contains(MarcadorSessaoChatTeste)
            );

        if (sessao == null)
        {
            sessao = new Sessao { FestivalId = festival.Id };
            await db.Sessoes.AddAsync(sessao);
        }

        sessao.FestivalId = festival.Id;
        sessao.Tipo = TipoSessao.HorarioFixo;
        sessao.Inicio = inicio;
        sessao.Fim = fim;
        sessao.TemChatAoVivo = true;
        sessao.FilmeId = filmeId.Value;
        sessao.Observacoes =
            $"{MarcadorSessaoChatTeste} - Sessao sempre ativa para testar o chat ao vivo.";

        await db.SaveChangesAsync();

        return sessao;
    }

    private static List<(
        DateTime Inicio,
        DateTime Fim,
        string Observacoes,
        bool TemChatAoVivo
    )> CriarPlanoSessoes(Festival festival)
    {
        var agora = DateTime.UtcNow;
        var hoje = agora.Date;

        var festivalADecorrer = festival.StartDate.Date <= hoje && festival.EndDate.Date >= hoje;
        var festivalPassado = festival.EndDate.Date < hoje;

        if (festivalADecorrer)
        {
            return new List<(DateTime Inicio, DateTime Fim, string Observacoes, bool TemChatAoVivo)>
            {
                (
                    hoje.AddDays(-1).AddHours(20),
                    hoje.AddDays(-1).AddHours(23),
                    "Sessão recente realizada ontem, útil para testar histórico.",
                    true
                ),
                (
                    agora.AddHours(-4),
                    agora.AddHours(-2),
                    "Sessão recente terminada há pouco tempo.",
                    true
                ),
                (
                    agora.AddHours(-1),
                    agora.AddHours(2),
                    "Sessão a decorrer neste momento com chat ao vivo.",
                    true
                ),
                (
                    hoje.AddHours(21),
                    hoje.AddHours(23).AddMinutes(30),
                    "Sessão marcada para hoje à noite.",
                    true
                ),
                (
                    hoje.AddDays(1).AddHours(18),
                    hoje.AddDays(1).AddHours(20),
                    "Sessão futura próxima.",
                    true
                ),
                (
                    hoje.AddDays(2).AddHours(0),
                    hoje.AddDays(2).AddHours(23).AddMinutes(59),
                    "Janela de acesso diária simulada.",
                    false
                ),
            };
        }

        if (festivalPassado)
        {
            var basePassada = festival.StartDate.Date;

            return new List<(DateTime Inicio, DateTime Fim, string Observacoes, bool TemChatAoVivo)>
            {
                (
                    basePassada.AddHours(18),
                    basePassada.AddHours(20),
                    "Sessão passada de abertura.",
                    true
                ),
                (
                    basePassada.AddDays(1).AddHours(20),
                    basePassada.AddDays(1).AddHours(22),
                    "Sessão passada em horário fixo.",
                    true
                ),
                (
                    basePassada.AddDays(2).AddHours(21),
                    basePassada.AddDays(2).AddHours(23),
                    "Sessão passada de competição.",
                    true
                ),
                (
                    basePassada.AddDays(3).AddHours(0),
                    basePassada.AddDays(3).AddHours(23).AddMinutes(59),
                    "Janela de acesso já terminada.",
                    false
                ),
                (
                    basePassada.AddDays(4).AddHours(19),
                    basePassada.AddDays(4).AddHours(21),
                    "Sessão passada de encerramento.",
                    true
                ),
                (
                    basePassada.AddDays(5).AddHours(20),
                    basePassada.AddDays(5).AddHours(22),
                    "Repetição passada.",
                    false
                ),
            };
        }

        var baseFutura = festival.StartDate.Date;

        return new List<(DateTime Inicio, DateTime Fim, string Observacoes, bool TemChatAoVivo)>
        {
            (baseFutura.AddHours(18), baseFutura.AddHours(20), "Sessão futura de abertura.", true),
            (baseFutura.AddHours(21), baseFutura.AddHours(23), "Sessão futura de estreia.", true),
            (
                baseFutura.AddDays(1).AddHours(20),
                baseFutura.AddDays(1).AddHours(22),
                "Sessão futura em horário fixo.",
                true
            ),
            (
                baseFutura.AddDays(2).AddHours(0),
                baseFutura.AddDays(2).AddHours(23).AddMinutes(59),
                "Janela de acesso futura.",
                false
            ),
            (
                baseFutura.AddDays(3).AddHours(19),
                baseFutura.AddDays(3).AddHours(21),
                "Sessão futura de debate.",
                true
            ),
            (
                baseFutura.AddDays(4).AddHours(20),
                baseFutura.AddDays(4).AddHours(22),
                "Sessão futura de encerramento.",
                true
            ),
        };
    }
}
