namespace DNDSkoleOpgave.Combat;

public sealed class ActiveEffect
{
    public ActiveEffect(ActionEffect effect)
    {
        Effect = effect ?? throw new ArgumentNullException(nameof(effect));
        RemainingTurns = effect.DurationTurns;
    }

    public ActionEffect Effect { get; }
    public int RemainingTurns { get; private set; }
    public bool IsFinished => RemainingTurns <= 0;

    public void ProcessTurn(Characters.IDamageable target)
    {
        if (IsFinished) return;
        Effect.Apply(target);
        RemainingTurns--;
    }
}
