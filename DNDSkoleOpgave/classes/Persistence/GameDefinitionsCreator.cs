using DNDSkoleOpgave.CharacterClasses;
using DNDSkoleOpgave.Combat;
using DNDSkoleOpgave.Combat.Spells;
using DNDSkoleOpgave.Items;
using DNDSkoleOpgave.Races;
using DNDSkoleOpgave.Utilities;

namespace DNDSkoleOpgave.Persistence;

public static class GameDefinitionsCreator
{
    public static GameDefinitions Create() => new()
    {
        Classes = CharacterClassCreator.AvailableNames
            .Select(CharacterClassCreator.Create)
            .Select(CreateClassDefinition)
            .ToList(),
        Races = CharacterRaceCreator.AvailableNames
            .Select(CharacterRaceCreator.Create)
            .Select(CreateRaceDefinition)
            .ToList(),
        Items = ItemCatalog.AllIds
            .Select(ItemCreator.Create)
            .Select(CreateItemDefinition)
            .ToList(),
        Actions = ActionCatalog.All.Values
            .Select(CreateActionDefinition)
            .ToList()
    };

    private static ClassDefinition CreateClassDefinition(CharacterClass characterClass) => new()
    {
        Name = characterClass.ClassName,
        Description = characterClass.Description,
        BaseHealth = characterClass.BaseHealth,
        HealthPerLevel = characterClass.HealthPerLevel,
        StatBuffs = new(characterClass.StatBuffs),
        StartingItemIds = [.. characterClass.StartingItemIds],
        LevelLockedActionIds = characterClass.LevelLockedActions.ToDictionary(
            entry => entry.Key,
            entry => entry.Value.ToList())
    };

    private static RaceDefinition CreateRaceDefinition(CharacterRace race) => new()
    {
        Name = race.RaceName,
        Description = race.Description,
        StatBuffs = new(race.StatBuffs)
    };

    private static ItemDefinition CreateItemDefinition(CoreItem item)
    {
        EquipmentItem? equipment = item as EquipmentItem;
        WeaponItem? weapon = item as WeaponItem;
        return new ItemDefinition
        {
            Id = item.ID,
            Name = item.Name,
            ItemType = item.ItemType,
            EquipmentSlot = equipment?.EquipmentSlot,
            ArmorClass = equipment?.ArmorClass,
            WeaponType = weapon?.WeaponType,
            ActionIds = weapon?.ActionIds.ToList() ?? []
        };
    }

    private static ActionDefinition CreateActionDefinition(CombatAction action)
    {
        WeakFireball? fireball = action as WeakFireball;
        return new ActionDefinition
        {
            Id = action.ID,
            Name = action.Name,
            Kind = action switch
            {
                SpellAction => "spell",
                AbilityAction => "ability",
                _ => "attack"
            },
            ActionCost = action.ActionCost,
            DiceAmount = action.DiceAmount,
            DiceSides = action.DiceSides,
            PrimaryStat = action.PrimaryStat,
            TargetsAlly = (action as SpellAction)?.TargetsAlly,
            SavingThrowDifficulty = fireball?.SavingThrowDifficulty,
            BurningDamageDiceSides = fireball?.BurningDamageDiceSides,
            BurningDurationDiceSides = fireball?.BurningDurationDiceSides,
            Effects = action.Effects.Select(effect => new EffectDefinition
            {
                Id = $"{action.ID}-{effect.Type.ToString().ToLowerInvariant()}",
                Name = effect.Type.ToString(),
                Type = effect.Type,
                Amount = effect.Amount
            }).ToList()
        };
    }
}
