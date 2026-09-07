using DNDSkoleOpgave.Enums;

namespace DNDSkoleOpgave.Combat;

public static class ActionEffectCreator
{
    public static ActionEffect Create(ActionEffectType type, int amount) => new(type, amount);
}
