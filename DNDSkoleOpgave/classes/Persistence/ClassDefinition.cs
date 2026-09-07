using DNDSkoleOpgave.Enums;
using Newtonsoft.Json;

namespace DNDSkoleOpgave.Persistence;

public sealed class ClassDefinition
{
    [JsonProperty("name", Required = Required.Always)]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("description", Required = Required.Always)]
    public string Description { get; set; } = string.Empty;

    [JsonProperty("baseHealth", Required = Required.Always)]
    public int BaseHealth { get; set; }

    [JsonProperty("healthPerLevel", Required = Required.Always)]
    public int HealthPerLevel { get; set; }

    [JsonProperty("statBuffs", Required = Required.Always)]
    public Dictionary<CharacterStat, int> StatBuffs { get; set; } = [];

    [JsonProperty("startingItemIds", Required = Required.Always)]
    public List<string> StartingItemIds { get; set; } = [];

    [JsonProperty("levelLockedActionIds", Required = Required.Always)]
    public Dictionary<int, List<string>> LevelLockedActionIds { get; set; } = [];
}
