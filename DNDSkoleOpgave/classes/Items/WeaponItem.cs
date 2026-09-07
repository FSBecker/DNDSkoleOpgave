using DNDSkoleOpgave.Combat;
using DNDSkoleOpgave.Enums;
using DNDSkoleOpgave.Utilities;

namespace DNDSkoleOpgave.Items;

public abstract class WeaponItem : EquipmentItem
{
    protected WeaponItem(string id, string name, WeaponType weaponType, params string[] actionIds)
        : base(id, name, EquipmentSlot.MainWeapon, 0, ItemType.Weapon)
    {
        WeaponType = weaponType;
        ActionIds = actionIds;
    }

    public WeaponType WeaponType { get; }
    public IReadOnlyList<string> ActionIds { get; }

    public IEnumerable<CombatAction> GetActions() =>
        ActionIds.Select(ActionCatalog.Get);
}
