using Newtonsoft.Json;

namespace DNDSkoleOpgave.Persistence;

public sealed class LootEntryDefinition
{
    [JsonProperty("itemId", Required = Required.Always)]
    public string ItemId { get; set; } = string.Empty;

    [JsonProperty("chancePercent", Required = Required.Always)]
    public int ChancePercent { get; set; }
}
