using DNDSkoleOpgave.Persistence;

namespace DNDSkoleOpgave.Combat;

public sealed class DefinedSpellAction : SpellAction
{
    public DefinedSpellAction(ActionDefinition definition)
        : base(definition.Id, definition.Name, definition.ActionCost, definition.DiceAmount,
            definition.DiceSides, definition.TargetsAlly ?? false)
    {
        SavingThrowDifficulty = definition.SavingThrowDifficulty;
        BurningDamageDiceSides = definition.BurningDamageDiceSides;
        BurningDurationDiceSides = definition.BurningDurationDiceSides;
    }

    public int? SavingThrowDifficulty { get; }
    public int? BurningDamageDiceSides { get; }
    public int? BurningDurationDiceSides { get; }
}
