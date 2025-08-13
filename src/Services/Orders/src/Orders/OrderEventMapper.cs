using BuildingBlocks.Contracts.EventBus.Messages;
using BuildingBlocks.Core;
using BuildingBlocks.Core.Event;
using Orders.OrderItems.Features.CreateOrderItem.V1;
using Orders.OrderItems.Features.DeletedOrderItem.V1;
using Orders.OrderItems.Features.UpdateOrderItem.V1;
using Orders.Orders.Features.CreatingOrder.V1;
using Orders.Orders.Features.DeleteOrder.V1;
using Orders.Orders.Features.UpdateOrder.V1;

namespace Orders;
public sealed class OrderEventMapper : IEventMapper
{
    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            OrderCreatedDomainEvent e => new OrderCreated(e.Id),
            OrderUpdatedDomainEvent e => new OrderUpdated(e.Id),
            OrderDeletedDomainEvent e => new OrderDeleted(e.Id),
            OrderItemCreatedDomainEvent e => new OrderItemAdded(e.Id),
            OrderItemUpdatedDomainEvent e => new OrderItemUpdated(e.Id),
            OrderItemDeletedDomainEvent e => new OrderItemDeleted(e.Id),
            _ => null
        };
    }

    public IInternalCommand? MapToInternalCommand(IDomainEvent @event)
    {
        return null;
    }
}
