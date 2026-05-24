using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Models;

namespace OnlineCinemaFestival.Api.Data.Seed;

public static partial class DbSeeder
{
    public sealed class ComunidadesSeeder : ISeedStep
    {
        public async Task ExecutarAsync(SeedContext contexto)
        {
            contexto.Comunidades.Clear();
            contexto.Comunidades.AddRange(
                await CriarComunidadesAsync(contexto.Db, contexto.Utilizadores)
            );

            await CriarMembrosComunidadesAsync(
                contexto.Db,
                contexto.Comunidades,
                contexto.Utilizadores
            );

            await CriarComentariosAsync(contexto.Db, contexto.Comunidades, contexto.Utilizadores);
        }
    }

    private static async Task<List<Comunidade>> CriarComunidadesAsync(
        AppDbContext db,
        List<Utilizador> utilizadores
    )
    {
        var existentes = await db.Comunidades.ToListAsync();

        if (existentes.Count >= NumeroComunidades)
            return existentes;

        var comunidades = new List<Comunidade>();

        for (var i = existentes.Count; i < NumeroComunidades; i++)
        {
            var nome = NomesComunidades[i % NomesComunidades.Length];

            if (existentes.Any(c => c.Name == nome) || comunidades.Any(c => c.Name == nome))
                nome = $"{nome} {i + 1}";

            comunidades.Add(
                new Comunidade
                {
                    Name = nome,
                    Description =
                        $"Espaço para discutir filmes, sessões, críticas e recomendações relacionadas com {nome}.",
                    ImageUrl = $"https://picsum.photos/seed/comunidade-{i + 1}/600/300",
                    IsPublic = SeedRandom.Next(1, 100) <= 90,
                    CodigoConvite = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant(),
                    CreatedByUserId = EscolherAleatorio(utilizadores, 1).Single().Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-SeedRandom.Next(1, 80)),
                }
            );
        }

        if (comunidades.Count > 0)
        {
            await db.Comunidades.AddRangeAsync(comunidades);
            await db.SaveChangesAsync();
        }

        return await db.Comunidades.ToListAsync();
    }

    private static async Task CriarMembrosComunidadesAsync(
        AppDbContext db,
        List<Comunidade> comunidades,
        List<Utilizador> utilizadores
    )
    {
        var existentes = await db.ComunidadeMembros.ToListAsync();

        var chavesExistentes = existentes
            .Select(m => Chave(m.ComunidadeId, m.UtilizadorId))
            .ToHashSet();

        var membros = new List<ComunidadeMembro>();

        foreach (var comunidade in comunidades)
        {
            var membrosAtuais = existentes.Count(m => m.ComunidadeId == comunidade.Id);
            var quantidadePretendida = SeedRandom.Next(8, Math.Min(22, utilizadores.Count) + 1);
            var quantidadeEmFalta = Math.Max(0, quantidadePretendida - membrosAtuais);

            if (quantidadeEmFalta == 0)
                continue;

            var utilizadoresEscolhidos = EscolherAleatorio(utilizadores, quantidadeEmFalta + 5);

            foreach (var utilizador in utilizadoresEscolhidos)
            {
                var chave = Chave(comunidade.Id, utilizador.Id);

                if (chavesExistentes.Contains(chave))
                    continue;

                membros.Add(
                    new ComunidadeMembro
                    {
                        ComunidadeId = comunidade.Id,
                        UtilizadorId = utilizador.Id,
                        Role = default,
                        JoinedAt = DateTime.UtcNow.AddDays(-SeedRandom.Next(1, 90)),
                    }
                );

                chavesExistentes.Add(chave);

                if (membros.Count(m => m.ComunidadeId == comunidade.Id) >= quantidadeEmFalta)
                    break;
            }
        }

        if (membros.Count > 0)
        {
            await db.ComunidadeMembros.AddRangeAsync(membros);
            await db.SaveChangesAsync();
        }
    }

    private static async Task CriarComentariosAsync(
        AppDbContext db,
        List<Comunidade> comunidades,
        List<Utilizador> utilizadores
    )
    {
        var quantidadeExistente = await db.Comentarios.CountAsync();

        if (quantidadeExistente >= NumeroComentarios)
            return;

        var comentarios = new List<Comentario>();
        var quantidadeEmFalta = NumeroComentarios - quantidadeExistente;

        for (var i = 0; i < quantidadeEmFalta; i++)
        {
            var reportado = SeedRandom.Next(1, 100) <= 12;
            var visivel = !reportado || SeedRandom.Next(1, 100) <= 55;

            comentarios.Add(
                new Comentario
                {
                    UsuarioId = EscolherAleatorio(utilizadores, 1).Single().Id,
                    ComunidadeId = EscolherAleatorio(comunidades, 1).Single().Id,
                    Texto = TextosComentarios[SeedRandom.Next(TextosComentarios.Length)],
                    CriadoEm = DateTime.UtcNow.AddDays(-SeedRandom.Next(1, 60)),
                    Visivel = visivel,
                    Reportado = reportado,
                }
            );
        }

        await db.Comentarios.AddRangeAsync(comentarios);
        await db.SaveChangesAsync();
    }
}
