using DNDSkoleOpgave.CharacterClasses;
using DNDSkoleOpgave.Combat;
using DNDSkoleOpgave.Items;
using DNDSkoleOpgave.Persistence;
using DNDSkoleOpgave.Races;

namespace DNDSkoleOpgave.Characters;

public static class EnemyCreator
{
    public static EnemyCharacter Create(EnemyDefinition enemy, GameDefinitions definitions,
        int? level = null, string? instanceId = null, string? name = null)
    {
        ArgumentNullException.ThrowIfNull(enemy);
        ArgumentNullException.ThrowIfNull(definitions);

        ClassDefinition characterClass = FindClass(enemy, definitions);
        RaceDefinition race = FindRace(enemy, definitions);
        EnemyCharacter character = new(instanceId ?? enemy.Id, name ?? enemy.Name, level ?? enemy.Level,
            CharacterClassCreator.Create(characterClass), CharacterRaceCreator.Create(race), enemy.BaseStats);

        GiveItems(character, characterClass, definitions);
        GiveActions(character, definitions);
        GiveLootTable(character, enemy, definitions);
        return character;
    }

    private static ClassDefinition FindClass(EnemyDefinition enemy, GameDefinitions definitions) =>
        definitions.AllClasses.First(value => value.EnemyOnly &&
            value.Name.Equals(enemy.ClassName, StringComparison.OrdinalIgnoreCase));

    private static RaceDefinition FindRace(EnemyDefinition enemy, GameDefinitions definitions) =>
        definitions.AllRaces.First(value => value.EnemyOnly &&
            value.Name.Equals(enemy.RaceName, StringComparison.OrdinalIgnoreCase));

    private static void GiveItems(EnemyCharacter character, ClassDefinition characterClass, GameDefinitions definitions)
    {
        List<string> itemIds = new(characterClass.StartingItemIds);
        foreach (var entry in characterClass.LevelUnlocks.OrderBy(entry => entry.Key))
        {
            if (entry.Key <= character.Level)
                itemIds.AddRange(entry.Value.ItemIds);
        }

        foreach (string itemId in itemIds)
        {
            ItemDefinition definition = definitions.AllItems.First(item =>
                item.Id.Equals(itemId, StringComparison.OrdinalIgnoreCase));
            CoreItem item = ItemCreator.Create(definition, definitions);
            character.AddItem(item);
            if (item is EquipmentItem equipment && character.Equipment[equipment.EquipmentSlot] is null)
                character.EquipItem(equipment);
        }
    }

    private static void GiveActions(EnemyCharacter character, GameDefinitions definitions)
    {
        foreach (string actionId in character.CharacterClass.GetActionIdsForLevel(character.Level))
        {
            ActionDefinition action = definitions.AllActions.First(value =>
                value.Id.Equals(actionId, StringComparison.OrdinalIgnoreCase));
            character.Actions.Add(CombatActionCreator.Create(action, definitions));
        }
    }

    private static void GiveLootTable(EnemyCharacter character, EnemyDefinition enemy, GameDefinitions definitions)
    {
        foreach (LootEntryDefinition loot in enemy.LootTable)
        {
            ItemDefinition item = definitions.AllItems.First(value =>
                value.Id.Equals(loot.ItemId, StringComparison.OrdinalIgnoreCase));
            character.LootTable.Add(new LootEntry(loot.ItemId, loot.ChancePercent,
                () => ItemCreator.Create(item, definitions)));
        }
    }
}
