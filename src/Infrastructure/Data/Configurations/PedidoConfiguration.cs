using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public sealed class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
    {
        public void Configure(EntityTypeBuilder<Pedido> builder)
        {
            builder.ToTable("Pedidos");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                   .ValueGeneratedNever();

            builder.Property(p => p.ClienteId)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.DataCriacao)
                   .IsRequired();

            builder.Property(p => p.Status)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(p => p.ValorTotal)
                   .HasColumnType("decimal(18,2)");

            // Relacionamento 1:N com ItemPedido
            builder.HasMany(p => p.Itens)
                   .WithOne()
                   .HasForeignKey("PedidoId")
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public sealed class ItemPedidoConfiguration : IEntityTypeConfiguration<ItemPedido>
    {
        public void Configure(EntityTypeBuilder<ItemPedido> builder)
        {
            builder.ToTable("ItensPedido");

            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id)
                   .ValueGeneratedNever();

            builder.Property(i => i.ProdutoId)
                   .IsRequired();

            builder.Property(i => i.Quantidade)
                   .IsRequired();

            builder.Property(i => i.PrecoUnitario)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();
        }
    }
}
