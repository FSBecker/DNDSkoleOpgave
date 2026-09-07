using DNDSkoleOpgave.Enums;

namespace DNDSkoleOpgave.Combat;

public class AbilityAction : CombatAction
{
    public AbilityAction(
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
