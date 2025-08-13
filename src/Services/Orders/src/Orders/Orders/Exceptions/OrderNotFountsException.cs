using BuildingBlocks.Exception;
using System.Net;

namespace Orders.Orders.Exceptions;
public class OrderNotFountsException : AppException
{
    public OrderNotFountsException() : base("Order not found!", HttpStatusCode.NotFound)
    {
    }
}
