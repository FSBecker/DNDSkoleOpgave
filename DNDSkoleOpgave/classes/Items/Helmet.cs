using DNDSkoleOpgave.Enums;

namespace DNDSkoleOpgave.Items;

public sealed class Helmet : EquipmentItem
{
    public Helmet(string id, string name, int armorClass)
        : base(id, name, EquipmentSlot.Helmet, armorClass)
    {
    }
}
