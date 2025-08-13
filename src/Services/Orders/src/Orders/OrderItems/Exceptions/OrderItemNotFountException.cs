using BuildingBlocks.Exception;

namespace Orders.OrderItems.Exceptions;
public class OrderItemNotFountException : AppException
{
    public OrderItemNotFountException(Guid id, int? code = default)
        : base($"Order Item with ID '{id}' not found.", System.Net.HttpStatusCode.NotFound, code)
    {
    }
}
