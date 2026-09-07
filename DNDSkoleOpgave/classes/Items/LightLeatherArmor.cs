using DNDSkoleOpgave.Enums;

namespace DNDSkoleOpgave.Items;

public sealed class LightLeatherArmor : EquipmentItem
{
    public LightLeatherArmor()
        : base("light-leather-armor", "Light Leather Armor", EquipmentSlot.ChestArmor, 2)
    {
    }
}
