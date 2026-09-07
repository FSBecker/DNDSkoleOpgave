using DNDSkoleOpgave.Enums;
using Newtonsoft.Json;

namespace DNDSkoleOpgave.Persistence;

public sealed class EffectDefinition
{
    [JsonProperty("id", Required = Required.Always)]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name", Required = Required.Always)]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("type", Required = Required.Always)]
    public ActionEffectType Type { get; set; }

    [JsonProperty("amount", Required = Required.Always)]
    public int Amount { get; set; }

    [JsonProperty("target")]
    public EffectTarget Target { get; set; } = EffectTarget.Enemy;

    [JsonProperty("timing")]
    public EffectTiming Timing { get; set; } = EffectTiming.Instant;

    [JsonProperty("durationTurns")]
    public int DurationTurns { get; set; } = 1;
}
