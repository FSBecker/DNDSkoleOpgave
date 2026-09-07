using DNDSkoleOpgave.Enums;

namespace DNDSkoleOpgave.CharacterClasses;

public abstract class CharacterClass
{
    protected CharacterClass(string className, string description, int baseHealth, int healthPerLevel)
    {
        ClassName = className;
        Description = description;
        BaseHealth = baseHealth;
        HealthPerLevel = healthPerLevel;
    }

    public string ClassName { get; }
    public string Description { get; }
    public int BaseHealth { get; }
    public int HealthPerLevel { get; }
    public Dictionary<CharacterStat, int> StatBuffs { get; } = [];
    public List<string> StartingItemIds { get; } = [];
    public Dictionary<int, List<string>> LevelLockedActions { get; } = [];
    public Dictionary<int, LevelUnlock> LevelUnlocks { get; } = [];

    public IEnumerable<string> GetActionIdsForLevel(int level) =>
        LevelLockedActions
            .Where(entry => entry.Key <= level)
            .OrderBy(entry => entry.Key)
            .SelectMany(entry => entry.Value)
            .Concat(LevelUnlocks
                .Where(entry => entry.Key <= level)
                .OrderBy(entry => entry.Key)
                .SelectMany(entry => entry.Value.ActionIds))
            .Distinct(StringComparer.OrdinalIgnoreCase);

    public int GetBonusHealthForLevel(int level) => LevelUnlocks
        .Where(entry => entry.Key <= level)
        .Sum(entry => entry.Value.HealthBonus);
}
