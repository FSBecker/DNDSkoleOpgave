using DNDSkoleOpgave.Enums;

namespace DNDSkoleOpgave.Items;

public abstract class CoreItem
{
    protected CoreItem(string id, string name, ItemType itemType)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ID = id;
        Name = name;
        ItemType = itemType;
    }

    public string ID { get; }
    public string Name { get; }
    public ItemType ItemType { get; }
}
