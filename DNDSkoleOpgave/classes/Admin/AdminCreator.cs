using DNDSkoleOpgave.Enums;
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
            _output.WriteLine("3. Create item");
            _output.WriteLine("4. Create combat action, ability, or spell");
            _output.WriteLine("5. Create reusable effect");
            _output.WriteLine("6. List definitions");
            _output.WriteLine("0. Save and exit");

            try
            {
                switch (ReadInt("> ", 0, 6))
                {
                    case 1:
                        AddUnique(definitions.Classes, CreateClass(definitions), value => value.Name, "class");
                        break;
                    case 2:
                        AddUnique(definitions.Races, CreateRace(), value => value.Name, "race");
                        break;
                    case 3:
                        AddUnique(definitions.Items, CreateItem(definitions), value => value.Id, "item");
                        break;
                    case 4:
                        AddUnique(definitions.Actions, CreateAction(definitions), value => value.Id, "action");
                        break;
                    case 5:
                        AddUnique(definitions.Effects, CreateEffect(definitions), value => value.Id, "effect");
                        break;
                    case 6:
                        ListDefinitions(definitions);
                        continue;
                    case 0:
                        repository.Save(path, definitions);
                        _output.WriteLine($"Definitions saved to {Path.GetFullPath(path)}");
                        return;
                }

                repository.Save(path, definitions);
                _output.WriteLine("Created and saved.");
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
            BaseHealth = ReadInt("Base health: ", 1, 1000),
            HealthPerLevel = ReadInt("Health gained per level: ", 0, 1000),
            StartingItemIds = SelectMany(
                "Select starting items",
                definitions.Items,
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
            definition.LevelLockedActionIds[level] = SelectMany(
                "Select actions unlocked at this level",
                definitions.Actions,
                action => $"{action.Name} ({action.Id})",
                action => action.Id);
        }

        return definition;
    }

    private RaceDefinition CreateRace()
    {
        RaceDefinition definition = new()
        {
            Name = ReadRequired("Race name: "),
            Description = ReadRequired("Description: ")
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

    private ItemDefinition CreateItem(GameDefinitions definitions)
    {
        ItemType itemType = ReadEnum<ItemType>("Item type");
        ItemDefinition definition = new()
        {
            Id = GenerateUniqueId(definitions.Items.Select(item => item.Id)),
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
                definitions.Actions,
                action => $"{action.Name} ({action.Id})",
                action => action.Id);
        }

        return definition;
    }

    private ActionDefinition CreateAction(GameDefinitions definitions)
    {
        string[] kinds = ["attack", "ability", "spell"];
        string kind = kinds[ReadChoice("Action kind", kinds)];
        ActionDefinition definition = new()
        {
            Id = GenerateUniqueId(definitions.Actions.Select(action => action.Id)),
            Name = ReadRequired("Action name: "),
            Kind = kind,
            ActionCost = ReadInt("Action-point cost: ", 0, 1000),
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

        if (definitions.Effects.Count > 0)
        {
            definition.EffectIds = SelectMany(
                "Select effects for this action",
                definitions.Effects,
                effect => $"{effect.Name} ({effect.Id})",
                effect => effect.Id,
                allowEmpty: true);
        }

        return definition;
    }

    private EffectDefinition CreateEffect(GameDefinitions definitions) => new()
    {
        Id = GenerateUniqueId(definitions.Effects.Select(effect => effect.Id)),
        Name = ReadRequired("Effect name: "),
        Type = ReadEnum<ActionEffectType>("Effect type"),
        Amount = ReadInt("Effect amount: ", 0, 1000)
    };

    private void ListDefinitions(GameDefinitions definitions)
    {
        _output.WriteLine($"Classes: {string.Join(", ", definitions.Classes.Select(value => value.Name))}");
        _output.WriteLine($"Races: {string.Join(", ", definitions.Races.Select(value => value.Name))}");
        _output.WriteLine($"Items: {string.Join(", ", definitions.Items.Select(value => value.Id))}");
        _output.WriteLine($"Actions: {string.Join(", ", definitions.Actions.Select(value => value.Id))}");
        _output.WriteLine($"Effects: {string.Join(", ", definitions.Effects.Select(value => value.Id))}");
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
