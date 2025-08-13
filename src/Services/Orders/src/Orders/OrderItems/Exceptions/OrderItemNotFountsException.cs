using BuildingBlocks.Exception;
using System.Net;

namespace Orders.OrderItems.Exceptions;
public class OrderItemNotFountsException : AppException
{
    public OrderItemNotFountsException() : base("Order Item not found!", HttpStatusCode.NotFound)
    {
    }
}
