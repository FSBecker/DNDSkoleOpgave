using DNDSkoleOpgave.Characters;
using DNDSkoleOpgave.Combat;
using DNDSkoleOpgave.Enums;

namespace DNDSkoleOpgave.Items;

public sealed class ConsumableItem : CoreItem
{
    public ConsumableItem(string id, string name, IEnumerable<ActionEffect> effects)
        : base(id, name, ItemType.Consumable)
    {
        Effects = effects?.ToList() ?? throw new ArgumentNullException(nameof(effects));
    }

    public IReadOnlyList<ActionEffect> Effects { get; }

    public void Use(CoreCharacter user, CoreCharacter? selectedTarget = null)
    {
        ArgumentNullException.ThrowIfNull(user);
        foreach (ActionEffect effect in Effects)
        {
            effect.Apply(user, selectedTarget);
        }

        user.Inventory.Remove(this);
    }
}
