namespace Orders.OrderItems.ValueObjects;
public record UnitPrice(decimal Value)
{
    public static UnitPrice Of(decimal value)
    {
        if (value < 0)
            throw new ArgumentException("Unit price cannot be negative.");
        return new UnitPrice(decimal.Round(value, 2));
    }

    public static implicit operator decimal(UnitPrice price) => price.Value;
}
