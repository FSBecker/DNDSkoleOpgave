using DNDSkoleOpgave.CharacterClasses;
using DNDSkoleOpgave.Races;
using DNDSkoleOpgave.Items;

namespace DNDSkoleOpgave.Characters;

public sealed class EnemyCharacter : CoreCharacter
{
    public List<LootEntry> LootTable { get; } = [];

    public EnemyCharacter(
        string id,
        string characterName,
        int level,
        CharacterClass characterClass,
        CharacterRace characterRace,
        int[]? baseStats = null)
        : base(id, characterName, level, characterClass, characterRace, baseStats)
    {
    }

    public IReadOnlyList<CoreItem> RollLoot() => LootTable
        .Select(entry => entry.Roll())
        .OfType<CoreItem>()
        .ToList();

    public IReadOnlyList<CoreItem> RollLoot(Utilities.IDiceRoller dice)
    {
        List<CoreItem> loot = new();
        foreach (LootEntry entry in LootTable)
        {
            CoreItem? item = entry.Roll(dice);
            if (item is not null)
                loot.Add(item);
        }
        return loot;
    }
}
