using DNDSkoleOpgave.Enums;
using DNDSkoleOpgave.Items;

namespace DNDSkoleOpgave.Combat;

public sealed class CombatChoice
{
    public CombatChoice(CombatAction action) => Action = action ?? throw new ArgumentNullException(nameof(action));
    public CombatChoice(ConsumableItem item) => Item = item ?? throw new ArgumentNullException(nameof(item));

    public CombatAction? Action { get; }
    public ConsumableItem? Item { get; }
    public string Name => Action?.Name ?? Item!.Name;
    public int ActionCost => Action?.ActionCost ?? 1;

    public bool NeedsEnemyTarget
    {
        get
        {
            if (Item is not null)
                return Item.Effects.Any(effect => effect.Target == EffectTarget.Enemy);
            if (Action is not null && Action.Effects.Any(effect => effect.Target == EffectTarget.Enemy))
                return true;
            if (Action is SpellAction spell)
                return !spell.TargetsAlly;
            return true;
        }
    }

    public bool UsesHealingAbility
    {
        get
        {
            if (Action is SpellAction spell && spell.TargetsAlly)
                return true;
            return Action is not null && Action.Effects.Any(effect => effect.Type == ActionEffectType.Healing);
        }
    }
}
