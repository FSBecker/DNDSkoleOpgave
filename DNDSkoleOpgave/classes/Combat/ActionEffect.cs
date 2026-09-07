using DNDSkoleOpgave.Characters;
using DNDSkoleOpgave.Enums;

namespace DNDSkoleOpgave.Combat;

public sealed class ActionEffect
{
    public ActionEffect(ActionEffectType type, int amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        Type = type;
        Amount = amount;
    }

    public ActionEffectType Type { get; }
    public int Amount { get; }

    public void Apply(IDamageable target)
    {
        ArgumentNullException.ThrowIfNull(target);

        if (Type == ActionEffectType.Damage)
        {
            target.TakeDamage(Amount);
        }
        else
        {
            target.Heal(Amount);
        }
    }
}
