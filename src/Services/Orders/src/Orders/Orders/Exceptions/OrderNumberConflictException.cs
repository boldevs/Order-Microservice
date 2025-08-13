using SmartCharging.Infrastructure.Exceptions;

namespace Orders.Orders.Exceptions;
public class OrderNumberConflictException : DomainException
{
    public OrderNumberConflictException(string orderNumber)
        : base($"Order Number: '{orderNumber}' already exists.")
    {
    }
}
