namespace DNDSkoleOpgave.Races;

public static class CharacterRaceCreator
{
    private static readonly Dictionary<string, Func<CharacterRace>> RaceFactories = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Dwarf"] = () => new Dwarf(),
        ["Elf"] = () => new Elf(),
        ["Human"] = () => new Human()
    };

    public static IReadOnlyList<string> AllRaceNames => RaceFactories.Keys.ToList();

    public static void Load(Persistence.GameDefinitions definitions)
    {
        RaceFactories.Clear();
        foreach (Persistence.RaceDefinition race in definitions.AllRaces)
        {
            if (!race.EnemyOnly)
                RaceFactories[race.Name] = () => Create(race);
        }
    }

    public static CharacterRace Create(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (!RaceFactories.TryGetValue(name.Trim(), out Func<CharacterRace>? createRace))
            throw new ArgumentException($"Unknown character race '{name}'.", nameof(name));
        return createRace();
    }

    public static CharacterRace Create(Persistence.RaceDefinition definition) => new DefinedCharacterRace(definition);
}
