using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.OrderItems.Models;
using Orders.Orders.ValueObjects;

namespace Orders.Data.Configurations;
public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable(nameof(OrderItem));

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(flight => flight.Value, dbId => OrderItemId.Of(dbId));


        builder.OwnsOne(
            x => x.OrderId,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(OrderItem.OrderId))
                    .HasMaxLength(50)
                    .IsRequired();
            }
        );

        builder.OwnsOne(
            x => x.ProductId,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(OrderItem.ProductId))
                    .HasMaxLength(50)
                    .IsRequired();
            }
        );

        builder.OwnsOne(
            x => x.Quantity,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(OrderItem.Quantity))
                    .HasMaxLength(50)
                    .IsRequired();
            }
        );

        builder.OwnsOne(
            x => x.UnitPrice,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(OrderItem.UnitPrice))
                    .HasMaxLength(10)
                    .IsRequired();
            }
        );
    }
}
