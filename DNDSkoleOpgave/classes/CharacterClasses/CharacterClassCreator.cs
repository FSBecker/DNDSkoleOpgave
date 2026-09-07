namespace DNDSkoleOpgave.CharacterClasses;

public static class CharacterClassCreator
{
    public static IReadOnlyList<string> AvailableNames { get; } = ["Barbarian", "Ranger", "Wizard"];

    public static CharacterClass Create(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return name.Trim().ToLowerInvariant() switch
        {
            "barbarian" => new Barbarian(),
            "ranger" => new Ranger(),
            "wizard" => new Wizard(),
            _ => throw new ArgumentException($"Unknown character class '{name}'.", nameof(name))
        };
    }

    public static CharacterClass Create(Persistence.ClassDefinition definition) => new DefinedCharacterClass(definition);
}
