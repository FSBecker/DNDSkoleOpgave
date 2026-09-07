using DNDSkoleOpgave.Utilities;

namespace DNDSkoleOpgave.Combat;

public static class CombatActionCreator
{
    public static CombatAction Create(string id) => ActionCatalog.Get(id);

    public static CombatAction Create(Persistence.ActionDefinition definition, Persistence.GameDefinitions definitions)
    {
        CombatAction action = definition.Kind.ToLowerInvariant() switch
        {
            "spell" => new DefinedSpellAction(definition),
            "ability" => new AbilityAction(definition.Id, definition.Name, definition.ActionCost,
                definition.DiceAmount, definition.DiceSides, definition.PrimaryStat),
            _ => new AttackAction(definition.Id, definition.Name, definition.ActionCost,
                definition.DiceAmount, definition.DiceSides, definition.PrimaryStat)
        };

        List<Persistence.EffectDefinition> effects = new(definition.Effects);
        foreach (string effectId in definition.EffectIds)
        {
            Persistence.EffectDefinition effect = definitions.AllEffects.First(value =>
                value.Id.Equals(effectId, StringComparison.OrdinalIgnoreCase));
            if (!effects.Any(value => value.Id.Equals(effect.Id, StringComparison.OrdinalIgnoreCase)))
                effects.Add(effect);
        }

        foreach (Persistence.EffectDefinition effect in effects)
        {
            action.Effects.Add(ActionEffectCreator.Create(
                effect.Type, effect.Amount, effect.Target, effect.Timing, effect.DurationTurns));
        }

        return action;
    }
}
