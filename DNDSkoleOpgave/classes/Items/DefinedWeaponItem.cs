using DNDSkoleOpgave.Persistence;

namespace DNDSkoleOpgave.Items;

public sealed class DefinedWeaponItem : WeaponItem
{
    public DefinedWeaponItem(ItemDefinition definition)
        : base(definition.Id, definition.Name,
            definition.WeaponType ?? throw new ArgumentException("A weapon type is required."),
            [.. definition.ActionIds])
    {
    }
}
