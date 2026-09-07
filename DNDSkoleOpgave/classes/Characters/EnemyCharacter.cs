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
}
