using DNDSkoleOpgave.Combat;
using DNDSkoleOpgave.Combat.Spells;
using DNDSkoleOpgave.Enums;

namespace DNDSkoleOpgave.Utilities;

public static class ActionCatalog
{
    private static readonly Dictionary<string, CombatAction> Actions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["bow-shot"] = new AttackAction("bow-shot", "Bow Shot", 1, 1, 8, CharacterStat.Dexterity),
        ["short-sword-strike"] = new AttackAction("short-sword-strike", "Short Sword Strike", 1, 1, 6, CharacterStat.Dexterity),
        ["axe-swing"] = new AttackAction("axe-swing", "Axe Swing", 1, 1, 8, CharacterStat.Strength),
        ["great-axe-swing"] = new AttackAction("great-axe-swing", "Great Axe Swing", 1, 1, 12, CharacterStat.Strength),
        ["staff-strike"] = new AttackAction("staff-strike", "Staff Strike", 1, 1, 6, CharacterStat.Strength),
        ["healing-hand"] = new HealingHand(),
        ["weak-fireball"] = new WeakFireball(),
        ["witch-bolt"] = new WitchBolt()
    };

    public static IReadOnlyDictionary<string, CombatAction> AllActions => Actions;

    public static void Load(Persistence.GameDefinitions definitions)
    {
        Actions.Clear();
        foreach (Persistence.ActionDefinition action in definitions.AllActions)
            Actions.Add(action.Id, CombatActionCreator.Create(action, definitions));
    }

    public static CombatAction Get(string id) =>
        Actions.TryGetValue(id, out CombatAction? action)
            ? action
            : throw new KeyNotFoundException($"No combat action has the ID '{id}'.");
}
