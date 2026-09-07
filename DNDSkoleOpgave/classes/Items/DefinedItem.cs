using DNDSkoleOpgave.Persistence;

namespace DNDSkoleOpgave.Items;

public sealed class DefinedItem : CoreItem
{
    public DefinedItem(ItemDefinition definition) : base(definition.Id, definition.Name, definition.ItemType)
    {
    }
}
