namespace DNDSkoleOpgave.Utilities;

public static class DiceRoller
{
    public static int Roll(int amount, int sides) => Roll(amount, sides, new RandomDiceRoller());

    public static int Roll(int amount, int sides, IDiceRoller dice)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(amount, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(sides, 2);

        int total = 0;
        for (int roll = 0; roll < amount; roll++)
        {
            total += dice.Roll(sides);
        }

        return total;
    }
}
