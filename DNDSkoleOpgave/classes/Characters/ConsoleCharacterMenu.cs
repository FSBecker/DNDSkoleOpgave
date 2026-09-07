using DNDSkoleOpgave.Persistence;

namespace DNDSkoleOpgave.Characters;

public sealed class ConsoleCharacterMenu
{
    private readonly TextReader _input;
    private readonly TextWriter _output;

    public ConsoleCharacterMenu(TextReader input, TextWriter output)
    {
        _input = input ?? throw new ArgumentNullException(nameof(input));
        _output = output ?? throw new ArgumentNullException(nameof(output));
    }

    public PlayerCharacter CreateOrLoad(CharacterJsonRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        IReadOnlyList<string> savedIds = repository.GetSavedCharacterIds();
        if (savedIds.Count == 0)
        {
            return new ConsoleCharacterCreator(_input, _output).Create();
        }

        _output.Write("Create a new character or load one? [C/L]: ");
        string? choice = _input.ReadLine();
        if (!string.Equals(choice?.Trim(), "L", StringComparison.OrdinalIgnoreCase))
        {
            return new ConsoleCharacterCreator(_input, _output).Create();
        }

        List<PlayerCharacter> characters = savedIds.Select(repository.Load).ToList();
        while (true)
        {
            _output.WriteLine("Choose a saved character:");
            for (int index = 0; index < characters.Count; index++)
            {
                PlayerCharacter character = characters[index];
                _output.WriteLine($"  {index + 1}. {character.CharacterName} - Level {character.Level} {character.CharacterRace.RaceName} {character.CharacterClass.ClassName}");
            }

            _output.Write("> ");
            if (int.TryParse(_input.ReadLine(), out int selection)
                && selection >= 1
                && selection <= characters.Count)
            {
                PlayerCharacter loaded = characters[selection - 1];
                _output.WriteLine($"Loaded {loaded.CharacterName}.");
                return loaded;
            }

            _output.WriteLine("Please enter one of the displayed numbers.");
        }
    }
}
