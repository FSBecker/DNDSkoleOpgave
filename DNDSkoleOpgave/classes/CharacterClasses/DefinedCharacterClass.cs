using DNDSkoleOpgave.Persistence;

namespace DNDSkoleOpgave.CharacterClasses;

public sealed class DefinedCharacterClass : CharacterClass
{
    public DefinedCharacterClass(ClassDefinition definition)
        : base(definition.Name, definition.Description, definition.BaseHealth, definition.HealthPerLevel)
    {
        foreach (var buff in definition.StatBuffs) StatBuffs[buff.Key] = buff.Value;
        StartingItemIds.AddRange(definition.StartingItemIds);
        foreach (var unlock in definition.LevelLockedActionIds)
            LevelLockedActions[unlock.Key] = [.. unlock.Value];
        foreach (var entry in definition.LevelUnlocks)
        {
            LevelUnlock unlock = new() { HealthBonus = entry.Value.HealthBonus };
            unlock.ActionIds.AddRange(entry.Value.ActionIds);
            unlock.ItemIds.AddRange(entry.Value.ItemIds);
            foreach (var bonus in entry.Value.StatBonuses) unlock.StatBonuses[bonus.Key] = bonus.Value;
            LevelUnlocks[entry.Key] = unlock;
        }
    }
}
