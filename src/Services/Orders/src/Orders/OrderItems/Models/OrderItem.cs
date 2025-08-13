using BuildingBlocks.Core.Model;
using Orders.OrderItems.Features.CreateOrderItem.V1;
using Orders.OrderItems.Features.DeletedOrderItem.V1;
using Orders.OrderItems.Features.UpdateOrderItem.V1;
using Orders.OrderItems.ValueObjects;
using Orders.Orders.ValueObjects;

namespace Orders.OrderItems.Models;
public record OrderItem : Aggregate<OrderItemId>
{
    public OrderId OrderId { get; private set; } = default!;
    public ProductId ProductId { get; private set; } = default!;
    public Quantitys Quantity { get; private set; } = default!;
    public UnitPrice UnitPrice { get; private set; } = default!;

    public OrderItem()
    {

    }

    public static OrderItem Create(
        OrderItemId id,
        OrderId orderId,
        ProductId productId,
        Quantitys quantity,
        UnitPrice unitPrice)
    {
        var orderItem = new OrderItem
        {
            Id = id,
            OrderId = orderId,
            ProductId = productId,
            Quantity = quantity,
            UnitPrice = unitPrice
        };

        var @event = new OrderItemCreatedDomainEvent(
            orderItem.Id,
            orderItem.OrderId,
            orderItem.ProductId,
            orderItem.Quantity,
            orderItem.UnitPrice);

        orderItem.AddDomainEvent(@event);

        return orderItem;
    }

    public void Update(OrderItemId id, OrderId orderId, ProductId productId, Quantitys quantity, UnitPrice unitPrice)
    {
        Id = id;
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;

        var @event = new OrderItemUpdatedDomainEvent(
            Id,
            OrderId,
            ProductId,
            Quantity,
            UnitPrice);

        AddDomainEvent(@event);
    }

    public void Delete(OrderItemId id, OrderId orderId, ProductId productId, Quantitys quantity, UnitPrice unitPrice)
    {
        Id = id;
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;

        var @event = new OrderItemDeletedDomainEvent(
            Id,
            OrderId,
            ProductId,
            Quantity,
            UnitPrice);

        AddDomainEvent(@event);
    }
}
