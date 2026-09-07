using DNDSkoleOpgave.Combat;
using DNDSkoleOpgave.Enums;

namespace DNDSkoleOpgave.Utilities;

public static class ActionCatalog
{
    private static readonly Dictionary<string, CombatAction> Actions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["bow-shot"] = new("bow-shot", "Bow Shot", 1, 1, 8, CharacterStat.Dexterity),
        ["short-sword-strike"] = new("short-sword-strike", "Short Sword Strike", 1, 1, 6, CharacterStat.Dexterity),
        ["axe-swing"] = new("axe-swing", "Axe Swing", 1, 1, 12, CharacterStat.Strength),
        ["staff-strike"] = new("staff-strike", "Staff Strike", 1, 1, 6, CharacterStat.Intelligence)
    };

    public static IReadOnlyDictionary<string, CombatAction> All => Actions;

    public static CombatAction Get(string id) =>
        Actions.TryGetValue(id, out CombatAction? action)
            ? action
            : throw new KeyNotFoundException($"No combat action has the ID '{id}'.");
}
