using DNDSkoleOpgave.Enums;

namespace DNDSkoleOpgave.CharacterClasses;

public sealed class LevelUnlock
{
    public int HealthBonus { get; set; }
    public Dictionary<CharacterStat, int> StatBonuses { get; } = [];
    public List<string> ActionIds { get; } = [];
    public List<string> ItemIds { get; } = [];
}
