using Orders.OrderItems.Dto;
using Orders.Orders.Enums;

namespace Orders.Orders.Dtos;
public record OrderDto(
    Guid Id,
    string OrderNumber,
    Guid AccountId,
    DateTime OrderDate,
    OrderStatus Status,
    decimal TotalPrice,
    List<OrderItemDto> OrderItems
);
