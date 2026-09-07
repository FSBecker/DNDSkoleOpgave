using DNDSkoleOpgave.CharacterClasses;
using DNDSkoleOpgave.Items;
using DNDSkoleOpgave.Races;
using DNDSkoleOpgave.Utilities;

namespace DNDSkoleOpgave.Characters;

public static class CharacterCreator
{
    public static IReadOnlyList<string> AvailableClasses => CharacterClassCreator.AvailableNames;
    public static IReadOnlyList<string> AvailableRaces => CharacterRaceCreator.AvailableNames;

    public static PlayerCharacter Create(
        string characterName,
        string className,
        string raceName,
        int[]? baseStats = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(characterName);
        ArgumentException.ThrowIfNullOrWhiteSpace(className);
        ArgumentException.ThrowIfNullOrWhiteSpace(raceName);

        CharacterClass characterClass = CharacterClassCreator.Create(className);
        CharacterRace characterRace = CharacterRaceCreator.Create(raceName);
        string id = IdGenerator.Create();

        PlayerCharacter character = new(
            id,
            characterName,
            characterClass,
            characterRace,
            baseStats);

        GiveStartingItems(character);
        character.ApplyLevelRewardsForCurrentLevel();
        return character;
    }

    private static void GiveStartingItems(PlayerCharacter character)
    {
        foreach (string itemId in character.CharacterClass.StartingItemIds)
        {
            CoreItem item = ItemCreator.Create(itemId);
            character.Inventory.Add(item);

            if (item is EquipmentItem equipment && character.Equipment[equipment.EquipmentSlot] is null)
            {
                character.EquipItem(equipment);
            }
        }
    }
}
