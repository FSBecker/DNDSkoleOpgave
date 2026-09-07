using DNDSkoleOpgave.Enums;

namespace DNDSkoleOpgave.Items;

public abstract class EquipmentItem : CoreItem
{
    protected EquipmentItem(
        string id,
        string name,
        EquipmentSlot equipmentSlot,
        int armorClass,
        ItemType itemType = ItemType.Equipment)
        : base(id, name, itemType)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(armorClass);
        EquipmentSlot = equipmentSlot;
        ArmorClass = armorClass;
    }

    public EquipmentSlot EquipmentSlot { get; }
    public int ArmorClass { get; }
}
