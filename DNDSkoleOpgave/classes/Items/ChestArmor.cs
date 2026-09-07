using DNDSkoleOpgave.Enums;

namespace DNDSkoleOpgave.Items;

public sealed class ChestArmor : EquipmentItem
{
    public ChestArmor(string id, string name, int armorClass)
        : base(id, name, EquipmentSlot.ChestArmor, armorClass)
    {
    }
}
