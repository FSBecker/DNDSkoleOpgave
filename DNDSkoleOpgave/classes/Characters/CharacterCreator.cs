using DNDSkoleOpgave.CharacterClasses;
using DNDSkoleOpgave.Items;
using DNDSkoleOpgave.Races;
using DNDSkoleOpgave.Utilities;

namespace DNDSkoleOpgave.Characters;

public static class CharacterCreator
{
    public static readonly IReadOnlyList<string> AvailableClasses = ["Barbarian", "Ranger", "Wizard"];
    public static readonly IReadOnlyList<string> AvailableRaces = ["Dwarf", "Elf", "Human"];

    public static PlayerCharacter Create(
        string characterName,
        string className,
        string raceName,
        int[]? baseStats = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(characterName);
        ArgumentException.ThrowIfNullOrWhiteSpace(className);
        ArgumentException.ThrowIfNullOrWhiteSpace(raceName);

        CharacterClass characterClass = CreateClass(className);
        CharacterRace characterRace = CreateRace(raceName);
        string id = Guid.NewGuid().ToString("N");

        PlayerCharacter character = new(
            id,
            characterName,
            characterClass,
            characterRace,
            baseStats);

        GiveStartingItems(character);
        character.UnlockActionsForCurrentLevel();
        return character;
    }

    private static CharacterClass CreateClass(string className) => className.Trim().ToLowerInvariant() switch
    {
        "barbarian" => new Barbarian(),
        "ranger" => new Ranger(),
        "wizard" => new Wizard(),
        _ => throw new ArgumentException($"Unknown character class '{className}'.", nameof(className))
    };

    private static CharacterRace CreateRace(string raceName) => raceName.Trim().ToLowerInvariant() switch
    {
        "dwarf" => new Dwarf(),
        "elf" => new Elf(),
        "human" => new Human(),
        _ => throw new ArgumentException($"Unknown character race '{raceName}'.", nameof(raceName))
    };

    private static void GiveStartingItems(PlayerCharacter character)
    {
        foreach (string itemId in character.CharacterClass.StartingItemIds)
        {
            CoreItem item = ItemCatalog.Create(itemId);
            character.Inventory.Add(item);

            if (item is EquipmentItem equipment && character.Equipment[equipment.EquipmentSlot] is null)
            {
                character.Equipment.Equip(equipment);
            }
        }
    }
}
