using Newtonsoft.Json;

namespace DNDSkoleOpgave.Persistence;

public sealed class GameDefinitions
{
    [JsonProperty("version", Required = Required.Always)]
    public int Version { get; set; } = 1;

    [JsonProperty("classes", Required = Required.Always)]
    public List<ClassDefinition> AllClasses { get; set; } = [];

    [JsonProperty("races", Required = Required.Always)]
    public List<RaceDefinition> AllRaces { get; set; } = [];

    [JsonProperty("items", Required = Required.Always)]
    public List<ItemDefinition> AllItems { get; set; } = [];

    [JsonProperty("actions", Required = Required.Always)]
    public List<ActionDefinition> AllActions { get; set; } = [];

    [JsonProperty("effects")]
    public List<EffectDefinition> AllEffects { get; set; } = [];

    [JsonProperty("enemies")]
    public List<EnemyDefinition> AllEnemies { get; set; } = [];
}
