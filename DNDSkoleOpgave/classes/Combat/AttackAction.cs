using DNDSkoleOpgave.Enums;

namespace DNDSkoleOpgave.Combat;

public class AttackAction : CombatAction
{
    public AttackAction(
        string id,
        string name,
        int actionCost,
        int diceAmount,
        int diceSides,
        CharacterStat primaryStat)
        : base(id, name, actionCost, diceAmount, diceSides, primaryStat)
    {
    }
}
