namespace Orders.Orders.ValueObjects;

public record OrderNumber
{
    public string Value { get; }

    private OrderNumber(string value)
    {
        Value = value;
    }

    public static OrderNumber Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Order number cannot be empty or whitespace.");

        return new OrderNumber(value);
    }

    public static implicit operator string(OrderNumber orderNumber) => orderNumber.Value;
}
