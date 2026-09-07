namespace DNDSkoleOpgave.Utilities;

public sealed class RandomDiceRoller : IDiceRoller
{
    public int Roll(int sides)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(sides, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(sides, int.MaxValue - 1);
        return Random.Shared.Next(1, sides + 1);
    }
}
