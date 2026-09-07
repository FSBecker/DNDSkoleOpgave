using DNDSkoleOpgave.Enums;
using DNDSkoleOpgave.Utilities;

namespace DNDSkoleOpgave.Combat;

public abstract class CombatAction
{
    public CombatAction(
        string id,
        string name,
        int actionCost,
        int diceAmount,
        int diceSides,
        CharacterStat primaryStat)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfLessThan(actionCost, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(diceAmount, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(diceSides, 2);

        ID = id;
        Name = name;
        ActionCost = actionCost;
        DiceAmount = diceAmount;
        DiceSides = diceSides;
        PrimaryStat = primaryStat;
    }

    public string ID { get; }
    public string Name { get; }
    public int ActionCost { get; }
    public int DiceAmount { get; }
    public int DiceSides { get; }
    public CharacterStat PrimaryStat { get; }
    public List<ActionEffect> Effects { get; } = [];

    public int RollDamage() => DiceRoller.Roll(DiceAmount, DiceSides);

    public int RollDamage(IDiceRoller dice) => DiceRoller.Roll(DiceAmount, DiceSides, dice);
}
