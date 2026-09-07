using DNDSkoleOpgave.Enums;
using Newtonsoft.Json;

namespace DNDSkoleOpgave.Persistence;

public sealed class ActionDefinition
{
    [JsonProperty("id", Required = Required.Always)]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name", Required = Required.Always)]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("kind", Required = Required.Always)]
    public string Kind { get; set; } = string.Empty;

    [JsonProperty("actionCost", Required = Required.Always)]
    public int ActionCost { get; set; }

    [JsonProperty("diceAmount", Required = Required.Always)]
    public int DiceAmount { get; set; }

    [JsonProperty("diceSides", Required = Required.Always)]
    public int DiceSides { get; set; }

    [JsonProperty("primaryStat", Required = Required.Always)]
    public CharacterStat PrimaryStat { get; set; }

    [JsonProperty("targetsAlly", NullValueHandling = NullValueHandling.Ignore)]
    public bool? TargetsAlly { get; set; }

    [JsonProperty("savingThrowDifficulty", NullValueHandling = NullValueHandling.Ignore)]
    public int? SavingThrowDifficulty { get; set; }

    [JsonProperty("burningDamageDiceSides", NullValueHandling = NullValueHandling.Ignore)]
    public int? BurningDamageDiceSides { get; set; }

    [JsonProperty("burningDurationDiceSides", NullValueHandling = NullValueHandling.Ignore)]
    public int? BurningDurationDiceSides { get; set; }

    [JsonProperty("effects", Required = Required.Always)]
    public List<EffectDefinition> Effects { get; set; } = [];

    [JsonProperty("effectIds")]
    public List<string> EffectIds { get; set; } = [];
}
