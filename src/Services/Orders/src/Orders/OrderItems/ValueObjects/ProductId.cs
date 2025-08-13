namespace Orders.OrderItems.ValueObjects;
public record ProductId(Guid Value)
{
    public static ProductId Of(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ProductId cannot be empty.");
        return new ProductId(value);
    }

    public static implicit operator Guid(ProductId id) => id.Value;
}
