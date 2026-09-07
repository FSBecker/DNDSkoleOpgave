namespace DNDSkoleOpgave.Combat.Spells;

public sealed class WeakFireball : SpellAction
{
    public WeakFireball() : base("weak-fireball", "Weak Fireball", 1, 1, 4, false)
    {
    }

    public override int? SavingThrowDifficulty => 10;
    public override int? BurningDamageDiceSides => 2;
    public override int? BurningDurationDiceSides => 2;
}
