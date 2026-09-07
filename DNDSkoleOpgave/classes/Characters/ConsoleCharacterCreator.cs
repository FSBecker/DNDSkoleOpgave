namespace DNDSkoleOpgave.Characters;

public sealed class ConsoleCharacterCreator
{
    private readonly TextReader _input;
    private readonly TextWriter _output;

    public ConsoleCharacterCreator(TextReader input, TextWriter output)
    {
        _input = input ?? throw new ArgumentNullException(nameof(input));
        _output = output ?? throw new ArgumentNullException(nameof(output));
    }

    public PlayerCharacter Create()
    {
        _output.WriteLine("=== Character Creator ===");
        string name = ReadRequired("Character name: ");
        string race = Choose("race", CharacterCreator.AvailableRaces);
        string characterClass = Choose("class", CharacterCreator.AvailableClasses);

        PlayerCharacter character = CharacterCreator.Create(name, characterClass, race);
        PrintSummary(character);
        return character;
    }

    private string Choose(string category, IReadOnlyList<string> choices)
    {
        while (true)
        {
            _output.WriteLine($"Choose a {category}:");
            for (int index = 0; index < choices.Count; index++)
            {
                _output.WriteLine($"  {index + 1}. {choices[index]}");
            }

            _output.Write("> ");
            if (int.TryParse(_input.ReadLine(), out int selection)
                && selection >= 1
                && selection <= choices.Count)
            {
                return choices[selection - 1];
            }

            _output.WriteLine("Please enter one of the displayed numbers.");
        }
    }

    private string ReadRequired(string prompt)
    {
        while (true)
        {
            _output.Write(prompt);
            string? value = _input.ReadLine();
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }

            _output.WriteLine("A name is required.");
        }
    }

    private void PrintSummary(PlayerCharacter character)
    {
        _output.WriteLine();
        _output.WriteLine($"Created {character.CharacterName}, a {character.CharacterRace.RaceName} {character.CharacterClass.ClassName}.");
        _output.WriteLine($"Health: {character.CurrentHealth}/{character.MaximumHealth}");
        _output.WriteLine($"Armor class: {character.CalculateArmorClass()}");
        _output.WriteLine($"Equipped: {string.Join(", ", character.Equipment.All.Values.Select(item => item.Name))}");
        _output.WriteLine($"Inventory: {string.Join(", ", character.Inventory.Select(item => item.Name))}");
        _output.WriteLine($"Actions: {string.Join(", ", character.Actions.Select(action => action.Name))}");
    }
}
