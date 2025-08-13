using BuildingBlocks.Core.Model;
using Orders.OrderItems.Models;
using Orders.OrderItems.ValueObjects;
using Orders.Orders.Enums;
using Orders.Orders.Features.CreatingOrder.V1;
using Orders.Orders.Features.DeleteOrder.V1;
using Orders.Orders.Features.UpdateOrder.V1;
using Orders.Orders.ValueObjects;

namespace Orders.Orders.Models
{
    public record Order : Aggregate<OrderId>
    {
        public OrderNumber OrderNumber { get; private set; } = default!;
        public Accountid AccountId { get; private set; } = default!;
        public OrderDate OrderDate { get; private set; } = default!;
        public OrderStatus Status { get; private set; }
        public TotalPrice TotalPrice { get; private set; } = default!;
        public List<OrderItem> OrderItems { get; private set; } = new();

        public Order() { }

        public static Order Create(
            OrderId id,
            OrderNumber orderNumber,
            Accountid accountId,
            OrderDate orderDate,
            OrderStatus orderStatus,
            TotalPrice totalPrice)
        {
            var orders = new Order
            {
                Id = id,
                OrderNumber = orderNumber,
                AccountId = accountId,
                OrderDate = orderDate,
                Status = orderStatus,
                TotalPrice = totalPrice
            };

            var @event = new OrderCreatedDomainEvent(
                orders.Id,
                orders.OrderNumber,
                orders.AccountId,
                orders.OrderDate,
                orders.Status,
                orders.TotalPrice);

            orders.AddDomainEvent(@event);

            return orders;
        }

        public void Update(
            OrderId id,
            OrderNumber orderNumber,
            Accountid accountId,
            OrderDate orderDate,
            OrderStatus orderStatus,
            TotalPrice totalPrice)
        {
            var orders = new Order
            {
                Id = id,
                OrderNumber = orderNumber,
                AccountId = accountId,
                OrderDate = orderDate,
                Status = orderStatus,
                TotalPrice = totalPrice,
            };
            var @event = new OrderUpdatedDomainEvent(
                orders.Id,
                orders.OrderNumber,
                orders.AccountId,
                orders.OrderDate,
                orders.Status,
                orders.TotalPrice);

            orders.AddDomainEvent(@event);
        }

        public void Delete(
            OrderId id,
            OrderNumber orderNumber,
            Accountid accountId,
            OrderDate orderDate,
            OrderStatus orderStatus,
            TotalPrice totalPrice)
        {
            Id = id;
            OrderNumber = orderNumber;
            AccountId = accountId;
            OrderDate = orderDate;
            Status = orderStatus;
            TotalPrice = totalPrice;

            var @event = new OrderDeletedDomainEvent(
                id,
                orderNumber,
                accountId,
                orderDate,
                orderStatus,
                totalPrice);

            AddDomainEvent(@event);

        }

    }
}
