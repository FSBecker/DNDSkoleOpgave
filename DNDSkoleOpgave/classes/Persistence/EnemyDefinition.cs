using Newtonsoft.Json;

namespace DNDSkoleOpgave.Persistence;

public sealed class EnemyDefinition
{
    [JsonProperty("id", Required = Required.Always)]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name", Required = Required.Always)]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("className", Required = Required.Always)]
    public string ClassName { get; set; } = string.Empty;

    [JsonProperty("raceName", Required = Required.Always)]
    public string RaceName { get; set; } = string.Empty;

    [JsonProperty("level", Required = Required.Always)]
    public int Level { get; set; } = 1;

    [JsonProperty("baseStats", Required = Required.Always)]
    public int[] BaseStats { get; set; } = [10, 10, 10, 10];

    [JsonProperty("lootTable")]
    public List<LootEntryDefinition> LootTable { get; set; } = [];
}
