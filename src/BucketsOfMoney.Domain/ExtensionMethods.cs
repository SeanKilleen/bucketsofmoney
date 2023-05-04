namespace BucketsOfMoney.Domain;

public static class ExtensionMethods
{
    public static decimal MoneyRounded(this decimal value)
    {
        return Math.Round(value, 2, MidpointRounding.ToZero);
    }
}