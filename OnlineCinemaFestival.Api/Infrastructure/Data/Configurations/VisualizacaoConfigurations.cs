using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Infrastructure.Data.Configurations;

public class MensagemChatSessaoConfiguration : IEntityTypeConfiguration<MensagemChatSessao>
{
    public void Configure(EntityTypeBuilder<MensagemChatSessao> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Texto).HasMaxLength(600).IsRequired();
        builder.Property(m => m.EnviadaEm).IsRequired();
        builder.HasIndex(m => new { m.SessaoId, m.EnviadaEm });

        builder
            .HasOne(m => m.Sessao)
            .WithMany(s => s.MensagensChat)
            .HasForeignKey(m => m.SessaoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(m => m.Utilizador)
            .WithMany(u => u.MensagensChatSessao)
            .HasForeignKey(m => m.UtilizadorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class VisualizacaoConfiguration : IEntityTypeConfiguration<Visualizacao>
{
    public void Configure(EntityTypeBuilder<Visualizacao> builder)
    {
        builder
            .HasOne(v => v.Utilizador)
            .WithMany(u => u.Visualizacoes)
            .HasForeignKey(v => v.UtilizadorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(v => v.Filme)
            .WithMany(f => f.Visualizacoes)
            .HasForeignKey(v => v.FilmeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(v => v.Sessao)
            .WithMany(s => s.Visualizacoes)
            .HasForeignKey(v => v.SessaoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(v => v.Festival)
            .WithMany(f => f.Visualizacoes)
            .HasForeignKey(v => v.FestivalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(v => new { v.UtilizadorId, v.VisualizadoEm });
    }
}
