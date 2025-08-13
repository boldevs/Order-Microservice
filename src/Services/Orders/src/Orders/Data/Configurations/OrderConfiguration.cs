using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.OrderItems.ValueObjects;
using Orders.Orders.Enums;
using Orders.Orders.Models;

namespace Orders.Data.Configurations;
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable(nameof(Order));

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever()
            .HasConversion<Guid>(flight => flight.Value, dbId => OrderId.Of(dbId));

        builder.OwnsOne(
            x => x.OrderNumber,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Order.OrderNumber))
                    .HasMaxLength(50)
                    .IsRequired();
            }
        );

        builder.OwnsOne(
            x => x.AccountId,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Order.AccountId))
                    .HasMaxLength(50)
                    .IsRequired();
            }
        );

        builder.Property(x => x.Status)
            .HasDefaultValue(OrderStatus.Processing)
            .HasConversion(
                x => x.ToString(),
                x => (Orders.Enums.OrderStatus)Enum.Parse(typeof(Orders.Enums.OrderStatus), x));

        builder.OwnsOne(
            x => x.TotalPrice,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Order.TotalPrice))
                    .HasMaxLength(10)
                    .IsRequired();
            }
        );

        builder.OwnsOne(
            x => x.OrderDate,
            a =>
            {
                a.Property(p => p.Value)
                    .HasColumnName(nameof(Order.OrderDate))
                    .IsRequired();
            }
        );

    }
}
