using DNDSkoleOpgave.Enums;

namespace DNDSkoleOpgave.Items;

public sealed class Pants : EquipmentItem
{
    public Pants(string id, string name, int armorClass)
        : base(id, name, EquipmentSlot.Pants, armorClass)
    {
    }
}
