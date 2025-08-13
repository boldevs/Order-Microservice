namespace Orders.Orders.ValueObjects;

public record Accountid
{
    public Guid Value { get; }

    private Accountid(Guid value)
    {
        Value = value;
    }

    public static Accountid Of(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Account ID cannot be empty.");

        return new Accountid(value);
    }

    public static implicit operator Guid(Accountid accountId) => accountId.Value;
}
