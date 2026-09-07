using DNDSkoleOpgave.CharacterClasses;
using DNDSkoleOpgave.Combat;
using DNDSkoleOpgave.Items;
using DNDSkoleOpgave.Persistence;
using DNDSkoleOpgave.Races;

namespace DNDSkoleOpgave.Characters;

public static class EnemyCreator
{
    public static EnemyCharacter Create(EnemyDefinition enemy, GameDefinitions definitions)
    {
        ArgumentNullException.ThrowIfNull(enemy);
        ArgumentNullException.ThrowIfNull(definitions);

        ClassDefinition classDefinition = definitions.Classes.First(value =>
            value.EnemyOnly && value.Name.Equals(enemy.ClassName, StringComparison.OrdinalIgnoreCase));
        RaceDefinition raceDefinition = definitions.Races.First(value =>
            value.EnemyOnly && value.Name.Equals(enemy.RaceName, StringComparison.OrdinalIgnoreCase));

        EnemyCharacter character = new(enemy.Id, enemy.Name, enemy.Level,
            CharacterClassCreator.Create(classDefinition),
            CharacterRaceCreator.Create(raceDefinition), enemy.BaseStats);

        IEnumerable<string> grantedItemIds = classDefinition.StartingItemIds.Concat(
            classDefinition.LevelUnlocks
                .Where(entry => entry.Key <= enemy.Level)
                .OrderBy(entry => entry.Key)
                .SelectMany(entry => entry.Value.ItemIds));

        foreach (string itemId in grantedItemIds)
        {
            ItemDefinition itemDefinition = definitions.Items.First(value =>
                value.Id.Equals(itemId, StringComparison.OrdinalIgnoreCase));
            CoreItem item = ItemCreator.Create(itemDefinition, definitions);
            character.Inventory.Add(item);
            if (item is EquipmentItem equipment && character.Equipment[equipment.EquipmentSlot] is null)
                character.EquipItem(equipment);
        }

        foreach (string actionId in character.CharacterClass.GetActionIdsForLevel(character.Level))
        {
            ActionDefinition actionDefinition = definitions.Actions.First(value =>
                value.Id.Equals(actionId, StringComparison.OrdinalIgnoreCase));
            CombatAction action = CombatActionCreator.Create(actionDefinition, definitions);
            character.Actions.Add(action);
        }

        foreach (LootEntryDefinition loot in enemy.LootTable)
        {
            ItemDefinition itemDefinition = definitions.Items.First(value =>
                value.Id.Equals(loot.ItemId, StringComparison.OrdinalIgnoreCase));
            character.LootTable.Add(new LootEntry(
                loot.ItemId,
                loot.ChancePercent,
                () => ItemCreator.Create(itemDefinition, definitions)));
        }

        return character;
    }
}
