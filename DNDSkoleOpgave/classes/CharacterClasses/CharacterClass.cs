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
    public Dictionary<int, string> LevelLockedActions { get; } = [];
}
