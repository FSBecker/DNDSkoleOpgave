using DNDSkoleOpgave.Characters;
using DNDSkoleOpgave.Enums;
using Newtonsoft.Json;

namespace DNDSkoleOpgave.Persistence;

public sealed class CharacterSaveData
{
    [JsonProperty("version", Required = Required.Always)]
    public int Version { get; set; } = 1;

    [JsonProperty("id", Required = Required.Always)]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name", Required = Required.Always)]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("level", Required = Required.Always)]
    public int Level { get; set; }

    [JsonProperty("class", Required = Required.Always)]
    public string ClassName { get; set; } = string.Empty;

    [JsonProperty("race", Required = Required.Always)]
    public string RaceName { get; set; } = string.Empty;

    [JsonProperty("baseStats", Required = Required.Always)]
    public int[] BaseStats { get; set; } = [];

    [JsonProperty("currentHealth", Required = Required.Always)]
    public int CurrentHealth { get; set; }

    [JsonProperty("deathRolls", Required = Required.Always)]
    public int DeathRolls { get; set; }

    [JsonProperty("actionPoints", Required = Required.Always)]
    public int ActionPoints { get; set; }

    [JsonProperty("inventoryItemIds", Required = Required.Always)]
    public List<string> InventoryItemIds { get; set; } = [];

    [JsonProperty("equipment", Required = Required.Always)]
    public List<EquipmentSaveData> Equipment { get; set; } = [];

    [JsonProperty("actionIds", Required = Required.Always)]
    public List<string> ActionIds { get; set; } = [];

    public static CharacterSaveData FromCharacter(PlayerCharacter character)
    {
        ArgumentNullException.ThrowIfNull(character);
        int[] baseStats = [.. character.StatArray];
        RemoveBuffs(baseStats, character.CharacterClass.StatBuffs);
        RemoveBuffs(baseStats, character.CharacterRace.StatBuffs);

        return new CharacterSaveData
        {
            Id = character.ID,
            Name = character.CharacterName,
            Level = character.Level,
            ClassName = character.CharacterClass.ClassName,
            RaceName = character.CharacterRace.RaceName,
            BaseStats = baseStats,
            CurrentHealth = character.CurrentHealth,
            DeathRolls = character.DeathRolls,
            ActionPoints = character.ActionPoints,
            InventoryItemIds = character.Inventory.Select(item => item.ID).ToList(),
            Equipment = character.Equipment.All
                .Select(entry => new EquipmentSaveData { Slot = entry.Key, ItemId = entry.Value.ID })
                .ToList(),
            ActionIds = character.Actions.Select(action => action.ID).ToList()
        };
    }

    private static void RemoveBuffs(int[] stats, IReadOnlyDictionary<CharacterStat, int> buffs)
    {
        foreach ((CharacterStat stat, int buff) in buffs)
        {
            stats[(int)stat] -= buff;
        }
    }
}
