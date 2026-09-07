using DNDSkoleOpgave.CharacterClasses;
using DNDSkoleOpgave.Combat;
using DNDSkoleOpgave.Combat.Spells;
using DNDSkoleOpgave.Items;
using DNDSkoleOpgave.Races;
using DNDSkoleOpgave.Utilities;

namespace DNDSkoleOpgave.Persistence;

public static class GameDefinitionsCreator
{
    public static GameDefinitions Create()
    {
        GameDefinitions definitions = CreatePlayerDefinitions();
        AddPotionEffect(definitions);
        AddEnemyDefinitions(definitions);
        return definitions;
    }

    private static GameDefinitions CreatePlayerDefinitions() => new()
    {
        AllClasses = CharacterClassCreator.AllClassNames
            .Select(CharacterClassCreator.Create)
            .Select(CreateClassDefinition)
            .ToList(),
        AllRaces = CharacterRaceCreator.AllRaceNames
            .Select(CharacterRaceCreator.Create)
            .Select(CreateRaceDefinition)
            .ToList(),
        AllItems = ItemCatalog.AllItemIds
            .Select(ItemCreator.Create)
            .Select(CreateItemDefinition)
            .ToList(),
        AllActions = ActionCatalog.AllActions.Values
            .Select(CreateActionDefinition)
            .ToList()
    };

    private static void AddPotionEffect(GameDefinitions definitions)
    {
        definitions.AllEffects.Add(new EffectDefinition
        {
            Id = "small-potion-healing", Name = "Potion healing",
            Type = Enums.ActionEffectType.Healing, Amount = 6, Target = Enums.EffectTarget.Self
        });
    }

    private static void AddEnemyDefinitions(GameDefinitions definitions)
    {
        definitions.AllClasses.Add(new ClassDefinition
        {
            Name = "Monster", Description = "Wave enemy", EnemyOnly = true,
            BaseHealth = 8, HealthPerLevel = 3,
            StartingItemIds = ["short-sword"],
            LevelLockedActionIds = { [1] = ["short-sword-strike"] }
        });

        AddEnemy(definitions, "Goblin", new() { [Enums.CharacterStat.Dexterity] = 2 });
        AddEnemy(definitions, "Orc", new() { [Enums.CharacterStat.Constitution] = 4 });
        AddEnemy(definitions, "Skeleton", new());
    }

    private static void AddEnemy(GameDefinitions definitions, string name,
        Dictionary<Enums.CharacterStat, int> statBuffs)
    {
        definitions.AllRaces.Add(new RaceDefinition
        {
            Name = name, Description = "Enemy race", EnemyOnly = true, StatBuffs = statBuffs
        });
        definitions.AllEnemies.Add(new EnemyDefinition
        {
            Id = name.ToLowerInvariant(), Name = name, RaceName = name, ClassName = "Monster"
        });
    }

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
            entry => entry.Value.ToList()),
        LevelUnlocks = characterClass.LevelUnlocks.ToDictionary(
            entry => entry.Key,
            entry => new LevelUnlockDefinition
            {
                HealthBonus = entry.Value.HealthBonus,
                StatBonuses = new(entry.Value.StatBonuses),
                ActionIds = [.. entry.Value.ActionIds],
                ItemIds = [.. entry.Value.ItemIds]
            })
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
            ActionIds = weapon?.ActionIds.ToList() ?? [],
            EffectIds = item is ConsumableItem consumable
                ? consumable.Effects.Select(effect => $"{item.ID}-{effect.Type.ToString().ToLowerInvariant()}").ToList()
                : []
        };
    }

    private static ActionDefinition CreateActionDefinition(CombatAction action)
    {
        SpellAction? fireball = action as SpellAction;
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
                Amount = effect.Amount,
                Target = effect.Target,
                Timing = effect.Timing,
                DurationTurns = effect.DurationTurns
            }).ToList()
        };
    }
}
