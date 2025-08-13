namespace Orders.OrderItems.ValueObjects;

public record OrderId
{
    public Guid Value { get; }
    private OrderId() { }
    private OrderId(Guid value)
    {
        Value = value;
    }

    public static OrderId Of(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("OrderId cannot be empty.");

        return new OrderId(value);
    }

    public static implicit operator Guid(OrderId id) => id.Value;
}
