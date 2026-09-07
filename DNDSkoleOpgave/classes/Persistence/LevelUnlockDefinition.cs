using DNDSkoleOpgave.Enums;
using Newtonsoft.Json;

namespace DNDSkoleOpgave.Persistence;

public sealed class LevelUnlockDefinition
{
    [JsonProperty("healthBonus")]
    public int HealthBonus { get; set; }

    [JsonProperty("statBonuses")]
    public Dictionary<CharacterStat, int> StatBonuses { get; set; } = [];

    [JsonProperty("actionIds")]
    public List<string> ActionIds { get; set; } = [];

    [JsonProperty("itemIds")]
    public List<string> ItemIds { get; set; } = [];
}
