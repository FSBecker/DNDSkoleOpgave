using Newtonsoft.Json;

namespace DNDSkoleOpgave.Persistence;

public sealed class GameDefinitions
{
    [JsonProperty("version", Required = Required.Always)]
    public int Version { get; set; } = 1;

    [JsonProperty("classes", Required = Required.Always)]
    public List<ClassDefinition> Classes { get; set; } = [];

    [JsonProperty("races", Required = Required.Always)]
    public List<RaceDefinition> Races { get; set; } = [];

    [JsonProperty("items", Required = Required.Always)]
    public List<ItemDefinition> Items { get; set; } = [];

    [JsonProperty("actions", Required = Required.Always)]
    public List<ActionDefinition> Actions { get; set; } = [];

    [JsonProperty("effects")]
    public List<EffectDefinition> Effects { get; set; } = [];

    [JsonProperty("enemies")]
    public List<EnemyDefinition> Enemies { get; set; } = [];
}
