using DNDSkoleOpgave.CharacterClasses;
using DNDSkoleOpgave.Characters;
using DNDSkoleOpgave.Combat;
using DNDSkoleOpgave.Items;
using DNDSkoleOpgave.Races;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DNDSkoleOpgave.Persistence;

public sealed class CharacterJsonRepository
{
    private static readonly JsonSerializerSettings Settings = new()
    {
        Formatting = Formatting.Indented,
        MissingMemberHandling = MissingMemberHandling.Error,
        Converters = { new StringEnumConverter() }
    };

    public CharacterJsonRepository(string saveDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(saveDirectory);
        SaveDirectory = Path.GetFullPath(saveDirectory);
    }

    public string SaveDirectory { get; }

    public string Save(PlayerCharacter character)
    {
        ArgumentNullException.ThrowIfNull(character);
        Directory.CreateDirectory(SaveDirectory);
        string path = GetPath(character.ID);
        string json = JsonConvert.SerializeObject(CharacterSaveData.FromCharacter(character), Settings);
        File.WriteAllText(path, json);
        return path;
    }

    public PlayerCharacter Load(string characterId)
    {
        string json = File.ReadAllText(GetPath(characterId));
        CharacterSaveData data = JsonConvert.DeserializeObject<CharacterSaveData>(json, Settings)
            ?? throw new JsonSerializationException("The character save was empty.");
        return CreateCharacter(data);
    }

    public IReadOnlyList<string> GetSavedCharacterIds()
    {
        if (!Directory.Exists(SaveDirectory))
        {
            return [];
        }

        return Directory.GetFiles(SaveDirectory, "*.json")
            .Select(path => Path.GetFileNameWithoutExtension(path)!)
            .Order()
            .ToList();
    }

    private static PlayerCharacter CreateCharacter(CharacterSaveData data)
    {
        CharacterClass characterClass = CharacterClassCreator.Create(data.ClassName);
        CharacterRace race = CharacterRaceCreator.Create(data.RaceName);
        PlayerCharacter character = new(data.Id, data.Name, data.Level, characterClass, race, data.BaseStats);

        foreach (string itemId in data.InventoryItemIds)
        {
            character.Inventory.Add(ItemCreator.Create(itemId));
        }

        foreach (EquipmentSaveData savedEquipment in data.Equipment)
        {
            EquipmentItem item = character.Inventory
                .OfType<EquipmentItem>()
                .FirstOrDefault(candidate => candidate.ID.Equals(savedEquipment.ItemId, StringComparison.OrdinalIgnoreCase))
                ?? ItemCreator.Create(savedEquipment.ItemId) as EquipmentItem
                ?? throw new JsonSerializationException($"Equipped item '{savedEquipment.ItemId}' is not equipment.");

            if (item.EquipmentSlot != savedEquipment.Slot)
            {
                throw new JsonSerializationException($"Item '{item.ID}' cannot use the saved equipment slot.");
            }

            if (!character.Inventory.Contains(item))
            {
                character.Inventory.Add(item);
            }

            character.EquipItem(item);
        }

        foreach (string actionId in data.ActionIds)
        {
            CombatAction action = CombatActionCreator.Create(actionId);
            if (character.Actions.All(known => known.ID != action.ID))
            {
                character.Actions.Add(action);
            }
        }

        character.RestoreState(data.CurrentHealth, data.DeathRolls, data.ActionPoints);
        return character;
    }

    private string GetPath(string characterId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(characterId);
        if (characterId.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            throw new ArgumentException("The character ID contains invalid filename characters.", nameof(characterId));
        }

        return Path.Combine(SaveDirectory, $"{characterId}.json");
    }
}
