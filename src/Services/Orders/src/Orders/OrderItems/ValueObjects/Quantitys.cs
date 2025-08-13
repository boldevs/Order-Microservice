namespace Orders.OrderItems.ValueObjects;
public record Quantitys(int Value)
{
    public static Quantitys Of(int value)
    {
        if (value <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");
        return new Quantitys(value);
    }

    public static implicit operator int(Quantitys quantity) => quantity.Value;
}
