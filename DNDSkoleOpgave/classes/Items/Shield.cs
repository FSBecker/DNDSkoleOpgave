using DNDSkoleOpgave.Enums;

namespace DNDSkoleOpgave.Items;

public sealed class Shield : EquipmentItem
{
    public Shield(string id = "wooden-shield", string name = "Wooden Shield", int armorClass = 2)
        : base(id, name, EquipmentSlot.OffhandWeapon, armorClass, ItemType.Shield)
    {
    }
}
