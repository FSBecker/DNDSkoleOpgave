namespace DNDSkoleOpgave.Combat.Spells;

public sealed class WeakFireball : SpellAction
{
    public WeakFireball() : base("weak-fireball", "Weak Fireball", 1, 1, 4, false)
    {
    }

    public int SavingThrowDifficulty { get; } = 10;
    public int BurningDamageDiceSides { get; } = 4;
    public int BurningDurationDiceSides { get; } = 4;
}
