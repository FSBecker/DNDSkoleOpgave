using DNDSkoleOpgave.Enums;
using Newtonsoft.Json;

namespace DNDSkoleOpgave.Persistence;

public sealed class ItemDefinition
{
    [JsonProperty("id", Required = Required.Always)]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name", Required = Required.Always)]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("itemType", Required = Required.Always)]
    public ItemType ItemType { get; set; }

    [JsonProperty("equipmentSlot", NullValueHandling = NullValueHandling.Ignore)]
    public EquipmentSlot? EquipmentSlot { get; set; }

    [JsonProperty("armorClass", NullValueHandling = NullValueHandling.Ignore)]
    public int? ArmorClass { get; set; }

    [JsonProperty("weaponType", NullValueHandling = NullValueHandling.Ignore)]
    public WeaponType? WeaponType { get; set; }

    [JsonProperty("actionIds", Required = Required.Always)]
    public List<string> ActionIds { get; set; } = [];

    [JsonProperty("effectIds")]
    public List<string> EffectIds { get; set; } = [];
}
