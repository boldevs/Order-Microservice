namespace Orders.Orders.ValueObjects;

public record TotalPrice
{
    public decimal Value { get; }

    private TotalPrice(decimal value)
    {
        Value = value;
    }

    public static TotalPrice Of(decimal value)
    {
        if (value < 0)
            throw new ArgumentException("Total price cannot be negative.");

        return new TotalPrice(decimal.Round(value, 2));
    }

    public static TotalPrice Zero => new(0);

    public static TotalPrice operator +(TotalPrice a, TotalPrice b)
        => Of(a.Value + b.Value);

    public static TotalPrice operator -(TotalPrice a, TotalPrice b)
    {
        var result = a.Value - b.Value;
        if (result < 0)
            throw new InvalidOperationException("Resulting total price cannot be negative.");
        return Of(result);
    }

    public static bool operator >(TotalPrice a, TotalPrice b) => a.Value > b.Value;
    public static bool operator <(TotalPrice a, TotalPrice b) => a.Value < b.Value;

    public static implicit operator decimal(TotalPrice price) => price.Value;

    public override string ToString() => $"{Value:C}";
}
