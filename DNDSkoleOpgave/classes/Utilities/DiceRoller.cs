namespace DNDSkoleOpgave.Utilities;

public static class DiceRoller
{
    public static int Roll(int amount, int sides)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(amount, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(sides, 2);

        int total = 0;
        for (int roll = 0; roll < amount; roll++)
        {
            total += Random.Shared.Next(1, sides + 1);
        }

        return total;
    }
}
