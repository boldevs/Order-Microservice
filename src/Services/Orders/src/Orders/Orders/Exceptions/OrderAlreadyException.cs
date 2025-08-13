using BuildingBlocks.Exception;
using System.Net;

namespace Orders.Orders.Exceptions;
public class OrderAlreadyException : AppException
{
    public OrderAlreadyException(int? code = default) : base("Order already exist!", HttpStatusCode.Conflict, code)
    {
    }
}
