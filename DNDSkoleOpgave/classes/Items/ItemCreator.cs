using DNDSkoleOpgave.Utilities;

namespace DNDSkoleOpgave.Items;

public static class ItemCreator
{
    public static CoreItem Create(string id) => ItemCatalog.Create(id);

    public static CoreItem Create(
        Persistence.ItemDefinition definition,
        Persistence.GameDefinitions? definitions = null)
    {
        if (definition.ItemType == Enums.ItemType.Consumable)
        {
            IEnumerable<Combat.ActionEffect> effects = definition.EffectIds.Select(effectId =>
            {
                Persistence.EffectDefinition effect = definitions?.Effects.First(value =>
                    value.Id.Equals(effectId, StringComparison.OrdinalIgnoreCase))
                    ?? throw new ArgumentException("Game definitions are required to create a consumable.");
                return Combat.ActionEffectCreator.Create(
                    effect.Type, effect.Amount, effect.Target, effect.Timing, effect.DurationTurns);
            });
            return new ConsumableItem(definition.Id, definition.Name, effects);
        }

        return definition.ItemType switch
        {
            Enums.ItemType.Weapon => new DefinedWeaponItem(definition),
            Enums.ItemType.Equipment or Enums.ItemType.Shield => new DefinedEquipmentItem(definition),
            _ => new DefinedItem(definition)
        };
    }
}
