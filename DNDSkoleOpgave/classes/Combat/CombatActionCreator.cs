using DNDSkoleOpgave.Utilities;

namespace DNDSkoleOpgave.Combat;

public static class CombatActionCreator
{
    public static CombatAction Create(string id) => ActionCatalog.Get(id);
}
