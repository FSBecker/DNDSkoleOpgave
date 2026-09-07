using DNDSkoleOpgave.Enums;

namespace DNDSkoleOpgave.Items;

public sealed class Orb : EquipmentItem
{
    public Orb() : base("orb", "Orb", EquipmentSlot.OffhandWeapon, 0)
    {
    }
}
