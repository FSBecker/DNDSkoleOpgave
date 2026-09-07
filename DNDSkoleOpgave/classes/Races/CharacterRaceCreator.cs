namespace DNDSkoleOpgave.Races;

public static class CharacterRaceCreator
{
    public static IReadOnlyList<string> AvailableNames { get; } = ["Dwarf", "Elf", "Human"];

    public static CharacterRace Create(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return name.Trim().ToLowerInvariant() switch
        {
            "dwarf" => new Dwarf(),
            "elf" => new Elf(),
            "human" => new Human(),
            _ => throw new ArgumentException($"Unknown character race '{name}'.", nameof(name))
        };
    }
}
