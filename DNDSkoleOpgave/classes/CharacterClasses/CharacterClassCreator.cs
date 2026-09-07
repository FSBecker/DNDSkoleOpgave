namespace DNDSkoleOpgave.CharacterClasses;

public static class CharacterClassCreator
{
    private static readonly Dictionary<string, Func<CharacterClass>> ClassFactories = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Barbarian"] = () => new Barbarian(),
        ["Ranger"] = () => new Ranger(),
        ["Wizard"] = () => new Wizard()
    };

    public static IReadOnlyList<string> AllClassNames => ClassFactories.Keys.ToList();

    public static void Load(Persistence.GameDefinitions definitions)
    {
        ClassFactories.Clear();
        foreach (Persistence.ClassDefinition characterClass in definitions.AllClasses)
        {
            if (!characterClass.EnemyOnly)
                ClassFactories[characterClass.Name] = () => Create(characterClass);
        }
    }

    public static CharacterClass Create(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (!ClassFactories.TryGetValue(name.Trim(), out Func<CharacterClass>? createClass))
            throw new ArgumentException($"Unknown character class '{name}'.", nameof(name));
        return createClass();
    }

    public static CharacterClass Create(Persistence.ClassDefinition definition) => new DefinedCharacterClass(definition);
}
