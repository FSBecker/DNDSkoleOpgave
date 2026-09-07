using DNDSkoleOpgave.Enums;
using DNDSkoleOpgave.Characters;
using DNDSkoleOpgave.Persistence;
using DNDSkoleOpgave.Utilities;

namespace DNDSkoleOpgave.Admin;

public sealed class AdminCreator
{
    private readonly TextReader _input;
    private readonly TextWriter _output;

    public AdminCreator(TextReader input, TextWriter output)
    {
        _input = input ?? throw new ArgumentNullException(nameof(input));
        _output = output ?? throw new ArgumentNullException(nameof(output));
    }

    public void Run(GameDefinitions definitions, GameDefinitionsJsonRepository repository, string path)
    {
        ArgumentNullException.ThrowIfNull(definitions);
        ArgumentNullException.ThrowIfNull(repository);

        while (true)
        {
            _output.WriteLine();
            _output.WriteLine("=== Admin Creator ===");
            _output.WriteLine("1. Create class");
            _output.WriteLine("2. Create race");
            _output.WriteLine("3. Create enemy");
            _output.WriteLine("4. Create item");
            _output.WriteLine("5. Create combat action, ability, or spell");
            _output.WriteLine("6. Create reusable effect");
            _output.WriteLine("7. List definitions");
            _output.WriteLine("8. Edit an existing definition");
            _output.WriteLine("0. Save and exit");

            try
            {
                switch (ReadInt("> ", 0, 8))
                {
                    case 1:
                        AddUnique(definitions.AllClasses, CreateClass(definitions), value => value.Name, "class");
                        break;
                    case 2:
                        AddUnique(definitions.AllRaces, CreateRace(), value => value.Name, "race");
                        break;
                    case 3:
                    {
                        EnemyDefinition enemy = CreateEnemy(definitions);
                        _ = EnemyCreator.Create(enemy, definitions);
                        AddUnique(definitions.AllEnemies, enemy, value => value.Id, "enemy");
                        break;
                    }
                    case 4:
                        AddUnique(definitions.AllItems, CreateItem(definitions), value => value.Id, "item");
                        break;
                    case 5:
                        AddUnique(definitions.AllActions, CreateAction(definitions), value => value.Id, "action");
                        break;
                    case 6:
                        AddUnique(definitions.AllEffects, CreateEffect(definitions), value => value.Id, "effect");
                        break;
                    case 7:
                        ListDefinitions(definitions);
                        continue;
                    case 8:
                        EditDefinition(definitions);
                        break;
                    case 0:
                        repository.Save(path, definitions);
                        _output.WriteLine($"Definitions saved to {Path.GetFullPath(path)}");
                        return;
                }

                repository.Save(path, definitions);
                _output.WriteLine("Changes saved.");
            }
            catch (InvalidOperationException exception)
            {
                _output.WriteLine($"Could not create definition: {exception.Message}");
            }
        }
    }

    private ClassDefinition CreateClass(GameDefinitions definitions)
    {
        ClassDefinition definition = new()
        {
            Name = ReadRequired("Class name: "),
            Description = ReadRequired("Description: "),
            EnemyOnly = ReadYesNo("Is this class for enemies only? [Y/N]: "),
            BaseHealth = ReadInt("Base health: ", 1, 1000),
            HealthPerLevel = ReadInt("Health gained per level: ", 0, 1000),
            StartingItemIds = SelectMany(
                "Select starting items",
                definitions.AllItems,
                item => $"{item.Name} ({item.Id})",
                item => item.Id)
        };

        foreach (CharacterStat stat in Enum.GetValues<CharacterStat>())
        {
            int buff = ReadInt($"{stat} buff (-20 to 20): ", -20, 20);
            if (buff != 0)
            {
                definition.StatBuffs[stat] = buff;
            }
        }

        while (ReadYesNo("Add a level unlock? [Y/N]: "))
        {
            int level = ReadInt("Level: ", 1, 1000);
            LevelUnlockDefinition unlock = new()
            {
                HealthBonus = ReadInt("Bonus maximum health: ", 0, 1000),
                ActionIds = SelectMany(
                "Select actions unlocked at this level",
                definitions.AllActions,
                action => $"{action.Name} ({action.Id})",
                    action => action.Id),
                ItemIds = SelectMany(
                    "Select items granted at this level",
                    definitions.AllItems,
                    item => $"{item.Name} ({item.Id})",
                    item => item.Id)
            };

            foreach (CharacterStat stat in Enum.GetValues<CharacterStat>())
            {
                int bonus = ReadInt($"{stat} bonus at level {level} (-20 to 20): ", -20, 20);
                if (bonus != 0)
                {
                    unlock.StatBonuses[stat] = bonus;
                }
            }

            definition.LevelUnlocks[level] = unlock;
        }

        return definition;
    }

    private RaceDefinition CreateRace()
    {
        RaceDefinition definition = new()
        {
            Name = ReadRequired("Race name: "),
            Description = ReadRequired("Description: "),
            EnemyOnly = ReadYesNo("Is this race for enemies only? [Y/N]: ")
        };

        foreach (CharacterStat stat in Enum.GetValues<CharacterStat>())
        {
            int buff = ReadInt($"{stat} buff (-20 to 20): ", -20, 20);
            if (buff != 0)
            {
                definition.StatBuffs[stat] = buff;
            }
        }

        return definition;
    }

    private EnemyDefinition CreateEnemy(GameDefinitions definitions)
    {
        List<ClassDefinition> enemyClasses = definitions.AllClasses.Where(value => value.EnemyOnly).ToList();
        List<RaceDefinition> enemyRaces = definitions.AllRaces.Where(value => value.EnemyOnly).ToList();
        if (enemyClasses.Count == 0 || enemyRaces.Count == 0)
        {
            throw new InvalidOperationException(
                "Create at least one enemy-only class and one enemy-only race before creating an enemy.");
        }

        ClassDefinition selectedClass = enemyClasses[ReadChoice(
            "Enemy class",
            enemyClasses.Select(value => value.Name).ToList())];
        RaceDefinition selectedRace = enemyRaces[ReadChoice(
            "Enemy race",
            enemyRaces.Select(value => value.Name).ToList())];

        int[] stats = new int[Enum.GetValues<CharacterStat>().Length];
        foreach (CharacterStat stat in Enum.GetValues<CharacterStat>())
        {
            stats[(int)stat] = ReadInt($"Base {stat}: ", 1, 100);
        }

        EnemyDefinition enemy = new()
        {
            Id = GenerateUniqueId(definitions.AllEnemies.Select(enemy => enemy.Id)),
            Name = ReadRequired("Enemy name: "),
            ClassName = selectedClass.Name,
            RaceName = selectedRace.Name,
            Level = ReadInt("Enemy level: ", 1, 1000),
            BaseStats = stats
        };

        while (definitions.AllItems.Count > enemy.LootTable.Count
               && ReadYesNo("Add an item to this enemy's loot table? [Y/N]: "))
        {
            List<ItemDefinition> availableItems = definitions.AllItems
                .Where(item => enemy.LootTable.All(loot => !loot.ItemId.Equals(
                    item.Id, StringComparison.OrdinalIgnoreCase)))
                .ToList();
            ItemDefinition selectedItem = availableItems[ReadChoice(
                "Loot item",
                availableItems.Select(item => $"{item.Name} ({item.Id})").ToList())];
            enemy.LootTable.Add(new LootEntryDefinition
            {
                ItemId = selectedItem.Id,
                ChancePercent = ReadInt("Drop chance percentage (1-100): ", 1, 100)
            });
        }

        return enemy;
    }

    private ItemDefinition CreateItem(GameDefinitions definitions)
    {
        ItemType itemType = ReadEnum<ItemType>("Item type");
        ItemDefinition definition = new()
        {
            Id = GenerateUniqueId(definitions.AllItems.Select(item => item.Id)),
            Name = ReadRequired("Item name: "),
            ItemType = itemType
        };

        if (itemType is ItemType.Equipment or ItemType.Weapon or ItemType.Shield)
        {
            definition.EquipmentSlot = ReadEnum<EquipmentSlot>("Equipment slot");
            definition.ArmorClass = ReadInt("Armor class bonus: ", 0, 1000);
        }

        if (itemType == ItemType.Weapon)
        {
            definition.WeaponType = ReadEnum<WeaponType>("Weapon type");
            definition.ActionIds = SelectMany(
                "Select actions granted by this weapon",
                definitions.AllActions,
                action => $"{action.Name} ({action.Id})",
                action => action.Id);
        }

        if (itemType == ItemType.Consumable)
        {
            definition.EffectIds = SelectMany(
                "Select effects applied when this consumable is used",
                definitions.AllEffects,
                effect => $"{effect.Name} ({effect.Type}, {effect.Target}, {effect.Timing})",
                effect => effect.Id,
                allowEmpty: false);
        }

        return definition;
    }

    private ActionDefinition CreateAction(GameDefinitions definitions)
    {
        string[] kinds = ["attack", "ability", "spell"];
        string kind = kinds[ReadChoice("Action kind", kinds)];
        ActionDefinition definition = new()
        {
            Id = GenerateUniqueId(definitions.AllActions.Select(action => action.Id)),
            Name = ReadRequired("Action name: "),
            Kind = kind,
            ActionCost = ReadInt("Action-point cost: ", 1, 1000),
            DiceAmount = ReadInt("Number of dice: ", 1, 1000),
            DiceSides = ReadInt("Sides per die: ", 2, 1000),
            PrimaryStat = ReadEnum<CharacterStat>("Primary stat")
        };

        if (kind == "spell")
        {
            definition.TargetsAlly = ReadYesNo("Does the spell target an ally? [Y/N]: ");
            if (ReadYesNo("Does it require a saving throw? [Y/N]: "))
            {
                definition.SavingThrowDifficulty = ReadInt("Saving-throw difficulty: ", 1, 1000);
            }

            if (ReadYesNo("Does it apply burning damage? [Y/N]: "))
            {
                definition.BurningDamageDiceSides = ReadInt("Burning damage die sides: ", 2, 1000);
                definition.BurningDurationDiceSides = ReadInt("Burning duration die sides: ", 2, 1000);
            }
        }

        if (definitions.AllEffects.Count > 0)
        {
            definition.EffectIds = SelectMany(
                "Select effects for this action",
                definitions.AllEffects,
                effect => $"{effect.Name} ({effect.Id})",
                effect => effect.Id,
                allowEmpty: true);
        }

        return definition;
    }

    private EffectDefinition CreateEffect(GameDefinitions definitions)
    {
        string name = ReadRequired("Effect name: ");
        ActionEffectType type = ReadEnum<ActionEffectType>("Effect type");
        EffectTarget target = ReadEnum<EffectTarget>("Effect target");
        EffectTiming timing = ReadEnum<EffectTiming>("Effect timing");
        return new EffectDefinition
        {
            Id = GenerateUniqueId(definitions.AllEffects.Select(effect => effect.Id)),
            Name = name,
            Type = type,
            Target = target,
            Timing = timing,
            Amount = ReadInt(timing == EffectTiming.Instant
                ? "Effect amount: "
                : "Effect amount per turn: ", 0, 1000),
            DurationTurns = timing == EffectTiming.OverTime
                ? ReadInt("Duration in turns: ", 1, 1000)
                : 1
        };
    }

    private void EditDefinition(GameDefinitions definitions)
    {
        string[] categories = ["Class", "Race", "Enemy", "Item", "Action", "Effect"];
        switch (ReadChoice("Definition type to edit", categories))
        {
            case 0:
                EditClass(definitions);
                break;
            case 1:
                EditRace(definitions);
                break;
            case 2:
                EditEnemy(definitions);
                break;
            case 3:
                EditItem(definitions);
                break;
            case 4:
                EditAction(definitions);
                break;
            case 5:
                EditEffect(definitions);
                break;
        }
    }

    private void EditClass(GameDefinitions definitions)
    {
        int index = SelectExisting("class", definitions.AllClasses, value => value.Name);
        ClassDefinition oldValue = definitions.AllClasses[index];
        ClassDefinition newValue = CreateClass(definitions);
        EnsureUniqueExcept(definitions.AllClasses, newValue.Name, value => value.Name, index, "class");
        if (!newValue.EnemyOnly && definitions.AllEnemies.Any(enemy =>
                enemy.ClassName.Equals(oldValue.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("This class is used by an enemy and must remain enemy-only.");
        }
        definitions.AllClasses[index] = newValue;

        foreach (EnemyDefinition enemy in definitions.AllEnemies.Where(enemy =>
                     enemy.ClassName.Equals(oldValue.Name, StringComparison.OrdinalIgnoreCase)))
        {
            enemy.ClassName = newValue.Name;
        }
    }

    private void EditRace(GameDefinitions definitions)
    {
        int index = SelectExisting("race", definitions.AllRaces, value => value.Name);
        RaceDefinition oldValue = definitions.AllRaces[index];
        RaceDefinition newValue = CreateRace();
        EnsureUniqueExcept(definitions.AllRaces, newValue.Name, value => value.Name, index, "race");
        if (!newValue.EnemyOnly && definitions.AllEnemies.Any(enemy =>
                enemy.RaceName.Equals(oldValue.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("This race is used by an enemy and must remain enemy-only.");
        }
        definitions.AllRaces[index] = newValue;

        foreach (EnemyDefinition enemy in definitions.AllEnemies.Where(enemy =>
                     enemy.RaceName.Equals(oldValue.Name, StringComparison.OrdinalIgnoreCase)))
        {
            enemy.RaceName = newValue.Name;
        }
    }

    private void EditEnemy(GameDefinitions definitions)
    {
        int index = SelectExisting("enemy", definitions.AllEnemies, value => value.Name);
        string id = definitions.AllEnemies[index].Id;
        EnemyDefinition replacement = CreateEnemy(definitions);
        replacement.Id = id;
        definitions.AllEnemies[index] = replacement;
    }

    private void EditItem(GameDefinitions definitions)
    {
        int index = SelectExisting("item", definitions.AllItems, value => $"{value.Name} ({value.Id})");
        string id = definitions.AllItems[index].Id;
        ItemDefinition replacement = CreateItem(definitions);
        replacement.Id = id;
        definitions.AllItems[index] = replacement;
    }

    private void EditAction(GameDefinitions definitions)
    {
        int index = SelectExisting("action", definitions.AllActions, value => $"{value.Name} ({value.Id})");
        string id = definitions.AllActions[index].Id;
        ActionDefinition replacement = CreateAction(definitions);
        replacement.Id = id;
        definitions.AllActions[index] = replacement;
    }

    private void EditEffect(GameDefinitions definitions)
    {
        int index = SelectExisting("effect", definitions.AllEffects, value => $"{value.Name} ({value.Id})");
        string id = definitions.AllEffects[index].Id;
        EffectDefinition replacement = CreateEffect(definitions);
        replacement.Id = id;
        definitions.AllEffects[index] = replacement;
    }

    private int SelectExisting<T>(string category, IReadOnlyList<T> values, Func<T, string> getLabel)
    {
        if (values.Count == 0)
        {
            throw new InvalidOperationException($"There are no {category} definitions to edit.");
        }

        return ReadChoice($"Select a {category} to edit", values.Select(getLabel).ToList());
    }

    private static void EnsureUniqueExcept<T>(
        IReadOnlyList<T> values,
        string key,
        Func<T, string> getKey,
        int excludedIndex,
        string category)
    {
        if (values.Where((_, index) => index != excludedIndex)
            .Any(value => getKey(value).Equals(key, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"A {category} named '{key}' already exists.");
        }
    }

    private void ListDefinitions(GameDefinitions definitions)
    {
        _output.WriteLine($"Classes: {string.Join(", ", definitions.AllClasses.Select(value => value.Name))}");
        _output.WriteLine($"Races: {string.Join(", ", definitions.AllRaces.Select(value => value.Name))}");
        _output.WriteLine($"Items: {string.Join(", ", definitions.AllItems.Select(value => value.Id))}");
        _output.WriteLine($"Actions: {string.Join(", ", definitions.AllActions.Select(value => value.Id))}");
        _output.WriteLine($"Effects: {string.Join(", ", definitions.AllEffects.Select(value => value.Id))}");
        _output.WriteLine($"Enemies: {string.Join(", ", definitions.AllEnemies.Select(value => value.Name))}");
    }

    private void AddUnique<T>(List<T> list, T value, Func<T, string> getKey, string category)
    {
        string key = getKey(value);
        if (list.Any(existing => getKey(existing).Equals(key, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"A {category} named '{key}' already exists.");
        }

        list.Add(value);
    }

    private static string GenerateUniqueId(IEnumerable<string> existingIds)
    {
        HashSet<string> ids = existingIds.ToHashSet(StringComparer.OrdinalIgnoreCase);
        string id;
        do
        {
            id = IdGenerator.Create();
        }
        while (ids.Contains(id));

        return id;
    }

    private T ReadEnum<T>(string label) where T : struct, Enum
    {
        T[] values = Enum.GetValues<T>();
        string[] names = values.Select(value => value.ToString()).ToArray();
        return values[ReadChoice(label, names)];
    }

    private int ReadChoice(string label, IReadOnlyList<string> choices)
    {
        _output.WriteLine($"{label}:");
        for (int index = 0; index < choices.Count; index++)
        {
            _output.WriteLine($"  {index + 1}. {choices[index]}");
        }

        return ReadInt("> ", 1, choices.Count) - 1;
    }

    private List<string> SelectMany<T>(
        string label,
        IReadOnlyList<T> choices,
        Func<T, string> getLabel,
        Func<T, string> getId,
        bool allowEmpty = true)
    {
        if (choices.Count == 0)
        {
            _output.WriteLine($"{label}: no definitions are available yet.");
            return [];
        }

        while (true)
        {
            _output.WriteLine($"{label}:");
            for (int index = 0; index < choices.Count; index++)
            {
                _output.WriteLine($"  {index + 1}. {getLabel(choices[index])}");
            }

            _output.Write(allowEmpty
                ? "Enter numbers separated by commas, or leave blank for none: "
                : "Enter numbers separated by commas: ");
            string input = ReadLine().Trim();
            if (allowEmpty && input.Length == 0)
            {
                return [];
            }

            string[] parts = input.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length > 0
                && parts.All(part => int.TryParse(part, out int number) && number >= 1 && number <= choices.Count))
            {
                return parts
                    .Select(int.Parse)
                    .Distinct()
                    .Select(number => getId(choices[number - 1]))
                    .ToList();
            }

            _output.WriteLine("Select using only the displayed numbers.");
        }
    }

    private bool ReadYesNo(string prompt)
    {
        while (true)
        {
            _output.Write(prompt);
            string value = ReadLine().Trim();
            if (value.Equals("Y", StringComparison.OrdinalIgnoreCase)) return true;
            if (value.Equals("N", StringComparison.OrdinalIgnoreCase)) return false;
            _output.WriteLine("Enter Y or N.");
        }
    }

    private int ReadInt(string prompt, int minimum, int maximum)
    {
        while (true)
        {
            _output.Write(prompt);
            if (int.TryParse(ReadLine(), out int value) && value >= minimum && value <= maximum)
            {
                return value;
            }

            _output.WriteLine($"Enter a number from {minimum} to {maximum}.");
        }
    }

    private string ReadRequired(string prompt)
    {
        while (true)
        {
            _output.Write(prompt);
            string value = ReadLine().Trim();
            if (value.Length > 0) return value;
            _output.WriteLine("A value is required.");
        }
    }

    private string ReadLine() => _input.ReadLine() ?? throw new EndOfStreamException("Console input ended.");
}
