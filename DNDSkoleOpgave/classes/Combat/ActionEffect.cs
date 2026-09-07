using DNDSkoleOpgave.Characters;
using DNDSkoleOpgave.Enums;

namespace DNDSkoleOpgave.Combat;

public sealed class ActionEffect
{
    public ActionEffect(
        ActionEffectType type,
        int amount,
        EffectTarget target = EffectTarget.Enemy,
        EffectTiming timing = EffectTiming.Instant,
        int durationTurns = 1)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        ArgumentOutOfRangeException.ThrowIfLessThan(durationTurns, 1);
        Type = type;
        Amount = amount;
        Target = target;
        Timing = timing;
        DurationTurns = durationTurns;
    }

    public ActionEffectType Type { get; }
    public int Amount { get; }
    public EffectTarget Target { get; }
    public EffectTiming Timing { get; }
    public int DurationTurns { get; }

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

    public void Apply(CoreCharacter source, CoreCharacter? selectedTarget = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        CoreCharacter target = Target == EffectTarget.Self
            ? source
            : selectedTarget ?? throw new ArgumentNullException(nameof(selectedTarget));

        if (Timing == EffectTiming.Instant)
        {
            Apply((IDamageable)target);
        }
        else
        {
            target.AddActiveEffect(this);
        }
    }
}
