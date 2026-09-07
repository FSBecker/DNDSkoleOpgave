namespace DNDSkoleOpgave.Combat;

public abstract class SpellAction : CombatAction
{
    protected SpellAction(
        string id,
        string name,
        int actionCost,
        int diceAmount,
        int diceSides,
        bool targetsAlly)
        : base(id, name, actionCost, diceAmount, diceSides, Enums.CharacterStat.Intelligence)
    {
        TargetsAlly = targetsAlly;
    }

    public bool TargetsAlly { get; }
    public virtual int? SavingThrowDifficulty => null;
    public virtual int? BurningDamageDiceSides => null;
    public virtual int? BurningDurationDiceSides => null;
}
