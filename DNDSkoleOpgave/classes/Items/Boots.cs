using DNDSkoleOpgave.Enums;

namespace DNDSkoleOpgave.Items;

public sealed class Boots : EquipmentItem
{
    public Boots(string id, string name, int armorClass)
        : base(id, name, EquipmentSlot.Boots, armorClass)
    {
    }
}
