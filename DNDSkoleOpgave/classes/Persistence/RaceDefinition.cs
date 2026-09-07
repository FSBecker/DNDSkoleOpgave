using DNDSkoleOpgave.Enums;
using Newtonsoft.Json;

namespace DNDSkoleOpgave.Persistence;

public sealed class RaceDefinition
{
    [JsonProperty("name", Required = Required.Always)]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("description", Required = Required.Always)]
    public string Description { get; set; } = string.Empty;

    [JsonProperty("enemyOnly")]
    public bool EnemyOnly { get; set; }

    [JsonProperty("statBuffs", Required = Required.Always)]
    public Dictionary<CharacterStat, int> StatBuffs { get; set; } = [];
}
