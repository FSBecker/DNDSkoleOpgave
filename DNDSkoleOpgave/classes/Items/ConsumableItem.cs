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
    public bool IsHealing => Effects.Any(effect => effect.Type == ActionEffectType.Healing);

    public void Use(CoreCharacter user, CoreCharacter? selectedTarget = null)
    {
        ArgumentNullException.ThrowIfNull(user);
        if (user.IsDefeated || !user.Inventory.Contains(this))
            throw new InvalidOperationException("A living character must own the item before using it.");
        if (Effects.Any(effect => effect.Target == EffectTarget.Enemy) &&
            (selectedTarget is null || selectedTarget.IsDefeated))
            throw new InvalidOperationException("Choose a living enemy first.");
        foreach (ActionEffect effect in Effects)
        {
            effect.Apply(user, selectedTarget);
        }

        user.RemoveItem(this);
    }
}
