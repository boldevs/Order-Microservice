using SmartCharging.Infrastructure.Exceptions;

namespace Orders.Orders.Exceptions;
public class InvalidOrderDateException : DomainException
{
    public InvalidOrderDateException(DateTime orderDate)
        : base($"The order date '{orderDate}' is invalid.")
    {
    }
}
