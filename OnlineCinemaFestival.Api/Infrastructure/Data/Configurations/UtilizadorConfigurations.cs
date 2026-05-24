using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Infrastructure.Data.Configurations;

public class UtilizadorConfiguration : IEntityTypeConfiguration<Utilizador>
{
    public void Configure(EntityTypeBuilder<Utilizador> builder)
    {
        builder.HasIndex(u => u.Email).IsUnique();

        builder.HasIndex(u => u.PhoneNumber).IsUnique().HasFilter("\"PhoneNumber\" <> ''");

        builder
            .HasOne(u => u.Perfil)
            .WithOne(p => p.Utilizador)
            .HasForeignKey<PerfilUtilizador>(p => p.UtilizadorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class UtilizadorGeneroFavoritoConfiguration
    : IEntityTypeConfiguration<UtilizadorGeneroFavorito>
{
    public void Configure(EntityTypeBuilder<UtilizadorGeneroFavorito> builder)
    {
        builder.HasKey(ug => new { ug.UtilizadorId, ug.GeneroId });

        builder
            .HasOne(ug => ug.Utilizador)
            .WithMany(u => u.GenerosFavoritos)
            .HasForeignKey(ug => ug.UtilizadorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(ug => ug.Genero)
            .WithMany(g => g.UtilizadoresFavoritos)
            .HasForeignKey(ug => ug.GeneroId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
