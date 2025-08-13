using BuildingBlocks.Exception;
using System.Net;

namespace Orders.OrderItems.Exceptions;
public class OrderItemAlreadyException : AppException
{
    public OrderItemAlreadyException(int? code = default) : base("Order Item already exist!", HttpStatusCode.Conflict, code)
    {
    }
}
