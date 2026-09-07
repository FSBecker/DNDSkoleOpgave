using DNDSkoleOpgave.Enums;

namespace DNDSkoleOpgave.Items;

public sealed class EquipmentSlots
{
    private readonly Dictionary<EquipmentSlot, EquipmentItem> _items = [];

    public EquipmentItem? this[EquipmentSlot slot] => _items.GetValueOrDefault(slot);
    public IReadOnlyDictionary<EquipmentSlot, EquipmentItem> AllEquipment => _items;

    public void Clear() => _items.Clear();

    internal EquipmentItem? Equip(EquipmentItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        EquipmentItem? replacedItem = this[item.EquipmentSlot];
        _items[item.EquipmentSlot] = item;
        return replacedItem;
    }

    internal EquipmentItem? Unequip(EquipmentSlot slot)
    {
        if (!_items.Remove(slot, out EquipmentItem? item))
        {
            return null;
        }

        return item;
    }

    public int GetTotalArmorClass() => _items.Values.Sum(item => item.ArmorClass);
}
