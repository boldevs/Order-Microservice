namespace Orders.Orders.ValueObjects;

public record OrderDate
{
    public DateTime Value { get; }

    private OrderDate(DateTime value)
    {
        Value = value;
    }

    public static OrderDate Of(DateTime value)
    {
        if (value == default)
            throw new ArgumentException("Order date cannot be default value.");

        return new OrderDate(value);
    }

    public static implicit operator DateTime(OrderDate orderDate) => orderDate.Value;
}
