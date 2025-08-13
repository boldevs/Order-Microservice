namespace Orders.Orders.ValueObjects;
public record OrderItemId(Guid Value)
{
    public static OrderItemId Of(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("OrderItemId cannot be empty.");
        return new OrderItemId(value);
    }

    public static implicit operator Guid(OrderItemId id) => id.Value;
}
