using SmartCharging.Infrastructure.Exceptions;

namespace Orders.Orders.Exceptions;
public class OrderNumberException : DomainException
{
    public OrderNumberException(string orderNumber)
        : base($"Flight Number: '{orderNumber}' is invalid.")
    {
    }
}
