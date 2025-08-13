using BuildingBlocks.Exception;

namespace Orders.Orders.Exceptions;
public class OrderNotFountException : AppException
{
    public OrderNotFountException(Guid id, int? code = default)
        : base($"Order with ID '{id}' not found.", System.Net.HttpStatusCode.NotFound, code)
    {
    }
}
