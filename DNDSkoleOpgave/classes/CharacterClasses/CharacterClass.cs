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

    public IEnumerable<string> GetActionIdsForLevel(int level)
    {
        List<string> actionIds = new();
        AddLevelLockedActions(actionIds, level);
        AddUnlockedActions(actionIds, level);
        return actionIds.Distinct(StringComparer.OrdinalIgnoreCase);
    }

    private void AddLevelLockedActions(List<string> actionIds, int level)
    {
        foreach (var entry in LevelLockedActions.OrderBy(entry => entry.Key))
        {
            if (entry.Key <= level)
                actionIds.AddRange(entry.Value);
        }
    }

    private void AddUnlockedActions(List<string> actionIds, int level)
    {
        foreach (var entry in LevelUnlocks.OrderBy(entry => entry.Key))
        {
            if (entry.Key <= level)
                actionIds.AddRange(entry.Value.ActionIds);
        }
    }

    public int GetBonusHealthForLevel(int level)
    {
        int bonusHealth = 0;
        foreach (var entry in LevelUnlocks)
        {
            if (entry.Key <= level)
                bonusHealth += entry.Value.HealthBonus;
        }
        return bonusHealth;
    }
}
