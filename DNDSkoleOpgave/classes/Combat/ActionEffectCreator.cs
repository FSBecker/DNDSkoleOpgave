using DNDSkoleOpgave.Enums;

namespace DNDSkoleOpgave.Combat;

public static class ActionEffectCreator
{
    public static ActionEffect Create(
        ActionEffectType type,
        int amount,
        EffectTarget target = EffectTarget.Enemy,
        EffectTiming timing = EffectTiming.Instant,
        int durationTurns = 1) => new(type, amount, target, timing, durationTurns);
}
