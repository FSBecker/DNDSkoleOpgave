namespace DNDSkoleOpgave.Utilities;

public sealed class FixedDiceRoller : IDiceRoller
{
    private readonly int _value;

    public FixedDiceRoller(int value)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(value, 1);
        _value = value;
    }

    public int Roll(int sides)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(sides, 1);
        return Math.Min(_value, sides);
    }
}
