namespace Orders.OrderItems.Dto;
public record OrderItemDto(
    Guid Id,
    Guid ProductId,
    Guid OrderId,
    string ProductName,
    int Quantity,
    decimal UnitPrice
);