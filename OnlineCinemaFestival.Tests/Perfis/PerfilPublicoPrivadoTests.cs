using Microsoft.AspNetCore.Http;
using OnlineCinemaFestival.Api.DTOs;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Services;
using OnlineCinemaFestival.Tests.Support.Builders;
using OnlineCinemaFestival.Tests.Support.Fakes;

namespace OnlineCinemaFestival.Tests.Perfis;

public class PerfilPublicoPrivadoTests
{
    [Fact]
    public async Task ObterPerfilPublico_NaoExpoeEmailNemTelefone()
    {
        var utilizador = new UtilizadorBuilder()
            .ComId(2)
            .ComNome("Publico")
            .ComEmail("publico@teste.pt")
            .ComTelefone("910000000")
            .ComPerfil(new PerfilBuilder().Publico())
            .Build();
        var service = CriarPerfilService(utilizador);

        var perfil = await service.ObterPerfilPublicoAsync(utilizador.Id);

        Assert.Equal("Publico", perfil.Name);
        Assert.Null(typeof(PerfilPublicoDTO).GetProperty("Email"));
        Assert.Null(typeof(PerfilPublicoDTO).GetProperty("PhoneNumber"));
    }

    [Fact]
    public async Task ObterMeuPerfil_ExpoeEmailETelefone()
    {
        var utilizador = new UtilizadorBuilder()
            .ComId(7)
            .ComNome("Privado")
            .ComEmail("privado@teste.pt")
            .ComTelefone("920000000")
            .ComPerfil(new PerfilBuilder().Privado())
            .Build();
        var service = CriarPerfilService(utilizador);

        var perfil = await service.ObterMeuPerfilAsync(utilizador.Id);

        Assert.Equal("privado@teste.pt", perfil.Email);
        Assert.Equal("920000000", perfil.PhoneNumber);
        Assert.NotNull(typeof(PerfilPrivadoDTO).GetProperty("Email"));
        Assert.NotNull(typeof(PerfilPrivadoDTO).GetProperty("PhoneNumber"));
    }

    private static PerfilUtilizadorService CriarPerfilService(Utilizador utilizador)
    {
        return new PerfilUtilizadorService(
            new UtilizadorRepositoryFalso(utilizador),
            new GeneroRepositoryFalso(),
            new PerfilFotoUploadServiceFalso()
        );
    }

    private sealed class PerfilFotoUploadServiceFalso : IPerfilFotoUploadService
    {
        public Task<string> GuardarAsync(IFormFile ficheiro) => Task.FromResult("/foto.png");
    }
}
