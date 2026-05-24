using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Domain;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Infrastructure.Data.Seed;

public static partial class DbSeeder
{
    public sealed class UtilizadoresSeeder : ISeedStep
    {
        public async Task ExecutarAsync(SeedContext contexto)
        {
            contexto.Admin = await CriarAdminAsync(
                contexto.Db,
                contexto.PasswordHashingStrategy,
                contexto.Configuration
            );

            var utilizadores = await CriarUtilizadoresAsync(
                contexto.Db,
                contexto.PasswordHashingStrategy,
                contexto.Configuration
            );

            contexto.Utilizadores.Clear();
            contexto.Utilizadores.AddRange(utilizadores);

            await CriarPerfisAsync(contexto.Db, contexto.TodosUtilizadores);
            await CriarGenerosFavoritosAsync(contexto.Db, contexto.Utilizadores, contexto.Generos);
        }
    }

    private static async Task<Utilizador> CriarAdminAsync(
        AppDbContext db,
        IPasswordHashingStrategy passwordHashingStrategy,
        IConfiguration configuration
    )
    {
        var emailAdmin = ObterConfiguracaoObrigatoria(configuration, "Seed:AdminEmail");
        var passwordAdmin = ObterConfiguracaoObrigatoria(configuration, "Seed:AdminPassword");

        var admin = await db.Utilizadores.FirstOrDefaultAsync(u => u.Email == emailAdmin);

        if (admin != null)
            return admin;

        admin = new Utilizador
        {
            Name = "Administrador",
            Email = emailAdmin,
            PhoneNumber = "+351910000000",
            Role = PapelUtilizador.Administrador,
            IsActive = true,
            Nationality = "PT",
            CreatedAt = DateTime.UtcNow,
        };

        admin.PasswordHash = passwordHashingStrategy.HashPassword(admin, passwordAdmin);

        await db.Utilizadores.AddAsync(admin);
        await db.SaveChangesAsync();

        return admin;
    }

    private static async Task<List<Utilizador>> CriarUtilizadoresAsync(
        AppDbContext db,
        IPasswordHashingStrategy passwordHashingStrategy,
        IConfiguration configuration
    )
    {
        var faker = new Bogus.Faker("pt_PT");
        var passwordUtilizador = ObterConfiguracaoObrigatoria(
            configuration,
            "Seed:UtilizadorPassword"
        );

        var utilizadoresExistentes = await db
            .Utilizadores.Where(u => u.Email.EndsWith("@teste.pt"))
            .ToListAsync();

        if (utilizadoresExistentes.Count >= NumeroUtilizadores)
            return utilizadoresExistentes;

        var utilizadores = new List<Utilizador>();

        for (var i = 1; i <= NumeroUtilizadores; i++)
        {
            var email = $"utilizador{i}@teste.pt";

            if (await db.Utilizadores.AnyAsync(u => u.Email == email))
                continue;

            var utilizador = new Utilizador
            {
                Name = i <= 8 ? $"Utilizador Teste {i}" : faker.Name.FullName(),
                Email = email,
                PhoneNumber = $"+35191000{i:000}",
                Role = PapelUtilizador.Utilizador,
                IsActive = true,
                Nationality = Nacionalidades[SeedRandom.Next(Nacionalidades.Length)],
                CreatedAt = DateTime.UtcNow.AddDays(-SeedRandom.Next(1, 180)),
            };

            utilizador.PasswordHash = passwordHashingStrategy.HashPassword(
                utilizador,
                passwordUtilizador
            );

            utilizadores.Add(utilizador);
        }

        if (utilizadores.Count > 0)
        {
            await db.Utilizadores.AddRangeAsync(utilizadores);
            await db.SaveChangesAsync();
        }

        return await db.Utilizadores.Where(u => u.Email.EndsWith("@teste.pt")).ToListAsync();
    }

    private static async Task CriarPerfisAsync(AppDbContext db, List<Utilizador> utilizadores)
    {
        var utilizadoresComPerfil = await db
            .PerfisUtilizador.Select(p => p.UtilizadorId)
            .ToListAsync();

        var perfis = new List<PerfilUtilizador>();

        foreach (var utilizador in utilizadores)
        {
            if (utilizadoresComPerfil.Contains(utilizador.Id))
                continue;

            perfis.Add(
                new PerfilUtilizador
                {
                    UtilizadorId = utilizador.Id,
                    Bio =
                        $"Perfil de {utilizador.Name}. Apaixonado por cinema, festivais online e novas descobertas cinematograficas.",
                    ProfileImageUrl = $"https://picsum.photos/seed/perfil-{utilizador.Id}/200/200",
                    Nationality = ObterNomePais(utilizador.Nationality),
                    CountryCode = ObterCodigoPais(utilizador.Nationality),
                    Location = Localizacoes[SeedRandom.Next(Localizacoes.Length)],
                    IsPublic = SeedRandom.Next(1, 100) <= 85,
                    CreatedAt = DateTime.UtcNow.AddDays(-SeedRandom.Next(1, 120)),
                }
            );
        }

        if (perfis.Count > 0)
        {
            await db.PerfisUtilizador.AddRangeAsync(perfis);
            await db.SaveChangesAsync();
        }
    }

    private static string ObterCodigoPais(string nacionalidade)
    {
        return nacionalidade switch
        {
            "Portugal" or "PT" => "PT",
            "Brasil" or "BR" => "BR",
            "Cabo Verde" or "CV" => "CV",
            "Angola" or "AO" => "AO",
            "Moçambique" or "MZ" => "MZ",
            "Espanha" or "ES" => "ES",
            "França" or "FR" => "FR",
            "Itália" or "IT" => "IT",
            "Reino Unido" or "GB" => "GB",
            "Alemanha" or "DE" => "DE",
            _ => "PT",
        };
    }

    private static string ObterNomePais(string nacionalidade)
    {
        return nacionalidade switch
        {
            "PT" => "Portugal",
            "BR" => "Brasil",
            "CV" => "Cabo Verde",
            "AO" => "Angola",
            "MZ" => "Moçambique",
            "ES" => "Espanha",
            "FR" => "França",
            "IT" => "Itália",
            "GB" => "Reino Unido",
            "DE" => "Alemanha",
            _ => nacionalidade,
        };
    }

    private static async Task CriarGenerosFavoritosAsync(
        AppDbContext db,
        List<Utilizador> utilizadores,
        List<Genero> generos
    )
    {
        var favoritosExistentes = await db.UtilizadoresGenerosFavoritos.ToListAsync();

        var chavesExistentes = favoritosExistentes
            .Select(f => Chave(f.UtilizadorId, f.GeneroId))
            .ToHashSet();

        var favoritos = new List<UtilizadorGeneroFavorito>();

        foreach (var utilizador in utilizadores)
        {
            var favoritosAtuais = favoritosExistentes.Count(f => f.UtilizadorId == utilizador.Id);
            var quantidadePretendida = SeedRandom.Next(2, 6);
            var quantidadeEmFalta = Math.Max(0, quantidadePretendida - favoritosAtuais);

            if (quantidadeEmFalta == 0)
                continue;

            var generosEscolhidos = EscolherAleatorio(generos, quantidadeEmFalta + 3);

            foreach (var genero in generosEscolhidos)
            {
                var chave = Chave(utilizador.Id, genero.Id);

                if (chavesExistentes.Contains(chave))
                    continue;

                favoritos.Add(
                    new UtilizadorGeneroFavorito
                    {
                        UtilizadorId = utilizador.Id,
                        GeneroId = genero.Id,
                        CreatedAt = DateTime.UtcNow.AddDays(-SeedRandom.Next(1, 90)),
                    }
                );

                chavesExistentes.Add(chave);

                if (favoritos.Count(f => f.UtilizadorId == utilizador.Id) >= quantidadeEmFalta)
                    break;
            }
        }

        if (favoritos.Count > 0)
        {
            await db.UtilizadoresGenerosFavoritos.AddRangeAsync(favoritos);
            await db.SaveChangesAsync();
        }
    }
}
