using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineCinemaFestival.Api.Domain;

namespace OnlineCinemaFestival.Api.Infrastructure.Data.Configurations;

public class SessaoConfiguration : IEntityTypeConfiguration<Sessao>
{
    public void Configure(EntityTypeBuilder<Sessao> builder)
    {
        builder
            .HasOne(s => s.Festival)
            .WithMany(f => f.Sessoes)
            .HasForeignKey(s => s.FestivalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(s => s.Filme)
            .WithMany(f => f.Sessoes)
            .HasForeignKey(s => s.FilmeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class AcessoConfiguration : IEntityTypeConfiguration<Acesso>
{
    public void Configure(EntityTypeBuilder<Acesso> builder)
    {
        builder
            .HasOne(a => a.Sessao)
            .WithMany(s => s.Acessos)
            .HasForeignKey(a => a.SessaoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(a => a.Festival)
            .WithMany(f => f.Acessos)
            .HasForeignKey(a => a.FestivalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(a => a.Filme)
            .WithMany(f => f.Acessos)
            .HasForeignKey(a => a.FilmeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(a => a.Preco).HasConversion<double>().HasColumnType("REAL");
    }
}

public class CarrinhoConfiguration : IEntityTypeConfiguration<Carrinho>
{
    public void Configure(EntityTypeBuilder<Carrinho> builder)
    {
        builder
            .HasOne(c => c.Utilizador)
            .WithOne(u => u.Carrinho)
            .HasForeignKey<Carrinho>(c => c.UtilizadorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.UtilizadorId).IsUnique();
    }
}

public class CarrinhoItemConfiguration : IEntityTypeConfiguration<CarrinhoItem>
{
    public void Configure(EntityTypeBuilder<CarrinhoItem> builder)
    {
        builder.ToTable("ItensCarrinho");

        builder
            .HasOne(i => i.Carrinho)
            .WithMany(c => c.Itens)
            .HasForeignKey(i => i.CarrinhoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(i => i.Acesso)
            .WithMany(a => a.CarrinhoItems)
            .HasForeignKey(i => i.AcessoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(i => new { i.CarrinhoId, i.AcessoId }).IsUnique();
        builder.Property(i => i.PrecoUnitario).HasConversion<double>().HasColumnType("REAL");
    }
}

public class CompraConfiguration : IEntityTypeConfiguration<Compra>
{
    public void Configure(EntityTypeBuilder<Compra> builder)
    {
        builder
            .HasOne(c => c.Utilizador)
            .WithMany(u => u.Compras)
            .HasForeignKey(c => c.UtilizadorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.Referencia).IsUnique();
        builder.Property(c => c.ValorTotal).HasConversion<double>().HasColumnType("REAL");
    }
}

public class PagamentoConfiguration : IEntityTypeConfiguration<Pagamento>
{
    public void Configure(EntityTypeBuilder<Pagamento> builder)
    {
        builder
            .HasOne(p => p.Compra)
            .WithOne(c => c.Pagamento)
            .HasForeignKey<Pagamento>(p => p.CompraId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.CompraId).IsUnique();
        builder.HasIndex(p => p.Referencia).IsUnique();
        builder.Property(p => p.Valor).HasConversion<double>().HasColumnType("REAL");
    }
}

public class ItemCompraConfiguration : IEntityTypeConfiguration<ItemCompra>
{
    public void Configure(EntityTypeBuilder<ItemCompra> builder)
    {
        builder
            .HasOne(i => i.Compra)
            .WithMany(c => c.Itens)
            .HasForeignKey(i => i.CompraId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(i => i.Acesso)
            .WithMany(a => a.ItensCompra)
            .HasForeignKey(i => i.AcessoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(i => i.PrecoUnitario).HasConversion<double>().HasColumnType("REAL");
        builder.Property(i => i.Subtotal).HasConversion<double>().HasColumnType("REAL");
    }
}

public class AcessoUtilizadorConfiguration : IEntityTypeConfiguration<AcessoUtilizador>
{
    public void Configure(EntityTypeBuilder<AcessoUtilizador> builder)
    {
        builder
            .HasOne(a => a.Utilizador)
            .WithMany(u => u.AcessosUtilizador)
            .HasForeignKey(a => a.UtilizadorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(a => a.Acesso)
            .WithMany(a => a.AcessosUtilizador)
            .HasForeignKey(a => a.AcessoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(a => a.Compra)
            .WithMany(c => c.AcessosUtilizador)
            .HasForeignKey(a => a.CompraId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(a => a.Sessao)
            .WithMany(s => s.AcessosUtilizador)
            .HasForeignKey(a => a.SessaoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(a => a.Festival)
            .WithMany(f => f.AcessosUtilizador)
            .HasForeignKey(a => a.FestivalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(a => a.Filme)
            .WithMany(f => f.AcessosUtilizador)
            .HasForeignKey(a => a.FilmeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => new { a.UtilizadorId, a.AcessoId });
    }
}
