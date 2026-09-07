using DNDSkoleOpgave.Enums;
using Newtonsoft.Json;

namespace DNDSkoleOpgave.Persistence;

public sealed class EquipmentSaveData
{
    [JsonProperty("slot", Required = Required.Always)]
    public EquipmentSlot Slot { get; set; }

    [JsonProperty("itemId", Required = Required.Always)]
    public string ItemId { get; set; } = string.Empty;
}
