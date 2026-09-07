using DNDSkoleOpgave.Persistence;

namespace DNDSkoleOpgave.Items;

public sealed class DefinedEquipmentItem : EquipmentItem
{
    public DefinedEquipmentItem(ItemDefinition definition)
        : base(definition.Id, definition.Name,
            definition.EquipmentSlot ?? throw new ArgumentException("An equipment slot is required."),
            definition.ArmorClass ?? 0, definition.ItemType)
    {
    }
}
